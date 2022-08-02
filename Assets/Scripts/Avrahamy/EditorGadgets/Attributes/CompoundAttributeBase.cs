using UnityEngine;

namespace Avrahamy.EditorGadgets {
    public abstract class CompoundAttributeBase : PropertyAttribute {
        public virtual string PropertyDrawerTypeName {
            get {
                return GetType().FullName.Replace("Attribute", "PropertyDrawer")
                       + ", avrahamy-editorgadgets-editor";
            }
        }
    }
}
