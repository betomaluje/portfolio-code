using System.Reflection;
using System.Text.RegularExpressions;
using Attributes;
using UnityEditor;
using UnityEngine;

namespace Editor.PropertyDrawers {
    [CustomPropertyDrawer(typeof(InspectorButtonAttribute))]
    public class InspectorButtonDrawer : PropertyDrawer {
        private MethodInfo _eventMethodInfo;

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label) {
            var inspectorButtonAttribute = (InspectorButtonAttribute) attribute;

            // adding spaces in between the method name
            var buttonName = Regex.Replace(inspectorButtonAttribute.MethodName, "(\\B[A-Z])", " $1");

            var style = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter
            };

            if (!GUILayout.Button(buttonName, style)) {
                return;
            }

            var eventOwnerType = prop.serializedObject.targetObject.GetType();

            var eventName = inspectorButtonAttribute.MethodName;

            // we try and get the name of the method of the class using reflection
            if (_eventMethodInfo == null) {
                _eventMethodInfo = eventOwnerType.GetMethod(eventName,
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            }

            // if we got it, we trigger it
            if (_eventMethodInfo != null) {
                _eventMethodInfo.Invoke(prop.serializedObject.targetObject, null);
            }
            else {
                Debug.LogWarning($"InspectorButton: Unable to find method {eventName} in {eventOwnerType}");
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var inspectorButtonAttribute = (InspectorButtonAttribute) attribute;
            return base.GetPropertyHeight(property, label) + inspectorButtonAttribute.verticalPadding;
        }
    }
}