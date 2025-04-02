using System.IO;
using BerserkPixel.Tilemap_Generator.Attributes;
using UnityEditor;
using UnityEngine;

namespace BerserkPixel.Tilemap_Generator.Utilities
{
    /// <summary>
    ///     Based on: https://forum.unity.com/threads/draw-a-field-only-if-a-condition-is-met.448855/
    /// </summary>
    [CustomPropertyDrawer(typeof(DrawIfAttribute))]
    public class DrawIfPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!ShowMe(property) && drawIf.disablingType == DrawIfAttribute.DisablingType.DontDraw)
                return -EditorGUIUtility.standardVerticalSpacing;
            return EditorGUI.GetPropertyHeight(property, label);
        }

        /// <summary>
        ///     Errors default to showing the property.
        /// </summary>
        private bool ShowMe(SerializedProperty property)
        {
            drawIf = attribute as DrawIfAttribute;
            // Replace propertyname to the value from the parameter
            var path = property.propertyPath.Contains(".")
                ? Path.ChangeExtension(property.propertyPath, drawIf.comparedPropertyName)
                : drawIf.comparedPropertyName;

            comparedField = property.serializedObject.FindProperty(path);

            if (comparedField == null)
            {
                Debug.LogError("Cannot find property with name: " + path);
                return true;
            }

            switch (comparedField.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    return comparedField.boolValue.Equals(drawIf.comparedValue);
                case SerializedPropertyType.ObjectReference:
                    return comparedField.objectReferenceValue != null;
                case SerializedPropertyType.Enum:
                    return ((int)drawIf.comparedValue & comparedField.enumValueFlag) != 0;
                default:
                    Debug.LogError("Data type of the property used for conditional hiding [" +
                                    comparedField.propertyType + "] is currently not supported");
                    return true;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // If the condition is met, simply draw the field.
            if (ShowMe(property))
            {
                var title = string.IsNullOrEmpty(drawIf.prefix) ? label.text : $"{drawIf.prefix}{label.text}";
                EditorGUI.PropertyField(position, property, new GUIContent(title));
            } //...check if the disabling type is read only. If it is, draw it disabled
            else if (drawIf.disablingType == DrawIfAttribute.DisablingType.ReadOnly)
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property);
                GUI.enabled = true;
            }
        }

        #region Fields

        // Reference to the attribute on the property.
        private DrawIfAttribute drawIf;

        // Field that is being compared.
        private SerializedProperty comparedField;

        #endregion
    }
}