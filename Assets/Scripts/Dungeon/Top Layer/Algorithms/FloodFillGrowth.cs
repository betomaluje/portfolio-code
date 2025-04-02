using System;
using System.Collections.Generic;
using Extensions;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Dungeon {
    public class FloodFillGrowth : IFillChunk {
        private readonly int _seedCount = 10;

        private readonly int _growthSteps = 5;

        private readonly float _growChance = 0.3f;

        private readonly int _width = 100;

        private readonly int _height = 100;

        private Vector3Int _center;

        private NativeArray<Vector3Int> seeds;
        private NativeArray<bool> tilemapFlags;
        private System.Random _random;

        public FloodFillGrowth(Room room, float growChance, int seedCount, int growthSteps) : this(room, growChance, seedCount, growthSteps, (int)DateTime.Now.Ticks) { }

        public FloodFillGrowth(Room room, float growChance, int seedCount, int growthSteps, int seed) {
            _width = room.Width;
            _height = room.Height;
            _growChance = growChance;
            _seedCount = seedCount;
            _growthSteps = growthSteps;

            _random = new System.Random(seed + room.GetHashCode());

            if (_height < _width) {
                _width = _height;
            }

            _center = room.Center.ToInt3();
        }

        public List<Vector3Int> GenerateChunks() {
            // Initialize seed positions and start the generation process
            tilemapFlags = new NativeArray<bool>(_width * _height, Allocator.TempJob);

            // Step 1: Initialize seeds randomly
            seeds = InitializeSeedsAroundCenter(_center, math.min(_width, _height) / 2);

            // Step 2: Perform flood-fill growth in multiple steps
            PerformFloodFill();

            List<Vector3Int> _topLayerPositions = new();

            for (int i = 0; i < tilemapFlags.Length; i++) {
                if (tilemapFlags[i]) {
                    int x = i % _width;
                    int y = i / _height;

                    Vector3Int position = _center + new Vector3Int(x, y, 0) - new Vector3Int(_width / 2, _height / 2, 0);
                    _topLayerPositions.Add(position);
                }
            }

            // Dispose of the native arrays to free memory
            seeds.Dispose();
            tilemapFlags.Dispose();

            return _topLayerPositions;
        }

        private NativeArray<Vector3Int> InitializeSeedsAroundCenter(Vector3Int center, int radius) {
            NativeArray<Vector3Int> seeds = new(_seedCount, Allocator.TempJob);

            for (int i = 0; i < _seedCount; i++) {
                // Generate a random point within the radius around the center
                int xOffset = _random.Next(-radius, radius + 1);
                int yOffset = _random.Next(-radius, radius + 1);
                Vector3Int seedPosition = new(center.x + xOffset, center.y + yOffset, 0);

                // Ensure the seed position is within bounds
                seedPosition.x = math.clamp(seedPosition.x, 0, _width - 1);
                seedPosition.y = math.clamp(seedPosition.y, 0, _height - 1);

                seeds[i] = seedPosition;
            }
            return seeds;
        }

        private void PerformFloodFill() {
            for (int step = 0; step < _growthSteps; step++) {
                GrowthJob growthJob = new() {
                    seeds = seeds,
                    tilemapFlags = tilemapFlags,
                    width = _width,
                    height = _height,
                    growChance = _growChance,
                    randomSeed = (uint)_random.Next() // Seed for random number generation
                };

                JobHandle growthHandle = growthJob.Schedule();
                growthHandle.Complete();

                // Update seeds with new growth positions for the next pass
                seeds = UpdateSeedsForNextPass(tilemapFlags);
            }
        }

        private NativeArray<Vector3Int> UpdateSeedsForNextPass(NativeArray<bool> tilemapFlags) {
            var newSeeds = new NativeList<Vector3Int>(Allocator.TempJob);

            for (int y = 0; y < _height; y++) {
                for (int x = 0; x < _width; x++) {
                    int index = x + y * _width;
                    if (tilemapFlags[index]) {
                        newSeeds.Add(new Vector3Int(x, y, 0));
                    }
                }
            }
            return newSeeds.AsArray();
        }

        public void Dispose() {
            if (seeds.IsCreated) seeds.Dispose();
            if (tilemapFlags.IsCreated) tilemapFlags.Dispose();
        }
    }

    [BurstCompile]
    public struct GrowthJob : IJob {
        [ReadOnly] public NativeArray<Vector3Int> seeds;
        public NativeArray<bool> tilemapFlags;
        public int width;
        public int height;
        public float growChance;
        public uint randomSeed;

        private static readonly Vector3Int[] directions = {
                new(1, 0, 0),
                new(-1, 0, 0),
                new(0, 1, 0),
                new(0, -1, 0)
            };

        public void Execute() {
            var random = new Unity.Mathematics.Random(randomSeed);

            for (int index = 0; index < seeds.Length; index++) {
                Vector3Int seed = seeds[index];
                int seedIndex = seed.x + seed.y * width;
                if (seedIndex >= 0 && seedIndex < tilemapFlags.Length)
                    tilemapFlags[seedIndex] = true;

                foreach (var direction in directions) {
                    Vector3Int newPos = seed + direction;
                    int newIndex = newPos.x + newPos.y * width;

                    if (newPos.x >= 0 && newPos.x < width &&
                        newPos.y >= 0 && newPos.y < height &&
                        newIndex >= 0 && newIndex < tilemapFlags.Length &&
                        !tilemapFlags[newIndex] && random.NextFloat() < growChance) {
                        tilemapFlags[newIndex] = true;
                    }
                }
            }
        }
    }
}