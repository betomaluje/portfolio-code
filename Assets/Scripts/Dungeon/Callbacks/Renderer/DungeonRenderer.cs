using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Dungeon.Renderer {
    public class DungeonRenderer : DungeonCallback {
        private readonly Tilemap _tilemap;

        private readonly DungeonConfig _config;

        public DungeonRenderer(Tilemap tilemap, DungeonConfig config) {
            _tilemap = tilemap;
            _config = config;
        }

        public override void OnMapStarts() {
            _tilemap.ClearAllTiles();
        }

        public override void OnRoomsToCenter(ref IList<Room> rooms, ref IList<Vector3Int> positions) {
            _tilemap.color = _config.MapTint;

            var tiles = new TileBase[positions.Count];
            Array.Fill(tiles, _config.RuleTile);
            _tilemap.SetTiles(positions.ToArray(), tiles);
        }
    }
}