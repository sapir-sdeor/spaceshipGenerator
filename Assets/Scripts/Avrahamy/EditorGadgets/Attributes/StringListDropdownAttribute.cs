namespace Avrahamy.EditorGadgets {
    public class StringListDropdownAttribute : CompoundAttributeBase {
        public readonly string stringListAssetName;

        public StringListDropdownAttribute(string stringListAssetName) {
            this.stringListAssetName = stringListAssetName;
        }
    }
}
