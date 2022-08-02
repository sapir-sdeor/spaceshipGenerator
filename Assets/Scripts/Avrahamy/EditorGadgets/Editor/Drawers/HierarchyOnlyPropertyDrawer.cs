using UnityEngine;
using UnityEditor;

namespace Avrahamy.EditorGadgets {
    public class HierarchyOnlyPropertyDrawer : CompoundPropertyDrawerBase<HierarchyOnlyAttribute>,
            ICompoundAttributeModifier {
        public void BeginModifier(SerializedProperty property) {
            if (property.objectReferenceValue == null) return;

            // If assigned a game object, try to find a component that implements
            // all of the interfaces and use that as the value of the field.
            var go = property.objectReferenceValue as GameObject;
            if (go == null) {
                var component = property.objectReferenceValue as Component;
                if (component != null) {
                    go = component.gameObject;
                }
            }

            if (go != null && go.scene.IsValid()) return;

            // Game Object is in a scene. It is not a prefab.
            Debug.LogError($"ERROR: Field {property.objectReferenceValue} only accepts things in the hierarchy!");
            property.objectReferenceValue = null;
        }

        public void EndModifier() {
            // Do nothing.
        }
    }
}
