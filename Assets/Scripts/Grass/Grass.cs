using BerserkPixel.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Grass {
    [RequireComponent(typeof(SpriteRenderer))]
    public class Grass : MonoBehaviour {
        [SerializeField]
        private LayerMask _playerLayer;

        [SerializeField]
        [Range(0f, 1f)]
        private float _ExternalInfluenceStrength = .5f;

        [SerializeField]
        private float _duration = .5f;

        private readonly int _influenceProperty = Shader.PropertyToID("_ExternalInfluence");

        private float _enterOffsetX, _enterOffsetY, _startingXVelocity;

        private SpriteRenderer _spriteRenderer = null;
        private Material _material;

        private void Awake() {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _material = _spriteRenderer.material;
        }

        private void Start() {
            _startingXVelocity = _material.GetFloat(_influenceProperty);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (_playerLayer.LayerMatchesObject(other)) {
                _enterOffsetX = other.transform.position.x - transform.position.x;
                _enterOffsetY = other.transform.position.y - transform.position.y;
            }
        }

        private async void OnTriggerStay2D(Collider2D other) {
            if (_playerLayer.LayerMatchesObject(other) && other.TryGetComponent<Rigidbody2D>(out var rb)) {
                var offsetX = other.transform.position.x - transform.position.x;
                var offsetY = other.transform.position.y - transform.position.y;
                var enterOffsetXSign = Mathf.Sign(_enterOffsetX);
                var enterOffsetYSign = Mathf.Sign(_enterOffsetY);

                if (enterOffsetXSign != Mathf.Sign(offsetX)) {
                    var strength = _ExternalInfluenceStrength * -rb.linearVelocityX;
                    await DoInfluence(_startingXVelocity, strength);
                }

                if (enterOffsetYSign != Mathf.Sign(offsetY)) {
                    var strength = _ExternalInfluenceStrength * rb.linearVelocityY;
                    await DoInfluence(_startingXVelocity, strength);
                }
            }
        }

        private async void OnTriggerExit2D(Collider2D other) {
            if (_playerLayer.LayerMatchesObject(other)) {
                await DoInfluence(_spriteRenderer.material.GetFloat(_influenceProperty), 1);
            }
        }

        // simple method to offset the top 2 verts of a quad based on the offset and BEND_FACTOR constant
        private async UniTask DoInfluence(float startAmount, float endAmount) {
            var elapsed = 0f;
            while (elapsed < _duration) {
                elapsed += Time.deltaTime;
                var percentage = Mathf.Lerp(startAmount, endAmount, elapsed / _duration);
                if (_material != null) {
                    _material.SetFloat(_influenceProperty, percentage);
                }

                await UniTask.Yield();
            }

            if (_material != null) {
                _material.SetFloat(_influenceProperty, endAmount);
            }
        }
    }
}