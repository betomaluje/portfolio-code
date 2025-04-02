using System.Collections.Generic;
using UnityEngine;

namespace EditorTool.Models {
    [System.Serializable]
    public class GridSaveData {
        public string Name;
        public int X;
        public int Y;
        public bool OnlyCopy = false;

        public GridSaveData(Tool tool, Vector3Int position) {
            Name = tool.name;
            X = position.x;
            Y = position.y;
            OnlyCopy = tool.OnlyCopy;
        }

        public static string ToJson(Dictionary<Vector3Int, Tool> placedObjects) {
            var listToSave = new List<GridSaveData>(placedObjects.Count);
            foreach (var (position, tool) in placedObjects) {
                listToSave.Add(new GridSaveData(tool, position));
            }

            var savedObjects = JsonUtility.ToJson(listToSave.ToSerializable());
            return savedObjects;
        }

        public static List<GridSaveData> FromJson(string json) {
            if (string.IsNullOrEmpty(json)) {
                json = "{\"list\":[]}";
            }

            var loadedObjects = JsonUtility.FromJson<SerializableList<GridSaveData>>(json);
            return loadedObjects.list;
        }
    }
}