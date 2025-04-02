using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Extensions {
    public static class TilemapExt {
        public static Vector3Int GetRandomPosition(this Tilemap tilemap) {
            return GetRandomPositions(tilemap, 1).First();
        }

        public static Vector3Int GetRandomPosition(this Tilemap tilemap, IList<Vector3Int> tileWorldLocations) {
            if (tileWorldLocations == null || tileWorldLocations.Count == 0) {
                tileWorldLocations = tilemap.GetAllPositions();
            }
            return GetRandomPositions(tilemap, tileWorldLocations, 1).First();
        }

        public static List<Vector3Int> GetRandomPositions(this Tilemap tilemap, int amount) {
            var tileWorldLocations = tilemap.GetAllPositions();

            return tileWorldLocations.Take(amount).ToList();
        }

        public static List<Vector3Int> GetRandomPositions(this Tilemap tilemap, IList<Vector3Int> tileWorldLocations, int amount) {
            if (tileWorldLocations == null || tileWorldLocations.Count == 0) {
                tileWorldLocations = tilemap.GetAllPositions();
            }
            return tileWorldLocations.Take(amount).ToList();
        }

        public static IList<Vector3Int> GetAllPositions(this Tilemap tilemap) {
            var tileWorldLocations = new List<Vector3Int>();
            // we need to loop through the tile map to get the positions where there is a tile
            foreach (var pos in tilemap.cellBounds.allPositionsWithin) {
                if (tilemap.HasTile(pos)) {
                    tileWorldLocations.Add(pos);
                }
            }

            return tileWorldLocations.Shuffle();
        }

        public static void ChangeTileBase(this Tilemap tilemap, TileBase newTiles) {
            var tileWorldLocations = new List<Vector3Int>();
            // we need to loop through the tile map to get the positions where there is a tile
            foreach (var pos in tilemap.cellBounds.allPositionsWithin) {
                if (tilemap.HasTile(pos)) {
                    tileWorldLocations.Add(pos);
                }
            }
            var tiles = new TileBase[tileWorldLocations.Count];
            Array.Fill(tiles, newTiles);
            tilemap.SetTiles(tileWorldLocations.ToArray(), tiles);
        }
    }
}