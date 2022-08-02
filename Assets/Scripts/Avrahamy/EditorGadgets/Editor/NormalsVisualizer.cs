using UnityEditor;
using UnityEngine;

namespace Avrahamy.EditorGadgets {
    [CustomEditor(typeof(MeshFilter))]
    public class NormalsVisualizer : Editor {
        private const string SHOW_MESH_NORMALS_KEY = "SHOW_MESH_NORMALS";
        private const string MESH_NORMALS_SIZE_KEY = "MESH_NORMALS_SIZE_KEY";
        private const string NORMALS_SKIP_KEY = "NORMALS_SKIP_KEY";
        private const string OCCLUDE_NORMALS_KEY = "OCCLUDE_NORMALS_KEY";

        private Mesh mesh;
        private bool showNormals;
        private float size;
        private int skip;
        private bool occlude;

        protected void OnEnable() {
            var mf = target as MeshFilter;
            if (mf != null) {
                mesh = mf.sharedMesh;
            }
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            var showNormalsStoredValue = EditorPrefs.GetBool(SHOW_MESH_NORMALS_KEY, false);
            showNormals = EditorGUILayout.Toggle("Show Normals", showNormalsStoredValue);
            if (showNormals != showNormalsStoredValue) {
                EditorPrefs.SetBool(SHOW_MESH_NORMALS_KEY, showNormals);
            }

            if (showNormals) {
                var sizeStoredValue = EditorPrefs.GetFloat(MESH_NORMALS_SIZE_KEY, 0.25f);
                size = EditorGUILayout.FloatField("Normals Size", sizeStoredValue);
                if (!Mathf.Approximately(size, sizeStoredValue)) {
                    EditorPrefs.SetFloat(MESH_NORMALS_SIZE_KEY, size);
                }

                var skipStoredValue = EditorPrefs.GetInt(NORMALS_SKIP_KEY, 1);
                skip = Mathf.Max(1, EditorGUILayout.IntField("Show every", skipStoredValue));
                if (skip != skipStoredValue) {
                    EditorPrefs.SetInt(NORMALS_SKIP_KEY, skip);
                }

                var occludeStoredValue = EditorPrefs.GetBool(OCCLUDE_NORMALS_KEY, true);
                occlude = EditorGUILayout.Toggle("Occlude Normals", occludeStoredValue);
                if (occlude != occludeStoredValue) {
                    EditorPrefs.SetBool(OCCLUDE_NORMALS_KEY, occlude);
                }
            }
        }

        protected void OnSceneGUI() {
            if (mesh == null || !showNormals || skip < 1) {
                return;
            }

            var originalZTest = Handles.zTest;
            if (occlude) {
                Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            }

            for (int i = 0; i < mesh.vertexCount; i += skip) {
                Handles.matrix = (target as MeshFilter).transform.localToWorldMatrix;
                Handles.color = Color.green;
                Handles.DrawLine(
                    mesh.vertices[i],
                    mesh.vertices[i] + mesh.normals[i] * size);
            }

            Handles.zTest = originalZTest;
        }
    }
}
