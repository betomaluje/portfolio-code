using System;
using System.Text;
using UnityEngine;

namespace Dungeon.Renderer {
    [Serializable]
    public class ChunkSettings {
        public int chunkCount = 4;
        public float noiseScale = 0.3f;

        // Higher threshold means fewer chunks, more sparsely placed
        public float threshold = 0.5f;
        public float renderChance = 0.5f;
        public Vector2Int padding = new(5, 5);

        public static ChunkSettings GetRandom() {
            return new ChunkSettings {
                chunkCount = UnityEngine.Random.Range(1, 5),
                noiseScale = UnityEngine.Random.Range(0.1f, 0.5f),
                threshold = UnityEngine.Random.Range(0.1f, 0.5f),
                renderChance = UnityEngine.Random.Range(0.5f, 1f),
                padding = new(UnityEngine.Random.Range(1, 8), UnityEngine.Random.Range(0, 5))
            };
        }

        public override string ToString() {
            StringBuilder sb = new();
            sb.AppendLine("ChunkSettings:");
            sb.AppendLine($"ChunkCount: {chunkCount}");
            sb.AppendLine($"NoiseScale: {noiseScale}");
            sb.AppendLine($"Threshold: {threshold}");
            sb.AppendLine($"RenderChance: {renderChance}");
            sb.AppendLine($"Padding: {padding}");
            return sb.ToString();
        }
    }
}