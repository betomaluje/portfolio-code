using System.IO;
using UnityEditor;
using UnityEngine;

namespace BerserkPixel.Utils {
    public static class FileExt {
#if UNITY_EDITOR
        public static void SaveGameObject(string folderPath, string fileName, GameObject go) {
            if (go) {
                // check if folder exists first
                if (!Directory.Exists(folderPath))
                    // we create the folder
                {
                    Directory.CreateDirectory(folderPath);
                }

                // we append
                var savePath = $"{folderPath}/{fileName}";

                if (PrefabUtility.SaveAsPrefabAsset(go, savePath)) {
                    EditorUtility.DisplayDialog("Tilemap saved", "Your Tilemap was saved under" + savePath, "Continue");
                }
                else {
                    EditorUtility.DisplayDialog("Tilemap NOT saved",
                        "An ERROR occured while trying to saveTilemap under" + savePath, "Continue");
                }
            }
        }
#endif
    }
}