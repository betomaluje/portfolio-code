using System.IO;
using UnityEditor;
using UnityEngine;

public static class SelectionExtensions {
    public static string GetFolder() {
        Object[] selectedObjects = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);

        if ((selectedObjects?.Length ?? 0) == 0) {
            string folderPath = AssetDatabase.GetAssetPath(selectedObjects[0]);
            if (AssetDatabase.IsValidFolder(folderPath)) {
                return folderPath;
            }
            if (File.Exists(folderPath)) {
                return Path.GetDirectoryName(folderPath);
            }
        }
        return "Assets";
    }
}