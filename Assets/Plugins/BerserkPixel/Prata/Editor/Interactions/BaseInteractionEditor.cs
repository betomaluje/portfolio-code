using UnityEditor;
using UnityEngine;

namespace BerserkPixel.Prata {
    public abstract class BaseInteractionEditor<T> : Editor where T : IInteractionHolder {
        protected abstract string GetPropertyName();

        protected abstract string GetToolboxText();

        protected abstract string GetIconName();

        private SerializedProperty _serializedInteraction;

        private void OnEnable() {
            _serializedInteraction = serializedObject.FindProperty(GetPropertyName());
        }

        public override void OnInspectorGUI() {
			var helpBoxStyle = new GUIStyle(EditorStyles.helpBox) {
				stretchWidth = true
			};

			var content = new GUIContent(GetToolboxText());
            var texture = Resources.Load($"Icons/{GetIconName()}") as Texture2D;
            texture.hideFlags = HideFlags.None;
            content.image = texture;

            EditorGUILayout.LabelField(content, helpBoxStyle);

            EditorGUILayout.PropertyField(_serializedInteraction);

            serializedObject.ApplyModifiedProperties();
        }
    }
}