using UnityEditor;
using UnityEngine;

namespace Avrahamy.EditorGadgets {
    public class ReadOnlyPropertyDrawer : CompoundPropertyDrawerBase<ReadOnlyAttribute>,
            ICompoundAttributeModifier {
        private bool wasEnabled;

        public void BeginModifier(SerializedProperty property) {
            wasEnabled = GUI.enabled;
            GUI.enabled = false;
        }

        public void EndModifier() {
            GUI.enabled = wasEnabled;
        }
    }
}