// Originally developed by Tom Kail at Inkle
// Modifications made by Luiz Wendt
// Released under the MIT Licence as held at https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BitStrap
{
	/// <summary>
	/// Extends how ScriptableObject object references are displayed in the inspector
	/// Also provides a button to create a new ScriptableObject if property is null.
	/// If you want to show all values under the object reference you can use ScriptableObjectAttribute
	/// </summary>
	[CustomPropertyDrawer(typeof(ScriptableObject), true)]
	public class ScriptableObjectDrawer : PropertyDrawer
	{

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			if (property.objectReferenceValue != null)
			{
				if (label != GUIContent.none)
				{
					EditorGUI.LabelField(new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), property.displayName);

					EditorGUI.PropertyField(new Rect(EditorGUIUtility.labelWidth + 14, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight),
						property, GUIContent.none, true);

					if (GUI.changed)
					{
						property.serializedObject.ApplyModifiedProperties();
					}

					if (property.objectReferenceValue == null)
					{
						GUIUtility.ExitGUI();
					}
				}
				else
				{
					EditorGUI.PropertyField(new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), property, GUIContent.none, true);
					if (GUI.changed)
					{
						property.serializedObject.ApplyModifiedProperties();
					}

					if (property.objectReferenceValue == null)
					{
						GUIUtility.ExitGUI();
					}
				}

			}
			else
			{
				const float BUTTON_WIDTH = 58;
				const float SPACING = 2;
				EditorGUI.ObjectField(new Rect(position.x, position.y, position.width - (BUTTON_WIDTH + SPACING) * 2, EditorGUIUtility.singleLineHeight), property, label);
				if (GUI.Button(new Rect(position.x + position.width - BUTTON_WIDTH * 2 - SPACING, position.y, BUTTON_WIDTH, EditorGUIUtility.singleLineHeight), "Find")) {
					var filter = "t:" + fieldInfo.FieldType;
					var guids = AssetDatabase.FindAssets(filter);
					if (guids.Length == 0) return;
					var guid = guids[0];
					var path = AssetDatabase.GUIDToAssetPath(guid);
					var asset = AssetDatabase.LoadAssetAtPath(path, fieldInfo.FieldType);
					property.objectReferenceValue = asset;
				}
				if (GUI.Button(new Rect(position.x + position.width - BUTTON_WIDTH, position.y, BUTTON_WIDTH, EditorGUIUtility.singleLineHeight), "Create"))
				{
					string selectedAssetPath = "Assets";
					if (property.serializedObject.targetObject is MonoBehaviour)
					{
						MonoScript ms = MonoScript.FromMonoBehaviour((MonoBehaviour)property.serializedObject.targetObject);
						selectedAssetPath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(ms));
					}
					Type type = fieldInfo.FieldType;
					if (type.IsArray)
					{
						type = type.GetElementType();
					}
					else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
					{
						type = type.GetGenericArguments()[0];
					}

					property.objectReferenceValue = CreateAssetWithSavePrompt(type, selectedAssetPath);
				}
			}
			property.serializedObject.ApplyModifiedProperties();
			EditorGUI.EndProperty();
		}

		// Creates a new ScriptableObject via the default Save File panel
		ScriptableObject CreateAssetWithSavePrompt(Type type, string path)
		{
			path = EditorUtility.SaveFilePanelInProject("Save ScriptableObject", "New " + type.Name + ".asset", "asset", "Enter a file name for the ScriptableObject.", path);
			if (path == "")
			{
				return null;
			}

			ScriptableObject asset = ScriptableObject.CreateInstance(type);
			AssetDatabase.CreateAsset(asset, path);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
			EditorGUIUtility.PingObject(asset);
			return asset;
		}
	}
}