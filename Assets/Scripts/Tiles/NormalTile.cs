using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tiles {
    [CreateAssetMenu(menuName = "Aurora/Tiles/Base Tile")]
    public class NormalTile : TileBase {
        [SerializeField]
        private Sprite _sprite;

        [SerializeField]
        private Tile.ColliderType _colliderType = Tile.ColliderType.Grid;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            base.GetTileData(position, tilemap, ref tileData);
            if (_sprite != null) {
                tileData.sprite = _sprite;
                tileData.colliderType = _colliderType;
                tileData.flags = TileFlags.LockColor;
            }
        }
    }
}