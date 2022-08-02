using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BitStrap
{
	/// <summary>
	/// Searches through a target class in order to find all button attributes (<see cref="ButtonAttribute"/>).
	/// </summary>
	public sealed class ButtonAttributeHelper
	{
		private static object[] emptyParamList = new object[0];

		private IList<MethodInfo> methods = new List<MethodInfo>();
		private IList<MethodInfo> methodsDisabled = new List<MethodInfo>();
		private Object targetObject;

		public void Init( Object targetObject )
		{
			this.targetObject = targetObject;
			// (avrahamy) Allow to disable during edit mode.
			var allMethods = targetObject.GetType()
				.GetMethods( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
			foreach (var m in allMethods) {
				var attributes = m.GetCustomAttributes(typeof(ButtonAttribute), false);
				if (attributes.Length != 1 || m.GetParameters().Length > 0 || m.ContainsGenericParameters) continue;
				methods.Add(m);
				if (Application.isPlaying) continue;
				var buttonAttribute = attributes[0] as ButtonAttribute;
				if (buttonAttribute.disableInEditMode) {
					methodsDisabled.Add(m);
				}
			}
		}

		public void DrawButtons() {
			if (methods.Count > 0) {
				EditorGUILayout.HelpBox( "Click to execute methods!", MessageType.None );
				ShowMethodButtons();
			}
		}

		private void ShowMethodButtons() {
			var wasEnabled = GUI.enabled;
			foreach( MethodInfo method in methods ) {
				string buttonText = ObjectNames.NicifyVariableName( method.Name );
				GUI.enabled = !methodsDisabled.Contains(method);
				if( GUILayout.Button( buttonText ) ) {
					method.Invoke( targetObject, emptyParamList );
				}
			}
			GUI.enabled = wasEnabled;
		}
	}
}