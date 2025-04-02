using UnityEditor;
using UnityEngine;

namespace BerserkPixel.Prata {
    [CustomEditor(typeof(Interaction))]
    public class InteractionEditor : Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            
            EditorGUILayout.Space(8);
            if (GUILayout.Button("Reset")) {
                var interaction = (Interaction) target;
                interaction.Reset();
            }

        }
    }
}