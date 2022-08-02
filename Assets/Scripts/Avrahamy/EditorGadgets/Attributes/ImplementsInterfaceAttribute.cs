using System;

namespace Avrahamy.EditorGadgets {
    /// <summary>
    /// Forces a serialized field to only hold objects that implement an interface.
    /// Use with Object to have one field that can hold objects that derive from
    /// ScriptableObject or MonoBehavior.
    /// Setting an object that does not implement the interface is not allowed
    /// (the field will remain empty and an error will be printed to the console).
    /// </summary>
    public class ImplementsInterfaceAttribute : CompoundAttributeBase {
        public readonly Type[] interfaces;

        /// <summary>
        /// The field will only allow objects that implements ALL of the given interfaces.
        /// </summary>
        public ImplementsInterfaceAttribute(params Type[] interfaces) {
            if (interfaces == null || interfaces.Length < 1) {
                throw new ArgumentException("At least 1 interface is required");
            }

            foreach (var item in interfaces) {
                if (!item.IsInterface) {
                    throw new ArgumentException($"{item} is not an interface");
                }
            }

            this.interfaces = interfaces;
        }
    }
}
