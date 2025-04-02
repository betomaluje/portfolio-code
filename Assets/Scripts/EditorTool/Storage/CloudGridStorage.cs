using System;
using System.Collections.Generic;
using DebugTools;
using EditorTool.Models;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using UnityEngine;

namespace EditorTool.Storage {
    public class CloudGridStorage : MonoBehaviour, IGridStorage {
        private const string Player_Data_Key = "player-map";

        private BattleGrid _grid;
        private RaidCloudStorage _raidCloudStorage;

        private void Awake() {
            _grid = BattleGrid.Instance;
            _raidCloudStorage = new RaidCloudStorage();
        }

        public void SaveGrid(string fileName = null) {
            // send to unity cloud service
            SendToUnity(_grid.PlacedTools);
        }

        private async void SendToUnity(Dictionary<Vector3Int, Tool> placedObjects) {
            string json;
            if (placedObjects == null || placedObjects.Count == 0) {
                json = "{\"list\":[]}";
            }
            else {
                json = GridSaveData.ToJson(placedObjects);
            }

            // more info look at https://docs.unity.com/ugs/en-us/manual/cloud-save/manual/tutorials/unity-sdk-sample

            var oneElement = new Dictionary<string, object>
            {
                { Player_Data_Key, json }
            };

            var result = await CloudSaveService.Instance.Data.Player.SaveAsync(oneElement);

            var playerId = AuthenticationService.Instance.PlayerId;
            await _raidCloudStorage.SaveRaid(playerId, json);

            // string newWriteLock = result[Player_Data_Key];

            // DebugLog.Log($"Successfully saved {Player_Data_Key}:{string.Join(',', placedObjects)} with updated write lock {newWriteLock}");
            DebugLog.Log("Grid saved");
        }

        /// <summary>
        /// Loads the grid from a player's file.
        /// </summary>
        /// <param name="callback">Callback when the grid is done loading</param>
        /// <param name="contentId">The player's id to use</param>
        public async void LoadPlayerGrid(Action<List<GridSaveData>> callback = null, string contentId = null) {
            await _raidCloudStorage.LoadRaidAsync(contentId, callback);
        }

        public void LoadGrid(Action<List<GridSaveData>> callback = null, string fileName = null) {
            LoadFromUnity(callback);
        }

        private async void LoadFromUnity(Action<List<GridSaveData>> callback = null) {
            var results = await CloudSaveService.Instance.Data.Player.LoadAsync(
                    new HashSet<string> { Player_Data_Key }
                );

            if (results.TryGetValue(Player_Data_Key, out var item)) {
                var loadedJson = item.Value.GetAs<string>();
                callback?.Invoke(GridSaveData.FromJson(loadedJson));
            }
        }

        public void DeleteGrid(string fileName = null) {
            DeleteFromUnity();
        }

        private async void DeleteFromUnity() {
            var options = new Unity.Services.CloudSave.Models.Data.Player.DeleteOptions();
            await CloudSaveService.Instance.Data.Player.DeleteAsync(Player_Data_Key, options);

            DebugLog.Log($"Successfully deleted {Player_Data_Key}");
        }
    }
}