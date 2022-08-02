using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.ShortcutManagement;
using System.Collections.Generic;

namespace Avrahamy.Meshes {
    [EditorTool("Debug Mesh")]
    public class MeshDebuggerTool : EditorTool {
        private float scale = 2f;
        private int fontSize = 10;
        private readonly Collider[] results = new Collider[1];
        private Vector3 normal;
        private bool hasHit;
        private Vector3 lastVertexPosition;

        [Shortcut("Mesh Debugger Tool", KeyCode.U)]
        private static void Shortcut() {
            ToolManager.SetActiveTool(typeof(MeshDebuggerTool));
        }

        public override void OnToolGUI(EditorWindow window) {
            if (!(window is SceneView)) return;
            if (!ToolManager.IsActiveTool(this)) return;

            Handles.BeginGUI();
            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUIUtility.labelWidth = 60f;
                scale = EditorGUILayout.FloatField("Scale", scale);
                GUILayout.Space(10f);
                fontSize = EditorGUILayout.IntField("Font Size", fontSize);
                if (hasHit) {
                    EditorGUILayout.LabelField(lastVertexPosition.ToString());
                }
                GUILayout.FlexibleSpace();
            }
            Handles.EndGUI();

            // OnGUI is called twice, once to find layout positions and sizes
            // (EventType.Layout), and second to actually render them
            // (EventType.Repaint).
            // This phase is the Layout phase. Don't change hasHit before rendering
            // the UI that was just calculated.
            if (Event.current.type == EventType.Layout) return;

            var position = GetCurrentMousePositionInScene();
            var hits = Physics.OverlapBoxNonAlloc(position, Vector3.one * 0.25f, results);
            hasHit = hits > 0;
            if (!hasHit) return;

            var meshFilter = results[0].GetComponent<MeshFilter>();
            if (meshFilter == null) return;
            var relativePosition = meshFilter.transform.InverseTransformPoint(position);
            var mesh = meshFilter.sharedMesh;
            var closestSqrDistance = float.MaxValue;
            var indices = new List<int>();
            var vertexPosition = Vector3.zero;
            for (var i = 0; i < mesh.vertices.Length; i++) {
                var offset = mesh.vertices[i] - relativePosition;
                if (offset.sqrMagnitude + 0.0001f < closestSqrDistance) {
                    indices.Clear();
                    indices.Add(i);
                    closestSqrDistance = offset.sqrMagnitude;
                    vertexPosition = mesh.vertices[i];
                } else if (offset.sqrMagnitude - 0.0001f <= closestSqrDistance) {
                    indices.Add(i);
                }
            }
            lastVertexPosition = vertexPosition;
            vertexPosition = meshFilter.transform.TransformPoint(vertexPosition);
            var normals = new List<Vector3>();
            var uvs = new List<Vector2>();
            foreach (var i in indices) {
                normals.Add(meshFilter.transform.TransformVector(mesh.normals[i]).normalized);
                uvs.Add(mesh.uv[i]);
            }

            Handles.color = Color.LerpUnclamped(Color.white, Color.red, 0.4f);
            Handles.DrawSolidDisc(position, normal, 0.03f * scale);

            var multipleVertices = normals.Count > 1;
            for (int i = 0; i < normals.Count; i++) {
                Handles.color = Color.blue;
                var normal = normals[i];
                position = vertexPosition;
                if (multipleVertices) {
                    position += normal * 0.1f * scale;
                }
                DrawWireSphere(position, 0.05f * scale);

                if (multipleVertices) {
                    Handles.color = Color.yellow;
                    Handles.DrawLine(vertexPosition, position);
                }
                Handles.color = Color.green;
                Handles.DrawLine(position, position + normal * 0.5f * scale);

                var uv = uvs[i];
                var style = GUI.skin.label;
                style.fontSize = fontSize;
                style.alignment = TextAnchor.MiddleCenter;
                Handles.Label(position + normal * 0.4f * scale, $"({uv.x:N2}, {uv.y:N2})", style);
                style.fontStyle = FontStyle.Bold;
                Handles.Label(position, $"{indices[i]}", style);
            }

            // Force the window to repaint.
            window.Repaint();
        }

        private Vector3 GetCurrentMousePositionInScene() {
            var mousePosition = Event.current.mousePosition;
            var placeObject = HandleUtility.PlaceObject(mousePosition, out var newPosition, out normal);
            return placeObject ? newPosition : HandleUtility.GUIPointToWorldRay(mousePosition).GetPoint(10);
        }

        private static void DrawWireSphere(Vector3 center, float radius, float thickness = 0f) {
            Handles.DrawWireDisc(center, Vector3.up, radius, thickness);
            Handles.DrawWireDisc(center, Vector3.right, radius, thickness);
            Handles.DrawWireDisc(center, Vector3.forward, radius, thickness);
        }
    }
}
