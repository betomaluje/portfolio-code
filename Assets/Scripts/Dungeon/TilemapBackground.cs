using System.Collections.Generic;
using Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Dungeon {
    [ExecuteAlways]
    [RequireComponent(typeof(Tilemap))]
    public class TilemapBackground : MonoBehaviour {
        [BoxGroup("Tilemap")]
        [SerializeField]
        private Tilemap ReferenceTilemap;

        [BoxGroup("Tilemap")]
        [SerializeField]
        private Vector2 _positionOffset = new(0, -0.5f);

        [BoxGroup("Tiles")]
        [SerializeField]
        private TileBase _ruleTile;

        [BoxGroup("Tiles")]
        [SerializeField]
        private Vector2Int _tileOffset = new(0, -1);

        [BoxGroup("Tiles")]
        [Optional]
        [SerializeField]
        private TileBase _extraBottomTile;

        private Tilemap _tilemap;

        private void Init() {
            if (_tilemap == null) {
                _tilemap = gameObject.GetComponent<Tilemap>();
            }
        }

        private void OnValidate() {
            if (this.FindInParent<Tilemap>(out var parentTilemap)) {
                ReferenceTilemap = parentTilemap;

                if (TryGetComponent<Tilemap>(out var tilemap)) {
                    SetSortingLayer(ReferenceTilemap, tilemap);
                }
            }
        }

        /// <summary>
        ///     This method will copy the reference tilemap into the one on this gameobject
        /// </summary>
        [Button]
        public void UpdateBackground() {
            if (ReferenceTilemap == null) {
                return;
            }

            Init();

            Copy(ReferenceTilemap, _tilemap);

            SetSortingLayer(ReferenceTilemap, _tilemap);

            transform.position = _positionOffset;
        }

        private static void SetSortingLayer(Tilemap source, Tilemap destination) {
            var sourceRenderer = source.GetComponent<TilemapRenderer>();
            var destinationRenderer = destination.GetComponent<TilemapRenderer>();

            if (sourceRenderer == null || destinationRenderer == null) {
                return;
            }

            destinationRenderer.sortingLayerID = sourceRenderer.sortingLayerID;

            var sourceOrder = sourceRenderer.sortingOrder;
            destinationRenderer.sortingOrder = sourceOrder - 1;
        }

        /// <summary>
        ///     Copies the source tilemap on the destination tilemap
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        private void Copy(Tilemap source, Tilemap destination) {
            source.RefreshAllTiles();
            destination.RefreshAllTiles();

            var referenceTilemapPositions = new List<Vector3Int>();

            // we grab all filled positions from the ref tilemap
            foreach (var pos in source.cellBounds.allPositionsWithin) {
                var localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                if (source.HasTile(localPlace)) {
                    referenceTilemapPositions.Add(localPlace + _tileOffset.ToInt3());
                }
            }

            // we turn our list into an array
            var positions = new Vector3Int[referenceTilemapPositions.Count];
            var allTiles = new TileBase[referenceTilemapPositions.Count];
            var i = 0;

            foreach (var tilePosition in referenceTilemapPositions) {
                positions[i] = tilePosition;
                allTiles[i] = _ruleTile;
                i++;
            }

            // we clear our tilemap and resize it
            destination.ClearAllTiles();
            destination.RefreshAllTiles();
            destination.size = source.size;
            destination.origin = source.origin;
            destination.ResizeBounds();

            // we feed it our positions
            destination.SetTiles(positions, allTiles);

            if (_extraBottomTile == null) {
                return;
            }

            // HACK: now the extra row in the bottom
            referenceTilemapPositions.Clear();

            foreach (var pos in destination.cellBounds.allPositionsWithin) {
                var localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                var bottomPlace = new Vector3Int(pos.x, pos.y - 1, pos.z);
                var leftTile = new Vector3Int(pos.x - 1, pos.y, pos.z);
                var rightTile = new Vector3Int(pos.x + 1, pos.y, pos.z);

                var border = (!destination.HasTile(leftTile) && destination.HasTile(rightTile)) ||
                     (destination.HasTile(leftTile) && !destination.HasTile(rightTile));

                if (destination.HasTile(localPlace) &&
                    !destination.HasTile(bottomPlace) &&
                    !border) {
                    referenceTilemapPositions.Add(bottomPlace);
                }
            }

            allTiles = new TileBase[referenceTilemapPositions.Count];
            positions = new Vector3Int[referenceTilemapPositions.Count];

            i = 0;

            foreach (var tilePosition in referenceTilemapPositions) {
                positions[i] = tilePosition;
                allTiles[i] = _extraBottomTile;
                i++;
            }

            destination.SetTiles(positions, allTiles);
        }

        [Button]
        public void ClearMap() {
            if (_tilemap == null) {
                return;
            }

            _tilemap.ClearAllTiles();
        }
    }
}