using UnityEngine;
using System;

// Original version of the ConditionalHideAttribute created by Brecht Lecluyse (www.brechtos.com)
// Modified by: Or Avrahamy

namespace Avrahamy.EditorGadgets {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class ConditionalHideAttribute : CompoundAttributeBase {
        public readonly string conditionalSourceField = "";
        public readonly string[] conditionalSourceFields = {};
        public readonly bool[] conditionalSourceFieldInverseBools = {};
        public readonly bool hideInInspector = true;
        public readonly bool inverse = false;

        // Use this for initialization
        public ConditionalHideAttribute(string conditionalSourceField) {
            this.conditionalSourceField = conditionalSourceField;
        }

        public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector) {
            this.conditionalSourceField = conditionalSourceField;
            this.hideInInspector = hideInInspector;
        }

        public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector, bool inverse) {
            this.conditionalSourceField = conditionalSourceField;
            this.hideInInspector = hideInInspector;
            this.inverse = inverse;
        }

        public ConditionalHideAttribute(string[] conditionalSourceFields, bool[] conditionalSourceFieldInverseBools, bool hideInInspector = true) {
            this.conditionalSourceFields = conditionalSourceFields;
            this.conditionalSourceFieldInverseBools = conditionalSourceFieldInverseBools;
            this.hideInInspector = hideInInspector;
        }

        public ConditionalHideAttribute(string[] conditionalSourceFields, bool hideInInspector, bool inverse) {
            this.conditionalSourceFields = conditionalSourceFields;
            this.hideInInspector = hideInInspector;
            this.inverse = inverse;
        }

#if UNITY_EDITOR
        public bool GetConditionalHideAttributeResult(UnityEditor.SerializedProperty property) {
            var enabled = true;

            //Handle primary property
            UnityEditor.SerializedProperty sourcePropertyValue = null;
            //Get the full relative property path of the sourcefield so we can have nested hiding.Use old method when dealing with arrays
            if (!property.isArray) {
                string propertyPath = property.propertyPath; //returns the property path of the property we want to apply the attribute to
                string conditionPath = propertyPath.Replace(property.name, conditionalSourceField); //changes the path to the conditionalsource property path
                sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

                //if the find failed->fall back to the old system
                if (sourcePropertyValue == null) {
                    //original implementation (doesn't work with nested serializedObjects)
                    sourcePropertyValue = property.serializedObject.FindProperty(conditionalSourceField);
                }
            } else {
                //original implementation (doesn't work with nested serializedObjects)
                sourcePropertyValue = property.serializedObject.FindProperty(conditionalSourceField);
            }


            if (sourcePropertyValue != null) {
                enabled = CheckPropertyType(sourcePropertyValue);
            } else {
                //Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
            }

            //Handle the unlimited property array
            string[] conditionalSourceFieldArray = conditionalSourceFields;
            bool[] conditionalSourceFieldInverseArray = conditionalSourceFieldInverseBools;
            for (int index = 0; index < conditionalSourceFieldArray.Length; ++index) {
                UnityEditor.SerializedProperty sourcePropertyValueFromArray = null;
                if (!property.isArray) {
                    string propertyPath = property.propertyPath; //returns the property path of the property we want to apply the attribute to
                    string conditionPath = propertyPath.Replace(property.name, conditionalSourceFieldArray[index]); //changes the path to the conditionalsource property path
                    sourcePropertyValueFromArray = property.serializedObject.FindProperty(conditionPath);

                    //if the find failed->fall back to the old system
                    if (sourcePropertyValueFromArray == null) {
                        //original implementation (doens't work with nested serializedObjects)
                        sourcePropertyValueFromArray = property.serializedObject.FindProperty(conditionalSourceFieldArray[index]);
                    }
                } else {
                    // original implementation(doens't work with nested serializedObjects)
                    sourcePropertyValueFromArray = property.serializedObject.FindProperty(conditionalSourceFieldArray[index]);
                }

                //Combine the results
                if (sourcePropertyValueFromArray != null) {
                    bool propertyEnabled = CheckPropertyType(sourcePropertyValueFromArray);
                    if (conditionalSourceFieldInverseArray.Length >= (index + 1) && conditionalSourceFieldInverseArray[index]) propertyEnabled = !propertyEnabled;

                    enabled = enabled && propertyEnabled;
                } else {
                    //Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHAtt.ConditionalSourceField);
                }
            }


            //wrap it all up
            if (inverse) enabled = !enabled;

            return enabled;
        }

        private bool CheckPropertyType(UnityEditor.SerializedProperty sourcePropertyValue) {
            //Note: add others for custom handling if desired
            switch (sourcePropertyValue.propertyType) {
                case UnityEditor.SerializedPropertyType.Boolean:
                    return sourcePropertyValue.boolValue;
                case UnityEditor.SerializedPropertyType.ObjectReference:
                    return sourcePropertyValue.objectReferenceValue != null;
                case UnityEditor.SerializedPropertyType.Float:
                    return !Mathf.Approximately(sourcePropertyValue.floatValue, 0f);
                default:
                    Debug.LogError($"Data type of the property {sourcePropertyValue.name} used for conditional hiding [{sourcePropertyValue.propertyType}] is currently not supported");
                    return true;
            }
        }
#endif
    }
}