using UnityEditor;
using UnityEngine;

namespace Avrahamy.EditorGadgets {
    public class ConditionalHidePropertyDrawer : CompoundPropertyDrawerBase<ConditionalHideAttribute>,
            ICompoundAttributeToggle,
            ICompoundAttributeModifier,
            ICompoundAttributeHeightModifier {
        private bool wasEnabled;

        public bool ShouldDraw(SerializedProperty property) {
            bool enabled = attribute.GetConditionalHideAttributeResult(property);
            return !attribute.hideInInspector || enabled;
        }

        public void BeginModifier(SerializedProperty property) {
            wasEnabled = GUI.enabled;
            GUI.enabled = wasEnabled && attribute.GetConditionalHideAttributeResult(property);
        }

        public void EndModifier() {
            GUI.enabled = wasEnabled;
        }

        public bool GetPropertyHeight(SerializedProperty property, GUIContent label, ref float height, bool wasForced) {
            bool enabled = attribute.GetConditionalHideAttributeResult(property);

            if (!attribute.hideInInspector || enabled) {
                return false;
            }

            // The property is not being drawn
            // We want to undo the spacing added before and after the property
            height = -EditorGUIUtility.standardVerticalSpacing;
            return true;
        }
    }
}