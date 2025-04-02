using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tiles {
    [CreateAssetMenu(menuName = "Aurora/Tiles/GameObject Tile")]
    public class GameObjectTile : TileBase {
        [SerializeField]
        private GameObject _prefab;

        [SerializeField]
        private Tile.ColliderType _colliderType = Tile.ColliderType.Grid;

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go) {
            if (_prefab == null) {
                return false;
            }

            var parent = tilemap.GetComponent<Tilemap>().transform;

            if (parent == null) {
                return false;
            }

            go = Instantiate(_prefab, position, Quaternion.identity, parent);

            return base.StartUp(position, tilemap, go);
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            base.GetTileData(position, tilemap, ref tileData);
            if (_prefab != null) {
                tileData.colliderType = _colliderType;
                tileData.flags = TileFlags.LockColor;
            }
        }
    }
}