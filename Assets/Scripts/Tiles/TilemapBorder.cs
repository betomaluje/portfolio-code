using System;
using System.Collections.Generic;
using System.Linq;
using Attributes;
using BerserkPixel.Tilemap_Generator.Extensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tiles {
    public class TilemapBorder : MonoBehaviour {
        [SerializeField]
        private bool _dynamicBorder = false;

        [SerializeField]
        private Tilemap _borderTilemap;

        [SerializeField]
        private Tilemap[] _substractTilemaps;

        [SerializeField]
        private TilemapCollider2D _tilemapCollider;

        [SerializeField]
        [InspectorButton(nameof(GenerateBorder), 4)]
        [Tooltip("Make sure to first create transparent tiles in the border tilemap")]
        private bool _buttonSubstract;

        [SerializeField]
        [InspectorButton(nameof(ClearBorder), 4)]
        private bool _clearMap;

        [Header("Debug")]
        [SerializeField]
        private bool _debug;

        [SerializeField]
        private TileBase _debugTile;

        private void Awake() {
            _tilemapCollider = _borderTilemap.GetComponent<TilemapCollider2D>();
            _tilemapCollider.enabled = !_dynamicBorder;
        }

        private void OnValidate() {
            if (_borderTilemap != null && _borderTilemap.TryGetComponent(out _tilemapCollider)) {
                if (_tilemapCollider.TryGetComponent<CompositeCollider2D>(out var composite)) {
                    composite.geometryType = CompositeCollider2D.GeometryType.Polygons;
                }
            }
        }

        public void GenerateBorder() {
            var mainPosition = _borderTilemap.GetWorldPositionsWithTiles();

            List<Vector3Int> toSubstract = _substractTilemaps.GetUsedTilesInAllTilemaps();

            ClearBorder();

            var a = mainPosition.Except(toSubstract);

            var vector3Ints = a as Vector3Int[] ?? a.ToArray();
            if (_debug && vector3Ints.Any()) {
                var tiles = new TileBase[vector3Ints.Length];
                Array.Fill(tiles, _debugTile);
                _borderTilemap.SetTiles(vector3Ints.ToArray(), tiles);
            }

            GenerateColliders();
        }

        private void ClearBorder() {
            _borderTilemap.ClearAllTiles();
        }

        private void GenerateColliders() {
            if (_borderTilemap.TryGetComponent(out TilemapCollider2D currentCollider)) {
                currentCollider.enabled = false;
                currentCollider.enabled = true;
                currentCollider.ProcessTilemapChanges();
            }
            else {
                var tilemapCollider = _borderTilemap.gameObject.AddComponent<TilemapCollider2D>();
                _borderTilemap.gameObject.AddComponent<CompositeCollider2D>();
                tilemapCollider.compositeOperation = Collider2D.CompositeOperation.Merge;

                var rb = _borderTilemap.gameObject.GetComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Static;
            }
        }
    }
}