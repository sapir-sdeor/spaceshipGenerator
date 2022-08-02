using UnityEditor;
using UnityEngine;

namespace Avrahamy.EditorGadgets {
    public class InlinePropertyDrawer :
            CompoundPropertyDrawerBase<InlineAttribute>,
            ICompoundAttributeToggle,
            ICompoundAttributeLabelBuilder,
            ICompoundAttributeView {
        private bool hasReference;
        private bool foldout;

        public bool ShouldDraw(SerializedProperty property) {
            hasReference = property.type.StartsWith("PPtr") && property.objectReferenceValue != null;
            return true;
        }

        public GUIContent BuildLabel(ref Rect position, GUIContent label, SerializedProperty property) {
            if (!hasReference) return label;

            // Leave some room for Alt-Click Popup.
            var width = EditorGUIUtility.labelWidth - 2;
            var rect = new Rect(position.x, position.y, width, position.height);

            foldout = EditorGUI.Foldout(rect, foldout, label, true);

            position.x += width + 4;
            position.width -= width + 4;

            return null;
        }

        public bool Draw(ref Rect position, Rect originalRect, SerializedProperty property, GUIContent label) {
            EditorGUI.PropertyField(position, property, GUIContent.none, true);

            if (hasReference && foldout && property.objectReferenceValue) {
                EditorGUILayout.BeginVertical(BackgroundStyle.Get(Color.white * 0.3f));
                EditorGUI.indentLevel++;
                var editor = Editor.CreateEditor(property.objectReferenceValue);
                editor.OnInspectorGUI();
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }

            return true;
        }

        public static class BackgroundStyle {
            private static readonly GUIStyle style = new GUIStyle();
            private static Texture2D texture = new Texture2D(1, 1);

            public static GUIStyle Get(Color color) {
                if (texture == null) {
                    texture = new Texture2D(1, 1);
                }
                texture.SetPixel(0, 0, color);
                texture.Apply();
                style.normal.background = texture;
                return style;
            }
        }
    }
}