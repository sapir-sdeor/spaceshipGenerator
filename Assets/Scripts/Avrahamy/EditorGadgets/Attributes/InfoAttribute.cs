namespace Avrahamy.EditorGadgets {
    public class InfoAttribute : CompoundAttributeBase {
        public readonly string tooltip;

        public InfoAttribute(string tooltip) {
            this.tooltip = tooltip;
        }
    }
}
