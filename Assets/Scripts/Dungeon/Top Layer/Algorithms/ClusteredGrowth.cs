using System;
using System.Collections.Generic;
using Extensions;
using UnityEngine;

namespace Dungeon {
    public class ClusteredGrowth : IFillChunk {

        private readonly System.Random _random;
        private readonly Room _room;
        private readonly float _growthProbability;
        private readonly int _maxChunks;

        public ClusteredGrowth(Room room, float chance, int maxChunks = 100) : this(room, chance, (int)DateTime.Now.Ticks, maxChunks) { }

        public ClusteredGrowth(Room room, float chance, int seed, int maxChunks = 100) {
            _room = room;
            _random = new System.Random(seed + room.GetHashCode());
            _growthProbability = chance;
            _maxChunks = maxChunks;
        }

        public List<Vector3Int> GenerateChunks() {
            List<Vector3Int> chunks = new();
            Queue<Vector3Int> frontier = new();
            Vector3Int start = _room.Center.ToInt3();

            chunks.Add(start);
            frontier.Enqueue(start);

            Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

            while (chunks.Count < _maxChunks && frontier.Count > 0) {
                Vector3Int current = frontier.Dequeue();

                foreach (var dir in directions) {
                    Vector3Int newPos = current + dir;

                    // Adjust the growth probability slightly per position for variation
                    float adjustedGrowthProbability = _growthProbability * (float)(_random.NextDouble() * 0.5 + 0.75);

                    // Only add new positions based on probability and if not already present
                    if (!chunks.Contains(newPos) && _random.NextDouble() < adjustedGrowthProbability) {
                        chunks.Add(newPos);
                        frontier.Enqueue(newPos);
                    }
                }
            }

            return chunks;
        }

        public void Dispose() { }
    }
}