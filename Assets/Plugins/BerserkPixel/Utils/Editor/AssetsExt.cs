using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BerserkPixel.Extensions.Editor {
    public static class AssetsExt {
#if UNITY_EDITOR
        
        public static List<T> FindAllScriptableObjectsOfType<T>(string filter, string folder = "Assets")
            where T : ScriptableObject {
            return AssetDatabase.FindAssets(filter, new[] {folder})
                .Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToList();
        }
        
        public static T FindObjectOfType<T>(string filter, string folder = "Assets")
            where T : Object {
            return AssetDatabase.FindAssets(filter, new[] {folder})
                .Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)))
                .FirstOrDefault();
        }
        
        public static List<T> FindAllObjectsOfType<T>(string filter, string folder = "Assets")
            where T : Object {
            return AssetDatabase.FindAssets(filter, new[] {folder})
                .Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToList();
        }
        
#endif
    }
}