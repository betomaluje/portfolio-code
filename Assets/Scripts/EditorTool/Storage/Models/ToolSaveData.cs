using System.Collections.Generic;
using UnityEngine;

namespace EditorTool.Models {

    [System.Serializable]
    public class ToolSaveData {
        public string Name;
        public int Amount;

        public ToolSaveData(string name, int amount) {
            Name = name;
            Amount = amount;
        }

        public static string ToJson(ToolsPerPlayer toolsPerPlayer) {
            var listToSave = new List<ToolSaveData>(toolsPerPlayer.Tools.Count);
            foreach (var (tool, amount) in toolsPerPlayer.Tools) {
                listToSave.Add(new ToolSaveData(tool.name, amount));
            }

            var savedObjects = JsonUtility.ToJson(listToSave.ToSerializable());
            return savedObjects;
        }

        public static List<ToolSaveData> FromJson(string json) {
            if (string.IsNullOrEmpty(json)) {
                json = "{\"list\":[]}";
            }

            var toolSavedData = JsonUtility.FromJson<SerializableList<ToolSaveData>>(json);
            return toolSavedData.list;
        }
    }
}