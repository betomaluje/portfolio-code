using Tiles;

namespace Dungeon {
    public class DungeonTilemaps : DungeonCallback {
        private readonly TilemapBackground[] _tilemapBackgrounds;

        private readonly TilemapBorder _tilemapBorder;

        public DungeonTilemaps(TilemapBackground[] tilemapBackground, TilemapBorder tilemapBorder) {
            if (tilemapBackground == null || tilemapBackground.Length == 0) {
                _tilemapBackgrounds = new TilemapBackground[] { };
            }
            else {
                _tilemapBackgrounds = tilemapBackground;
            }

            _tilemapBorder = tilemapBorder;
        }

        public override void OnMapStarts() {
            foreach (var tilemap in _tilemapBackgrounds) {
                tilemap.ClearMap();
            }
        }

        public override void OnMapLoaded() {
            _tilemapBorder.GenerateBorder();
            foreach (var tilemap in _tilemapBackgrounds) {
                tilemap.UpdateBackground();
            }
        }
    }
}