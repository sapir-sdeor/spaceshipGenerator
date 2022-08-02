using UnityEditor;
using UnityEngine;

namespace Avrahamy.EditorGadgets {
    public class PopupPropertyDrawer /*:
            CompoundPropertyDrawerBase<PopupAttribute>,
            ICompoundAttributeView*/ {

        /* Disabled as an attribute - it is included in BitStrap's NonNullableDrawer
           and PopOut is called in CompoundAttributeDrawer.
        public bool Draw(ref Rect position, Rect originalRect, SerializedProperty property, GUIContent label) {
            PopOut(originalRect, property);
            return false;
        }*/

        public static void PopOut(Rect rect, SerializedProperty property) {
            var hasReference = property.type.StartsWith("PPtr") && property.objectReferenceValue != null;
            if (!hasReference) return;

            var hittingControlLeftClick = Event.current.type == EventType.MouseDown && Event.current.alt && Event.current.button == 0;

            if (hittingControlLeftClick) {
                rect.width = EditorGUIUtility.labelWidth;
                var mouseOverLabel = rect.Contains(Event.current.mousePosition);
                if (mouseOverLabel) {
                    PopupEditor.ShowEditorFor(property.objectReferenceValue);
                }
            }
        }
    }
}