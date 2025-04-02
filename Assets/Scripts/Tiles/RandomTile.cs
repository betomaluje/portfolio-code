using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tiles {
    [CreateAssetMenuAttribute(menuName = "Aurora/Tiles/Random Tile")]
    public class RandomTile : TileBase {
        [SerializeField]
        private RandomTilesConfig _randomConfig;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            var selectedSprite = _randomConfig.GetRandomSprite();
            if (selectedSprite != null) {
                tileData.sprite = selectedSprite;
                tileData.flags = TileFlags.LockColor;
            }
            else {
                base.GetTileData(position, tilemap, ref tileData);
            }
        }
    }

    [Serializable]
    internal struct TileRandom {
        [PreviewField]
        public Sprite Sprite;

        [Min(0)]
        public int Weight;
    }
}