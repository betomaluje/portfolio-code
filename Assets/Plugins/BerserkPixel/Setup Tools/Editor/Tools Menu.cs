using UnityEditor;
using static System.IO.Directory;
using static System.IO.Path;
using static UnityEngine.Application;
using static UnityEditor.AssetDatabase;

namespace BerserkTools
{
    public static class ToolsMenu
    {
        [MenuItem("Tools/Setup/Change Company Name")]
        public static void SetupCompanyName()
        {
            PlayerSettings.companyName = "Berserk Pixel Studios";
        }
    
        [MenuItem("Tools/Setup/Create Default Folders")]
        public static void CreateDefaultFolders()
        {
            Dir("", "Scripts", "Scenes", "Art", "Animations", "Prefabs", "Sound", "Materials", "Plugins");
            Dir("Art", "Sprites", "Icons");
            Dir("Sound", "Music", "SFX");
            Dir("Plugins", "BerserkPixel");
            Refresh();
        }

        [MenuItem("Tools/Setup/Everything")]
        public static void SetupEverything()
        {
            SetupCompanyName();
            CreateDefaultFolders();
        }

        private static void Dir(string root, params string[] dir)
        {
            var fullPath = Combine(dataPath, root);
            foreach (var newDirectory in dir)
            {
                CreateDirectory(Combine(fullPath, newDirectory));
            }
        }
    }
}