using BerserkPixel.Tilemap_Generator.Attributes;
using UnityEngine;
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
#endif

namespace BerserkPixel.Tilemap_Generator.Utilities
{
#if UNITY_EDITOR
    /// <summary>
    ///     Draws the property field for any field marked with ExpandableAttribute.
    /// </summary>
    [CustomPropertyDrawer(typeof(ExpandableAttribute), true)]
    public class ExpandableAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var totalHeight = 0.0f;

            totalHeight += EditorGUIUtility.singleLineHeight;

            if (property.objectReferenceValue == null)
                return totalHeight;

            if (!property.isExpanded)
                return totalHeight;

            var targetObject = new SerializedObject(property.objectReferenceValue);

            var field = targetObject.GetIterator();

            field.NextVisible(true);

            if (SHOW_SCRIPT_FIELD)
                totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            while (field.NextVisible(false))
                totalHeight += EditorGUI.GetPropertyHeight(field, true) + EditorGUIUtility.standardVerticalSpacing;

            totalHeight += INNER_SPACING * 2;
            totalHeight += OUTER_SPACING * 2;

            return totalHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var fieldRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight
            };

            EditorGUI.PropertyField(fieldRect, property, label, true);

            if (property.objectReferenceValue == null) return;

            property.isExpanded = EditorGUI.Foldout(
                fieldRect,
                property.isExpanded,
                GUIContent.none,
                true
            );

            if (!property.isExpanded)
                return;

            var targetObject = new SerializedObject(property.objectReferenceValue);

            #region Format Field Rects

            var propertyRects = new List<Rect>();
            var marchingRect = new Rect(fieldRect);

            var bodyRect = new Rect(fieldRect);
            bodyRect.xMin += EditorGUI.indentLevel * 14;
            bodyRect.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing
                                                               + OUTER_SPACING;

            var field = targetObject.GetIterator();
            field.NextVisible(true);

            marchingRect.y += INNER_SPACING + OUTER_SPACING;

            if (SHOW_SCRIPT_FIELD)
            {
                propertyRects.Add(marchingRect);
                marchingRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            while (field.NextVisible(false))
            {
                marchingRect.y += marchingRect.height + EditorGUIUtility.standardVerticalSpacing;
                marchingRect.height = EditorGUI.GetPropertyHeight(field, true);
                propertyRects.Add(marchingRect);
            }

            marchingRect.y += INNER_SPACING;

            bodyRect.yMax = marchingRect.yMax;

            #endregion

            var expandableAttribute = (ExpandableAttribute) attribute;
            if (expandableAttribute != null) BACKGROUND_STYLE = expandableAttribute.BackgroundStyle;

            DrawBackground(bodyRect);

            #region Draw Fields

            EditorGUI.indentLevel++;

            var index = 0;
            field = targetObject.GetIterator();
            field.NextVisible(true);

            if (SHOW_SCRIPT_FIELD)
            {
                //Show the disabled script field
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.PropertyField(propertyRects[index], field, true);
                EditorGUI.EndDisabledGroup();
                index++;
            }

            while (field.NextVisible(false))
            {
                try
                {
                    EditorGUI.PropertyField(propertyRects[index], field, true);
                }
                catch (StackOverflowException)
                {
                    field.objectReferenceValue = null;
                    Debug.LogError("Detected self-nesting cauisng a StackOverflowException, avoid using the same " +
                                   "object iside a nested structure.");
                }

                index++;
            }

            targetObject.ApplyModifiedProperties();

            EditorGUI.indentLevel--;

            #endregion
        }

        /// <summary>
        ///     Draws the Background
        /// </summary>
        /// <param name="rect">The Rect where the background is drawn.</param>
        private void DrawBackground(Rect rect)
        {
            switch (BACKGROUND_STYLE)
            {
                case BackgroundStyles.HelpBox:
                    EditorGUI.HelpBox(rect, "", MessageType.None);
                    break;

                case BackgroundStyles.Darken:
                    EditorGUI.DrawRect(rect, DARKEN_COLOUR);
                    break;

                case BackgroundStyles.Lighten:
                    EditorGUI.DrawRect(rect, LIGHTEN_COLOUR);
                    break;
            }
        }
        // Use the following area to change the style of the expandable ScriptableObject drawers;

        #region Style Setup

        /// <summary>
        ///     Whether the default editor Script field should be shown.
        /// </summary>
        private static readonly bool SHOW_SCRIPT_FIELD = false;

        /// <summary>
        ///     The spacing on the inside of the background rect.
        /// </summary>
        private static readonly float INNER_SPACING = 6.0f;

        /// <summary>
        ///     The spacing on the outside of the background rect.
        /// </summary>
        private static readonly float OUTER_SPACING = 4.0f;

        /// <summary>
        ///     The style the background uses.
        /// </summary>
        private static BackgroundStyles BACKGROUND_STYLE = BackgroundStyles.HelpBox;

        /// <summary>
        ///     The colour that is used to darken the background.
        /// </summary>
        private static readonly Color DARKEN_COLOUR = new Color(0.0f, 0.0f, 0.0f, 0.2f);

        /// <summary>
        ///     The colour that is used to lighten the background.
        /// </summary>
        private static readonly Color LIGHTEN_COLOUR = new Color(1.0f, 1.0f, 1.0f, 0.2f);

        #endregion
    }
#endif
}