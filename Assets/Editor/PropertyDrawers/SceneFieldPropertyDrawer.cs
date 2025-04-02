#if UNITY_EDITOR
using Scene_Management;
using UnityEditor;
using UnityEngine;

namespace Editor.PropertyDrawers {
    [CustomPropertyDrawer(typeof(SceneField))]
    public class SceneFieldPropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            var sceneAsset = property.FindPropertyRelative("m_SceneAsset");
            var sceneName = property.FindPropertyRelative("m_SceneName");
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            if (sceneAsset != null) {
                sceneAsset.objectReferenceValue =
                    EditorGUI.ObjectField(position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);

                if (sceneAsset.objectReferenceValue != null) {
                    sceneName.stringValue = ((SceneAsset) sceneAsset.objectReferenceValue).name;
                }
            }

            EditorGUI.EndProperty();
        }
    }
}
#endif