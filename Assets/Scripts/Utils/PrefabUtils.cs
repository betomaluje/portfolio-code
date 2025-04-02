#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class PrefabUtils {
    public static List<GameObject> GetAllPrefabs() {
        string[] foldersToSearch = { "Assets" };
        return GetAllPrefabs(foldersToSearch);
    }

    public static List<GameObject> GetAllPrefabs(string[] foldersToSearch) {
        return GetAssets<GameObject>(foldersToSearch, "t:prefab");
    }

    public static List<T> GetAssets<T>(string[] foldersToSearch, string filter) where T : Object {
        string[] guids = AssetDatabase.FindAssets(filter, foldersToSearch);
        List<T> a = new();
        for (int i = 0; i < guids.Length; i++) {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a.Add(AssetDatabase.LoadAssetAtPath<T>(path));
        }
        return a;
    }
}

#endif