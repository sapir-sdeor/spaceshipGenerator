using UnityEngine;
using UnityEditor;

namespace Avrahamy.EditorGadgets {
    public class SortingLayerPropertyDrawer :
            CompoundPropertyDrawerBase<SortingLayerAttribute>,
            ICompoundAttributeLabelBuilder,
            ICompoundAttributeView {
        public GUIContent BuildLabel(ref Rect position, GUIContent label, SerializedProperty property) {
            if (property.propertyType != SerializedPropertyType.Integer) {
                EditorGUI.LabelField(position, "ERROR:", "May only apply to type int");
                return null;
            }
            return EditorGUI.BeginProperty(position, label, property);
        }

        public bool Draw(ref Rect position, Rect originalRect, SerializedProperty property, GUIContent label) {
            if (label != null && property.hasChildren) {
                position = EditorGUI.PrefixLabel(position, label);
            }

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            int layerCount = SortingLayer.layers.Length;
            var stringValues = new string[layerCount];

            for (int i = 0; i < layerCount; i++) {
                stringValues[i] = SortingLayer.layers[i].name;
            }

            InspectorUtilities.ShowDropdownForIntProperty(
                position,
                property,
                stringValues,
                LayerNameToID,
                LayerIDToName);

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
            return true;
        }

        public static int LayerNameToID(string name) {
            int layerCount = SortingLayer.layers.Length;

            for (int i = 0; i < layerCount; i++) {
                if (name == SortingLayer.layers[i].name) return SortingLayer.layers[i].id;
            }

            return -1;
        }

        public static string LayerIDToName(int id) {
            int layerCount = SortingLayer.layers.Length;

            for (int i = 0; i < layerCount; i++) {
                if (id == SortingLayer.layers[i].id) return SortingLayer.layers[i].name;
            }

            return null;
        }
    }
}
