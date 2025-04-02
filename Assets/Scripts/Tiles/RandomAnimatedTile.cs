using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tiles {
    [CreateAssetMenu(menuName = "Aurora/Tiles/Random Animated Tile")]
    public class RandomAnimatedTile : AnimatedTile {
        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Percentage of animated tiles. The bigger, more animated and vice versa")]
        private float _animatedThreshold = .5f;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            if (_animatedThreshold <= Random.value) {
                m_TileAnimationFlags = TileAnimationFlags.PauseAnimation;
            }
            else {
                m_TileAnimationFlags = TileAnimationFlags.None;
            }

            base.GetTileData(position, tilemap, ref tileData);
        }
    }
}