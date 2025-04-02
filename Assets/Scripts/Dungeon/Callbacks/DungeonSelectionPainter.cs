using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Dungeon {
    public class DungeonSelectionPainter : MonoBehaviour, IDungeonCallback {
        [Header("Render")]
        [SerializeField]
        private Sprite _squareMaterial;

        [SerializeField]
        private float _squareScale = 1f;

        [SerializeField]
        private Material _normalRoomMaterial;

        [SerializeField]
        private Material _selectedRoomMaterial;

        public float FadeDuration = 0.2f;

        private Dictionary<string, SpriteRenderer> _renderers;

        private Transform _callbackReceiver;
        private Dictionary<int, Transform> _allRooms;

        public void OnMapStarts() {
            _renderers = new();
        }

        public void OnAllRoomsGenerated(ref IList<Room> rooms, ref IList<Transform> roomTransforms) {
            _callbackReceiver = FindFirstObjectByType<DungeonMap>().transform.Find("DungeonContainer");
            _allRooms = new();

            int i = 0;
            foreach (var roomTransform in roomTransforms) {
                var key = roomTransform.gameObject.name;

                var containerChild = FindInContainer(key);
                if (containerChild != null) {
                    roomTransform.SetParent(containerChild);
                }

                roomTransform.localPosition = Vector3.zero;
                roomTransform.localScale = new Vector3(_squareScale, _squareScale, 1f);

                var renderer = roomTransform.gameObject.AddComponent<SpriteRenderer>();
                renderer.sprite = _squareMaterial;
                renderer.material = _normalRoomMaterial;

                // since we are creating the gameObject with the room.ToString() as name, they should be unique
                _renderers[key] = renderer;

                _allRooms[i] = roomTransform;

                i++;
            }
        }

        public void OnRoomsSelected(ref IList<Room> selectedRooms) {
            foreach (var item in selectedRooms) {
                if (_renderers.TryGetValue(item.ToString(), out var renderer)) {
                    renderer.material = _selectedRoomMaterial;

                    var sourceOrder = renderer.sortingOrder;
                    renderer.sortingOrder = sourceOrder + 1;
                }
            }
        }

        private Transform FindInContainer(string name) {
            try {
                var toSearch = !name.EndsWith("(Clone)") ? $"{name}(Clone)" : name;
                return _callbackReceiver.Find($"{toSearch}");
            }
            catch (Exception e) {
                Debug.Log($"Can't find {name} -> {e}");
                return null;
            }
        }

        private async void DestroyObjects() {
            foreach (var item in _renderers) {
                var renderer = item.Value;
                if (renderer == null) {
                    continue;
                }

                await Fade(renderer, 0f, 1f, FadeDuration);
            }

            _callbackReceiver.gameObject.SetActive(false);
        }

        private async UniTask Fade(SpriteRenderer renderer, float from, float to, float duration = 0.5f) {
            var preAmount = from;
            var endAmount = to;
            var elapsed = 0f;
            string fadeProperty = "_FadeAmount";
            Material material = renderer.material;

            material.SetFloat(fadeProperty, preAmount);

            while (elapsed < duration) {
                elapsed += Time.deltaTime;
                var percentage = Mathf.Lerp(preAmount, endAmount, elapsed / duration);
                material.SetFloat(fadeProperty, percentage);
                await UniTask.Yield();
            }

            material.SetFloat(fadeProperty, endAmount);
        }

        public void OnRoomsToCenter(ref IList<Room> rooms, ref IList<Vector3Int> positions) { }

        public void OnMapLoaded() {
            DestroyObjects();
        }
    }
}