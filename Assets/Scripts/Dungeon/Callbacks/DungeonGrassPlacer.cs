using System;
using System.Collections.Generic;
using System.Linq;
using BerserkPixel.Tilemap_Generator.Extensions;
using Sirenix.OdinInspector;
using Tiles;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Dungeon {
    public class DungeonGrassPlacer : MonoBehaviour, IDungeonCallback {
        [SerializeField]
        private GameObjectTile _grassTile;

        [SerializeField]
        private Tilemap _mainTilemap;

        [SerializeField]
        private Tilemap[] _allGroundTilemaps;

        [BoxGroup("Chunks")]
        [SerializeField]
        [Range(0f, 1f)]
        private float _threshold = .8f;

        [BoxGroup("Chunks")]
        [SerializeField]
        private int _chunksPerRoom = 5;

        [BoxGroup("Chunks")]
        [SerializeField]
        [Range(0, 10)]
        private int _width = 8;

        [BoxGroup("Chunks")]
        [SerializeField]
        [Range(0, 10)]
        private int _height = 5;

        public void OnMapStarts() { }

        public void OnAllRoomsGenerated(ref IList<Room> rooms, ref IList<Transform> roomTransforms) { }

        public void OnRoomsSelected(ref IList<Room> selectedRooms) { }

        public void OnRoomsToCenter(ref IList<Room> rooms, ref IList<Vector3Int> positions) { }

        public void OnMapLoaded() {
            var allUsedTiles = _allGroundTilemaps.GetUsedTilesInAllTilemaps().SimpleShuffle();
            var totalTiles = allUsedTiles.Count;
            var bannedPositions = new HashSet<Vector3Int>();

            for (int i = 0; i < _chunksPerRoom; i++) {

                var randomPoint = allUsedTiles[UnityEngine.Random.Range(0, totalTiles)];

                for (int y = 0; y < _height; y++) {
                    for (int x = 0; x < _width; x++) {
                        // Generate Perlin noise value
                        float2 noiseCoord = new(randomPoint.x + x, randomPoint.y + y);
                        float noiseValue = noise.snoise(noiseCoord);

                        // Apply the threshold to determine if this position should be included in a chunk
                        if (noiseValue >= _threshold) {
                            continue;
                        }

                        var position = new Vector3Int((int)noiseCoord.x, (int)noiseCoord.y);

                        if (allUsedTiles.Contains(position) && !bannedPositions.Contains(position)) {
                            bannedPositions.Add(position);
                        }
                    }
                }

                var tiles = new TileBase[bannedPositions.Count];
                Array.Fill(tiles, _grassTile);
                _mainTilemap.SetTiles(bannedPositions.ToArray(), tiles);
            }
        }
    }
}