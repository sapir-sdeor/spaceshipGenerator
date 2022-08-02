using UnityEditor;
using UnityEngine;

namespace Avrahamy.EditorGadgets {
    public class InfoPropertyDrawer : CompoundPropertyDrawerBase<InfoAttribute>,
            ICompoundAttributeLabelBuilder,
            ICompoundAttributeView {
        private const int PADDING = 2;

        private static Texture2D texture;

        public override void Init(PropertyAttribute attribute) {
            base.Init(attribute);
            if (texture == null) {
                texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Scripts/Avrahamy/EditorGadgets/Editor/EditorIcons/info.png");
            }
        }

        public GUIContent BuildLabel(ref Rect position, GUIContent label, SerializedProperty property) {
            label.tooltip = attribute.tooltip;
            return label;
        }

        public bool Draw(ref Rect position, Rect originalRect, SerializedProperty property, GUIContent label) {
            var size = position.height;
            var rect = new Rect(position.x + position.width - size, position.y, size, size);
            label = new GUIContent(texture, attribute.tooltip);
            GUI.Label(rect, label);

            position.width -= size + PADDING;
            return false;
        }
    }
}