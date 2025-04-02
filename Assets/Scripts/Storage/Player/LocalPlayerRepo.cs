using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Storage.Player {
    public class LocalPlayerRepo : IPlayerStorage {
        private const string PLAYER_KEY = "player_key";

        public void SavePlayer(ref PlayerMiniModel playerMiniModel) {
            var toJson = PlayerMiniModelExt.ToJson(playerMiniModel);

            PlayerPrefs.SetString(PLAYER_KEY, toJson);
        }

        public UniTask<PlayerMiniModel> LoadPlayer() {
            var jsonString = PlayerPrefs.GetString(PLAYER_KEY, null);

            if (string.IsNullOrEmpty(jsonString)) {
                var defaultPlayer = PlayerMiniModelExt.GetDefault();
                SavePlayer(ref defaultPlayer);
                return UniTask.FromResult(defaultPlayer);
            }

            var fromJson = PlayerMiniModelExt.FromJson(jsonString);

            return UniTask.FromResult(fromJson);
        }
    }
}