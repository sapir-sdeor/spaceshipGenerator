using UnityEngine;

namespace Avrahamy.EditorGadgets {
    /// <summary>
    /// New Compound Property Drawers should inherit from this class and implement
    /// one or more of the ICompoundAttributePropertyDrawer interfaces.
    /// </summary>
    public class CompoundPropertyDrawerBase<T> : ICompoundAttributePropertyDrawer where T : PropertyAttribute {
        protected T attribute;

        public virtual void Init(PropertyAttribute attribute) {
            this.attribute = attribute as T;
        }
    }
}