using EditorTool;
using UnityEngine;

namespace Raid {
    public class RaidInitializer : MonoBehaviour {
        [SerializeField]
        private PersistentRaid _raid;

        private void Start() {
            BattleGrid.Instance.LoadOtherPlayerGrid(_raid.ContentId);
        }
    }
}