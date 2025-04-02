using System;
using BerserkPixel.Utils;
using Dungeon;
using Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Utils {
    public class RandomObjectPlacer : MonoBehaviour {
        [ListDrawerSettings(NumberOfItemsPerPage = 5)]
        [SerializeField]
        private WeightedListItem<DungeonObjectWrapper>[] _toPlace;

        [SerializeField]
        private Tilemap _groundTilemap;

        [SerializeField]
        private Transform _parent;

        [SerializeField]
        private int _amount;

        private WeightedList<DungeonObjectWrapper> _randomObjects;

        private void Start() {
            System.Random random = new(DateTime.Now.TimeOfDay.Minutes);
            _randomObjects = new(_toPlace, random);
        }

        private void OnValidate() {
            if (_parent == null) {
                _parent = transform;
            }
        }

        [Button]
        private void ClearObjects() {
            _parent.DestroyChildren();
        }

        [Button]
        public void PlaceRandomObjects() {
            ClearObjects();

            if (_randomObjects == null || _randomObjects.Count == 0) {
                System.Random random = new(DateTime.Now.TimeOfDay.Minutes);
                _randomObjects = new(_toPlace, random);
            }

            Vector3Int[] positions;

            if (_groundTilemap != null) {
                positions = _groundTilemap.GetRandomPositions(_amount).ToArray();
                DungeonPositionsHolder.Instance.AddUsedPositions(positions);
            }
            else {
                positions = DungeonPositionsHolder.Instance.GetRandomPoints(_amount);
            }

            foreach (var position in positions) {
                var pos = new Vector3(position.x, position.y, 0);
                var randomObject = _randomObjects.Next();
                var placed = Instantiate(randomObject.Transform, pos, Quaternion.identity, _parent);
                var scaleX = UnityEngine.Random.Range(randomObject.ScaleRangeX.x, randomObject.ScaleRangeX.y);
                var scaleY = UnityEngine.Random.Range(randomObject.ScaleRangeY.x, randomObject.ScaleRangeY.y);
                placed.localScale = new Vector3(scaleX, scaleY, 1);
            }
        }
    }

    [Serializable]
    internal class DungeonObjectWrapper {
        public Transform Transform;
        [Tooltip("Min and Max scale X of the object")]
        public Vector2 ScaleRangeX = new(.9f, 1.1f);
        [Tooltip("Min and Max scale Y of the object")]
        public Vector2 ScaleRangeY = new(.9f, 1.1f);
    }
}