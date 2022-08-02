using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Avrahamy.EditorGadgets {
    /// <summary>
    /// This class is the actual PropertyDrawer for all Compound Attributes.
    /// It uses classes that inherit from CompoundPropertyDrawerBase and
    /// implement one or more of the following interfaces:
    /// # ICompoundAttributeToggle
    /// # ICompoundAttributeModifier
    /// # ICompoundAttributeLabelBuilder
    /// # ICompoundAttributeView
    /// # ICompoundAttributeHeightModifier
    /// </summary>
    [CustomPropertyDrawer(typeof(CompoundAttributeBase), true)]
    public class CompoundAttributeDrawer : PropertyDrawer {
        private PropertyAttribute firstAttribute;
        private readonly List<ICompoundAttributeToggle> toggles = new List<ICompoundAttributeToggle>();
        private readonly List<ICompoundAttributeModifier> modifiers = new List<ICompoundAttributeModifier>();
        private readonly List<ICompoundAttributeLabelBuilder> labelBuilders = new List<ICompoundAttributeLabelBuilder>();
        private readonly List<ICompoundAttributeView> views = new List<ICompoundAttributeView>();
        private readonly List<ICompoundAttributeHeightModifier> heightModifiers = new List<ICompoundAttributeHeightModifier>();

        private static readonly Dictionary<string, Type> attributeDrawersTypesCache = new Dictionary<string, Type>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            Init();
            if (Equals(firstAttribute, attribute)) {
                DoOnGUI(position, property, label);

                // Enable PopOut.
                PopupPropertyDrawer.PopOut(position, property);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var height = EditorGUI.GetPropertyHeight(property);
            if (!Equals(firstAttribute, attribute)) return height;

            var wasForced = false;
            foreach (var attr in heightModifiers) {
                wasForced = attr.GetPropertyHeight(property, label, ref height, wasForced);
            }
            return height;
        }

        private void Init() {
            if (firstAttribute != null) return;

            var attributes = fieldInfo.GetCustomAttributes(typeof(CompoundAttributeBase), true);
            foreach (CompoundAttributeBase attr in attributes) {
                var attributeTypeName = attr.PropertyDrawerTypeName;
                var type = GetAttributeDrawer(attributeTypeName);
                if (type == null) continue;

                var attributeDrawer = Activator.CreateInstance(type) as ICompoundAttributePropertyDrawer;
                if (attributeDrawer == null) {
                    Debug.LogError($"CompoundAttributeDrawer: Can't create instance of class {type.Name}");
                    continue;
                }
                attributeDrawer.Init(attr);

                if (attributeDrawer is ICompoundAttributeToggle toggle) {
                    toggles.Add(toggle);
                }
                if (attributeDrawer is ICompoundAttributeModifier modifier) {
                    modifiers.Add(modifier);
                }
                if (attributeDrawer is ICompoundAttributeLabelBuilder labelBuilder) {
                    labelBuilders.Add(labelBuilder);
                }
                if (attributeDrawer is ICompoundAttributeView view) {
                    views.Add(view);
                }
                if (attributeDrawer is ICompoundAttributeHeightModifier heightModifier) {
                    heightModifiers.Add(heightModifier);
                }
            }

            firstAttribute = attributes[0] as PropertyAttribute;
        }

        private static Type GetAttributeDrawer(string typeName) {
            if (attributeDrawersTypesCache.ContainsKey(typeName)) {
                return attributeDrawersTypesCache[typeName];
            }
            var type = Type.GetType(typeName);
            if (type == null) {
                Debug.LogError($"CompoundAttributeDrawer: Can't find class {typeName}");
                return null;
            }
            attributeDrawersTypesCache[typeName] = type;
            return type;
        }

        private void DoOnGUI(Rect position, SerializedProperty property, GUIContent label) {
            foreach (var attr in toggles) {
                if (!attr.ShouldDraw(property)) return;
            }

            foreach (var attr in modifiers) {
                attr.BeginModifier(property);
            }

            var originalRect = position;
            foreach (var attr in labelBuilders) {
                label = attr.BuildLabel(ref position, label, property);
            }

            if (label != null && !property.hasChildren) {
                position = EditorGUI.PrefixLabel(position, label);
            }

            var didDraw = false;
            foreach (var attr in views) {
                // TODO: Draw should draw label if != null and property.hasChildren.
                didDraw = attr.Draw(ref position, originalRect, property, label) || didDraw;
            }
            if (!didDraw) {
                var tooltip = label?.tooltip;
                var handler = PropertyHandler.GetHandler(property);
                handler.OnGUI(position, property, property.hasChildren ? label : GUIContent.none, true);
                if (!string.IsNullOrEmpty(tooltip)) {
                    EditorGUI.LabelField(position, new GUIContent(string.Empty, tooltip));
                }
            }

            foreach (var attr in modifiers) {
                attr.EndModifier();
            }
        }
    }

    public interface ICompoundAttributePropertyDrawer {
        void Init(PropertyAttribute attribute);
    }

    public interface ICompoundAttributeToggle : ICompoundAttributePropertyDrawer {
        bool ShouldDraw(SerializedProperty property);
    }

    public interface ICompoundAttributeModifier : ICompoundAttributePropertyDrawer {
        void BeginModifier(SerializedProperty property);
        void EndModifier();
    }

    public interface ICompoundAttributeLabelBuilder : ICompoundAttributePropertyDrawer {
        /// <summary>
        /// Returns null if the label was drawn or should not be drawn at all.
        /// Otherwise returns a valid label to be drawn.
        /// </summary>
        GUIContent BuildLabel(ref Rect position, GUIContent label, SerializedProperty property);
    }

    public interface ICompoundAttributeView : ICompoundAttributePropertyDrawer {
        /// <summary>
        /// Returns did draw the property field.
        /// </summary>
        bool Draw(ref Rect position, Rect originalRect, SerializedProperty property, GUIContent label);
    }

    public interface ICompoundAttributeHeightModifier : ICompoundAttributePropertyDrawer {
        /// <summary>
        /// Returns true if should force the current height.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <param name="height">The result will be written to this variable.</param>
        /// <param name="wasForced">If true, it means another attribute wanted to
        /// force the height to be at this value and it should (generally) not be
        /// changed further.</param>
        bool GetPropertyHeight(SerializedProperty property, GUIContent label, ref float height, bool wasForced);
    }
}