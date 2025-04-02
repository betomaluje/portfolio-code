using System;
using System.Collections.Generic;
using Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Dungeon {
    [Serializable]
    public enum FillType {
        FloodFill,
        Clustered
    }

    public class TopLayerGenerator : MonoBehaviour, IDungeonCallback {
        [SerializeField]
        private Tilemap _topLayer;

        [SerializeField]
        private FillType _fillType = FillType.FloodFill;

        [SerializeField]
        private DungeonConfig _config;

        [SerializeField]
        [Tooltip("Number of seeds to use for the initial flood-fill growth")]
        [Min(1)]
        private int _seedCount = 10;

        [SerializeField]
        [Tooltip("Number of growth steps to perform for each seed")]
        [Min(1)]
        private int _growthSteps = 5;

        [SerializeField]
        [Tooltip("Chance to grow into neighboring tile")]
        [Range(0f, 1f)]
        private float _growChance = 0.3f;

        [SerializeField]
        [Tooltip("Seed to use for the initial flood-fill growth. Will be converted to int using HashCode.")]
        private string _seed;

        private IList<Room> _rooms;

        public void OnMapStarts() { }

        public void OnAllRoomsGenerated(ref IList<Room> rooms, ref IList<Transform> roomTransforms) { }

        public void OnRoomsSelected(ref IList<Room> selectedRooms) { }

        public void OnRoomsToCenter(ref IList<Room> rooms, ref IList<Vector3Int> positions) => _rooms = rooms;

        [Button("Generate Top Layer")]
        public void OnMapLoaded() => StartGeneration(_rooms);

        private void StartGeneration(IList<Room> rooms) {
            if (_topLayer == null || _config == null || _rooms == null || _rooms.Count == 0) {
                return;
            }

            _topLayer.color = _config.MapTint;

            ClearTopLayer();

            List<Vector3Int> _topLayerPositions = new();

            foreach (var room in rooms) {
                IFillChunk fillChunk = CreateFillChunk(room);

                _topLayerPositions.AddRange(fillChunk.GenerateChunks());

                fillChunk.Dispose();
            }

            var tiles = new TileBase[_topLayerPositions.Count];
            Array.Fill(tiles, _config.RuleTile);
            _topLayer.SetTiles(_topLayerPositions.ToArray(), tiles);

            if (_topLayer.gameObject.FindInChildren(out TilemapBackground background)) {
                background.UpdateBackground();
                GenerateColliders(_topLayer.gameObject);
            }
        }

        private IFillChunk CreateFillChunk(Room room) {
            switch (_fillType) {
                case FillType.Clustered:
                    if (string.IsNullOrEmpty(_seed)) {
                        return new ClusteredGrowth(room, _growChance, _seedCount);
                    }
                    else {
                        return new ClusteredGrowth(room, _growChance, _seed.GetHashCode(), _seedCount);
                    }
                default:
                case FillType.FloodFill:
                    if (string.IsNullOrEmpty(_seed)) {
                        return new FloodFillGrowth(room, _growChance, _seedCount, _growthSteps);
                    }
                    else {
                        return new FloodFillGrowth(room, _growChance, _seedCount, _growthSteps, _seed.GetHashCode());
                    }
            }
        }

        private void ClearTopLayer() {
            _topLayer.ClearAllTiles();
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