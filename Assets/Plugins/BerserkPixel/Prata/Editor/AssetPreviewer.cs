using UnityEditor;
using UnityEditor.Callbacks;

namespace BerserkPixel.Prata
{
    public static class AssetPreviewer
    {
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            UnityEngine.Object obj = EditorUtility.InstanceIDToObject(instanceId);

            if (obj is DialogContainer dialogue)
            {
                OpenDialogue(dialogue);
                return true;
            }

            return false;
        }

        private static void OpenDialogue(DialogContainer dialogue)
        {
            var window = PrataEditorWindow.ExternalOpen();
            window.SetLoadedField(dialogue);
        }
    }
}