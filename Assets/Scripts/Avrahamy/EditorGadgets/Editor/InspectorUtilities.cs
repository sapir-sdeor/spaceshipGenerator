using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Avrahamy.EditorGadgets {
    public static class InspectorUtilities {
        /// <summary>
        /// Returns true if the dropdown is displayed.
        /// </summary>
        public static bool ShowDropdownForStringProperty(Rect position, SerializedProperty property, IEnumerable<string> values, bool allowEmpty = true) {
            if (!GUI.Button(position, property.stringValue, EditorStyles.popup)) return false;

            var menu = new GenericMenu();
            if (allowEmpty) {
                menu.AddItem(new GUIContent("None"), string.IsNullOrEmpty(property.stringValue),
                    val => {
                        property.stringValue = string.Empty;
                        property.serializedObject.ApplyModifiedProperties();
                    }, string.Empty);
            }
            foreach (var value in values) {
                menu.AddItem(
                    new GUIContent(value),
                    value == property.stringValue,
                    val => {
                        property.stringValue = val.ToString();
                        property.serializedObject.ApplyModifiedProperties();
                    },
                    value);
            }
            menu.ShowAsContext();
            return true;
        }
        /// <summary>
        /// Returns true if the dropdown is displayed.
        /// </summary>
        public static bool ShowDropdownForIntProperty(Rect position, SerializedProperty property, IEnumerable<string> values, Func<string, int> StringToInt, Func<int, string> IntToString) {
            var stringValue = IntToString(property.intValue);
            if (!GUI.Button(position, stringValue, EditorStyles.popup)) return false;

            var menu = new GenericMenu();
            foreach (var value in values) {
                menu.AddItem(
                    new GUIContent(value),
                    value == stringValue,
                    val => {
                        property.intValue = StringToInt(val.ToString());
                        property.serializedObject.ApplyModifiedProperties();
                    },
                    value);
            }
            menu.ShowAsContext();
            return true;
        }

        public static SerializedProperty FindBaseOrSiblingProperty(this SerializedProperty property, string propertyName) {
            if (string.IsNullOrEmpty(propertyName)) return null;

            var relativeProperty = property.serializedObject.FindProperty(propertyName);

            // If base property is not found, look for the sibling property.
            if (relativeProperty == null) {
                string propertyPath = property.propertyPath;
                int localPathLength = property.name.Length;

                string newPropertyPath = propertyPath.Remove(propertyPath.Length - localPathLength, localPathLength) + propertyName;
                relativeProperty = property.serializedObject.FindProperty(newPropertyPath);

                // If a direct sibling property was not found, try to find the sibling of the array.
                if (relativeProperty == null && property.isArray) {
                    int propertyPathLength = propertyPath.Length;

                    int dotCount = 0;
                    const int SIBLING_OF_LIST_DOT_COUNT = 3;
                    for (int i = 1; i < propertyPathLength; i++) {
                        if (propertyPath[propertyPathLength - i] == '.') {
                            dotCount++;
                            if (dotCount >= SIBLING_OF_LIST_DOT_COUNT) {
                                localPathLength = i - 1;
                                break;
                            }
                        }
                    }

                    newPropertyPath = propertyPath.Remove(propertyPath.Length - localPathLength, localPathLength) + propertyName;
                    relativeProperty = property.serializedObject.FindProperty(newPropertyPath);
                }
            }

            return relativeProperty;
        }

        public static object GetValue(this SerializedProperty property, FieldInfo fieldInfo) {
            object instance = property.serializedObject.targetObject;
            if (property.depth > 0) {
                var elements = property.propertyPath.Split( '.' );
                for (var index = 0; index < elements.Length; index++) {
                    var element = elements[index];
                    var type = instance.GetType();
                    if (type.IsArray && element == "Array") {
                        ++index;
                        element = elements[index];
                        var numString = element.Substring(5, element.Length - 6);
                        var array = instance as Array;
                        instance = array.GetValue(int.Parse(numString));
                        if (index >= elements.Length) break;
                        ++index;
                        element = elements[index];
                        type = type.GetElementType();
                    }
                    do {
                        var flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
                        fieldInfo = type.GetField(element, flags);
                        type = type.BaseType;
                    } while (fieldInfo == null && type != null);
                    instance = fieldInfo.GetValue(instance);
                }
            } else {
                instance = fieldInfo.GetValue(instance);
            }
            return instance;
        }

        public static void DrawUILine(Color color, int thickness = 2, int paddingLeft = 10, int paddingTop = 10, int paddingRight = 10, int paddingBottom = 10) {
            // Accomodate for the default offset.
            paddingLeft -= 3;
            paddingRight -= 3;
            var r = EditorGUILayout.GetControlRect(GUILayout.Height(paddingTop + paddingBottom + thickness));
            r.height = thickness;
            r.y += paddingTop;
            r.x += paddingLeft;
            r.width -= paddingLeft + paddingRight;
            EditorGUI.DrawRect(r, color);
        }
    }
}
