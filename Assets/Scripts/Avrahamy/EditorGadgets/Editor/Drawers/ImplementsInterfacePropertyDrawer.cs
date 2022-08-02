using UnityEngine;
using UnityEditor;
using System.Text;

namespace Avrahamy.EditorGadgets {
    public class ImplementsInterfacePropertyDrawer : CompoundPropertyDrawerBase<ImplementsInterfaceAttribute>,
            ICompoundAttributeModifier {
        public void BeginModifier(SerializedProperty property) {
            if (property.objectReferenceValue == null) return;

            // If assigned a game object, try to find a component that implements
            // all of the interfaces and use that as the value of the field.
            var go = property.objectReferenceValue as GameObject;
            if (go != null) {
                ProcessGameObject(go, property);
                return;
            }

            // Not a Game Object or GO does not have a suitable component.
            var unimplementedInterface = GetUnimplementedInterface(property.objectReferenceValue);
            if (unimplementedInterface == null) {
                // Object implements all interfaces.
                return;
            }

            var component = property.objectReferenceValue as Component;
            if (component != null) {
                go = component.gameObject;
                ProcessGameObject(go, property);
                return;
            }

            Debug.LogError($"ERROR: Object must implement '{unimplementedInterface}'. {property.objectReferenceValue} is invalid!");
            property.objectReferenceValue = null;
        }

        public void EndModifier() {
            // Do nothing.
        }

        private System.Type GetUnimplementedInterface(Object obj) {
            foreach (var item in attribute.interfaces) {
                if (!item.IsInstanceOfType(obj)) {
                    return item;
                }
            }
            return null;
        }

        private bool ProcessGameObject(GameObject go, SerializedProperty property) {
            var firstInterface = attribute.interfaces[0];
            var comps = go.GetComponents(firstInterface);

            foreach (var comp in comps) {
                if (comp == property.serializedObject.targetObject) {
                    // This is the object containing the field. This is usually
                    // not what we want to hold.
                    continue;
                }
                if (GetUnimplementedInterface(comp) != null) continue;
                // Found a component that implements all interfaces.
                property.objectReferenceValue = comp;
                return true;
            }
            property.objectReferenceValue = null;

            var sb = new StringBuilder("ERROR: No component found that implements '");
            for (var i = 0; i < attribute.interfaces.Length; i++) {
                sb.Append(attribute.interfaces[i]);
                if (i < attribute.interfaces.Length - 1) {
                    sb.Append("', '");
                }
            }
            sb.Append("' in Game Object '");
            sb.Append(go.name);
            sb.Append("'");
            Debug.LogError(sb);
            return false;
        }
    }
}
