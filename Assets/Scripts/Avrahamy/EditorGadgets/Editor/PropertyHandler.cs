using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace Avrahamy.EditorGadgets {
    public class PropertyHandler {
        private static MethodInfo getHandler;
        private static object[] getHandlerParams;

        private object handler;
        private Type type;

        private PropertyInfo propertyDrawerInfo;
        private MethodInfo guiHandler;
        private MethodInfo heightHandler;
        private object[] guiParams;
        private object[] heightParams;

        public PropertyDrawer propertyDrawer {
            get { return propertyDrawerInfo.GetValue(handler, null) as PropertyDrawer; }
        }

        static PropertyHandler() {
            getHandler = Type.GetType("UnityEditor.ScriptAttributeUtility, UnityEditor")
                .GetMethod("GetHandler", BindingFlags.NonPublic | BindingFlags.Static);
            getHandlerParams = new object[1];
        }

        private PropertyHandler(object handler) {
            this.handler = handler;

            type = handler.GetType();
            propertyDrawerInfo = type.GetProperty("propertyDrawer", BindingFlags.NonPublic | BindingFlags.Instance);
            guiHandler = type.GetMethod("OnGUI", BindingFlags.Public | BindingFlags.Instance);
            heightHandler = type.GetMethod("GetPropertyHeight", BindingFlags.Public | BindingFlags.Instance);
            guiParams = new object[4];
            heightParams = new object[2];
        }

        public bool OnGUI(Rect position, SerializedProperty property, GUIContent label, bool includeChildren) {
            guiParams[0] = position;
            guiParams[1] = property;
            guiParams[2] = label;
            guiParams[3] = includeChildren;

            return (bool)guiHandler.Invoke(handler, guiParams);
        }

        public float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            heightParams[0] = property;
            heightParams[1] = label;

            return (float)heightHandler.Invoke(handler, heightParams);
        }

        public static PropertyHandler GetHandler(SerializedProperty property) {
            getHandlerParams[0] = property;
            return new PropertyHandler(getHandler.Invoke(null, getHandlerParams));
        }
    }
}