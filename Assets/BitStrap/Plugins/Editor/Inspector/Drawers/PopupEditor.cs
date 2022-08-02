using UnityEditor;
using UnityEngine;

namespace Avrahamy.EditorGadgets {
    public class PopupEditor : EditorWindow {
        private static PopupEditor window;

        public static void ShowEditorFor(Object o) {
            if (window == null) {
                window = GetWindow<PopupEditor>();
            }
            window.SetTarget(o);
            window.titleContent = new GUIContent(o.ToString());
        }

        private Object targetObject;
        private Editor targetEditor;

        private void SetTarget(Object o) {
            if (targetObject == o) return;

            targetObject = o;
            targetEditor = Editor.CreateEditor(o);

            window.Focus();
        }

        private void OnGUI() {
            if (targetObject == null) return;
            targetEditor.OnInspectorGUI();
        }
    }
}