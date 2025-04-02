using Cysharp.Threading.Tasks;

namespace Storage.Player {
    public interface IPlayerStorage {
        void SavePlayer(ref PlayerMiniModel playerMiniModel);

        UniTask<PlayerMiniModel> LoadPlayer();
    }
}