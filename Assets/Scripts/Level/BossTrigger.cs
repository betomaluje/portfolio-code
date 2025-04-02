using BerserkPixel.Utils;
using UnityEngine;

namespace Level {
    [RequireComponent(typeof(Collider2D))]
    public class BossTrigger : MonoBehaviour {
        [SerializeField]
        private LayerMask _targetMask;

        [SerializeField]
        private GameObject[] _toAppear;

        [SerializeField]
        private GameObject[] _toDisappear;

        private bool _activated;

        private void Awake() {
            foreach (GameObject obj in _toAppear) {
                obj.SetActive(false);
            }

            foreach (GameObject obj in _toDisappear) {
                obj.SetActive(true);
            }

            _activated = false;
        }

        private void OnValidate() {
            var collider = GetComponent<Collider2D>();
            collider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (!_activated && _targetMask.LayerMatchesObject(other)) {
                foreach (GameObject obj in _toAppear) {
                    obj.SetActive(true);
                }

                foreach (GameObject obj in _toDisappear) {
                    obj.SetActive(false);
                }

                _activated = true;
            }

        }
    }
}