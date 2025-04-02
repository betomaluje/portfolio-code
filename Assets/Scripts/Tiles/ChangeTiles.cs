using Dungeon;
using Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tiles {
    public class ChangeTiles : MonoBehaviour {
        [SerializeField]
        private Tilemap _tilemapToChange;

        [SerializeField]
        private DungeonConfig _config;

#if UNITY_EDITOR
        [Button]
        public void ClickChangeTiles() {
            if (_tilemapToChange == null) return;

            _tilemapToChange.ChangeTileBase(_config.RuleTile);
        }
#endif
    }
}