using Cysharp.Threading.Tasks;
using UnityEngine;
using Unity.Services.CloudSave;
using System.Collections.Generic;
using System;
using System.Linq;
using EditorTool.Models;

namespace EditorTool.Storage {
    public class CloudPlayerTools : MonoBehaviour {
        private const string Player_Data_Key = "player-tools";

        public async UniTask<ToolsPerPlayer> GetPlayerTools(HashSet<Tool> tools) {
            var results = await CloudSaveService.Instance.Data.Player.LoadAsync(
                    new HashSet<string> { Player_Data_Key }
                );

            if (results.TryGetValue(Player_Data_Key, out var item)) {
                var loadedJson = item.Value.GetAs<string>();
                if (string.IsNullOrEmpty(loadedJson)) {
                    return null;
                }

                try {
                    var toolSavedData = ToolSaveData.FromJson(loadedJson);
                    ToolsPerPlayer toolsPerPlayer = ScriptableObject.CreateInstance<ToolsPerPlayer>();
                    toolsPerPlayer.Tools = new Dictionary<Tool, int>();
                    foreach (var toolSaveData in toolSavedData) {
                        var tool = tools.Where(tool => tool.name == toolSaveData.Name).FirstOrDefault();
                        if (tool == null) {
                            Debug.LogError($"Failed to find tool {toolSaveData.Name}");
                            continue;
                        }

                        toolsPerPlayer.Tools[tool] = toolSaveData.Amount;
                    }

                    return toolsPerPlayer;
                }
                catch (ArgumentException e) {
                    Debug.LogError($"Failed to parse json: {loadedJson}. {e.Message}");
                    return null;
                }
            }

            return null;
        }

        public async void SavePlayerTools(ToolsPerPlayer toolsPerPlayer) {
            var json = ToolSaveData.ToJson(toolsPerPlayer);

            var oneElement = new Dictionary<string, object>
            {
                { Player_Data_Key, json }
            };

            var result = await CloudSaveService.Instance.Data.Player.SaveAsync(oneElement);
        }
    }
}