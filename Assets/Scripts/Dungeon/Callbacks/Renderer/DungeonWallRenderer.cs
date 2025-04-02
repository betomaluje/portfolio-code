using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Dungeon.Renderer {
    public class DungeonWallRenderer : DungeonCallback {
        private readonly Tilemap _tilemap;

        private readonly DungeonConfig _config;

        public DungeonWallRenderer(Tilemap tilemap, DungeonConfig config) {
            _tilemap = tilemap;
            _config = config;
        }

        public override void OnMapStarts() {
            _tilemap.ClearAllTiles();
        }

        public override void OnRoomsToCenter(ref IList<Room> rooms, ref IList<Vector3Int> positions) {
            _tilemap.color = _config.MapTint;

            foreach (Room room in rooms) {
                var chunkSettings = ChunkSettings.GetRandom();
                room.ChunkSettings = chunkSettings;
                List<Vector3Int[]> chunks = GenerateChunks(room, chunkSettings);

                foreach (Vector3Int[] chunk in chunks) {
                    var chance = UnityEngine.Random.value;

                    if (chance <= chunkSettings.renderChance) {
                        var tiles = new TileBase[chunk.Length];
                        Array.Fill(tiles, _config.RuleTile);
                        _tilemap.SetTiles(chunk, tiles);
                    }
                }
            }

            if (_tilemap.gameObject.FindInChildren(out TilemapBackground background)) {
                background.UpdateBackground();
                GenerateColliders(_tilemap.gameObject);
            }
        }

        /// <summary>
        /// It uses Perlin noise to distribute positions within the room into chunks, based on the provided settings.
        /// The method first initializes an empty list of chunks, then iterates over all positions in the room.
        /// For each position, it calculates a Perlin noise value and checks if it's above a certain threshold. 
        /// If so, it adds the position to a chunk, using the noise value to determine which chunk to add it to.
        /// </summary>
        /// <param name="room">The room where this chunk belongs to</param>
        /// <param name="chunkSettings">The chunk settings to use. See <see cref="ChunkSettings"/></param>
        /// <returns>List of chunks, where each chunk is an array of Vector3Int positions.</returns>
        private List<Vector3Int[]> GenerateChunks(Room room, ChunkSettings chunkSettings) {

            if (chunkSettings.chunkCount <= 0) return new();

            List<List<Vector3Int>> chunks = new();

            for (int i = 0; i < chunkSettings.chunkCount; i++) {
                chunks.Add(new List<Vector3Int>());
            }

            var test = 0;
            var currentSign = RandomUtils.NextBool() ? 1 : -1;

            var paddingX = chunkSettings.padding.x * -currentSign;
            var paddingY = chunkSettings.padding.y * currentSign;

            foreach (Vector3Int pos in room.AllPositions) {
                // Normalize the position within the room
                float xCoord = (float)(pos.x - room.Center.x + room.Width / 2) / room.Width * chunkSettings.noiseScale;
                float yCoord = (float)(pos.y - room.Center.y + room.Height / 2) / room.Height * chunkSettings.noiseScale;

                // Generate Perlin noise value
                float2 noiseCoord = new(xCoord, yCoord);
                float noiseValue = noise.snoise(noiseCoord);

                // Apply the threshold to determine if this position should be included in a chunk
                if (noiseValue >= chunkSettings.threshold) {
                    // Quantize noise value to ensure even distribution among chunks
                    int chunkIndex = Mathf.FloorToInt(noiseValue * chunkSettings.chunkCount);

                    // Ensure chunkIndex is within bounds
                    chunkIndex = Mathf.Clamp(chunkIndex, 0, chunkSettings.chunkCount - 1);

                    var newPosition = pos;
                    newPosition.x += paddingX;
                    newPosition.y += paddingY;

                    // Add the position to the corresponding chunk
                    chunks[chunkIndex].Add(newPosition);

                    test++;
                }
            }

            return chunks.Select(innerList => innerList.ToArray()).ToList();
        }

        private void GenerateColliders(GameObject tilemap) {
            if (tilemap.TryGetComponent(out TilemapCollider2D currentCollider)) {
                currentCollider.ProcessTilemapChanges();
            }
            else {
                var tilemapCollider = tilemap.AddComponent<TilemapCollider2D>();
                var composite = tilemap.AddComponent<CompositeCollider2D>();

                composite.geometryType = CompositeCollider2D.GeometryType.Outlines;
                tilemapCollider.compositeOperation = Collider2D.CompositeOperation.Merge;
                composite.offset = new(0f, -.2f);
                composite.offsetDistance = .2f;

                var rb = tilemap.GetComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Static;
            }
        }
    }
}