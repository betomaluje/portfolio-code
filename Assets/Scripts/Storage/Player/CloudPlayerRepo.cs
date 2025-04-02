using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Unity.Services.CloudSave;
using static Storage.Player.PlayerMiniModel;

namespace Storage.Player {
    public class CloudPlayerRepo : IPlayerStorage {
        private const string Player_Data_Key = "player-stats";

        public void SavePlayer(ref PlayerMiniModel playerMiniModel) {
            var json = PlayerMiniModelExt.ToJson(playerMiniModel);
            SendToUnity(json);
        }

        private async void SendToUnity(string json) {
            var oneElement = new Dictionary<string, object>
                        {
                { Player_Data_Key, json }
            };

            var result = await CloudSaveService.Instance.Data.Player.SaveAsync(oneElement);
        }

        public async UniTask<PlayerMiniModel> LoadPlayer() {
            var results = await CloudSaveService.Instance.Data.Player.LoadAsync(
                   new HashSet<string> { Player_Data_Key }
               );

            if (results.TryGetValue(Player_Data_Key, out var item)) {
                var loadedJson = item.Value.GetAs<string>();
                if (string.IsNullOrEmpty(loadedJson)) {
                    return PlayerMiniModelExt.GetDefault();
                }

                var playerMiniModel = PlayerMiniModelExt.FromJson(loadedJson);
                return PlayerMiniModelExt.Check(playerMiniModel);
            }

            return PlayerMiniModelExt.GetDefault();
        }
    }
}