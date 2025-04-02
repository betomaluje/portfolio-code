using BerserkPixel.Prata.Utilities;
using UnityEditor;
using UnityEngine;

namespace BerserkPixel.Prata
{
    public class PrataEditor
    {
        [MenuItem(PrataConstants.MenuRoot + "/Setup Scene")]
        public static void Setup()
        {
            var prevDialogManager = Object.FindFirstObjectByType<DialogManager>();
            if (prevDialogManager == null)
            {
                GameObject g = new GameObject("Dialog Manager");
                g.AddComponent<DialogManager>();
            }

            GraphSaveUtilities.GenerateFolders();
            GraphSaveUtilities.CreateFirstCharacter("Player");
        }

        [MenuItem(PrataConstants.MenuRoot + "/Create/New Character")]
        public static void CreateCharacter()
        {
            var character = ScriptableObject.CreateInstance<Character>();

            GraphSaveUtilities.GenerateFolders();

            AssetDatabase.CreateAsset(character, $"{PrataConstants.FOLDER_CHARACTERS_COMPLETE}/New Character.asset");
            EditorUtility.SetDirty(character);
            AssetDatabase.SaveAssets();

            Selection.activeObject = character;
            SceneView.FrameLastActiveSceneView();
        }

        [MenuItem(PrataConstants.MenuRoot + "/Create/New Interaction")]
        public static void CreateInteraction()
        {
            var interaction = ScriptableObject.CreateInstance<Interaction>();

            GraphSaveUtilities.GenerateFolders();

            AssetDatabase.CreateAsset(interaction,
                $"{PrataConstants.FOLDER_INTERACTIONS_COMPLETE}/New Interaction.asset");
            EditorUtility.SetDirty(interaction);
            AssetDatabase.SaveAssets();

            Selection.activeObject = interaction;
            SceneView.FrameLastActiveSceneView();
        }
    }
}