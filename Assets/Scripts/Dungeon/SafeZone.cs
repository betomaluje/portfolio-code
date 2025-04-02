using System;
using System.Collections;
using Extensions;
using Sounds;
using UnityEngine;

namespace Dungeon {
    [RequireComponent(typeof(SpriteRenderer))]
    public class SafeZone : MonoBehaviour {
        [SerializeField]
        private float _fadeDuration = 0.5f;
        [SerializeField]
        private float _speed = 5f;

        private SpriteRenderer _spriteRenderer;
        private Transform _trailRenderer;
        private Vector2[] _roomCorners;
        private int _targetPoint = 0;
        private bool _isRendering;

        private void Awake() {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _trailRenderer = GetComponentInChildren<TrailRenderer>().transform;
        }

        public void SetupSafeZone(Room room, bool shouldRenderVisuals = true) {
            _roomCorners = room.GenerateCorners();

            var scale = transform.localScale;
            scale.x = room.Width;
            scale.y = room.Height;
            transform.localScale = scale;
            transform.position = room.Center.ToVector2();

            _trailRenderer.position = _roomCorners[_targetPoint];

            _spriteRenderer.enabled = shouldRenderVisuals;
            _isRendering = shouldRenderVisuals;

            if (shouldRenderVisuals) {
                // also soften the music
                SoundManager.instance.PitchEverything(0.45f, false);
            }
        }

        private void Update() {
            if (!_isRendering) return;

            var newPosition = Vector2.MoveTowards(_trailRenderer.position, _roomCorners[_targetPoint], Time.deltaTime * _speed);
            var distance = Vector2.Distance(newPosition, _trailRenderer.position);

            if (distance < 0.1f) {
                _targetPoint = (_targetPoint + 1) % _roomCorners.Length;
            }

            _trailRenderer.position = newPosition;
        }

        public void RemoveSafeZone(Action callback) {
            if (gameObject.activeSelf) {
                StartCoroutine(FadeOut(callback));
            }
        }

        private IEnumerator FadeOut(Action callback) {
            var material = _spriteRenderer.material;
            var elapsedTime = 0f;

            while (elapsedTime < _fadeDuration) {
                var alpha = Mathf.MoveTowards(1f, 0f, elapsedTime / _fadeDuration);
                material.SetFloat("_Alpha", alpha);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            material.SetFloat("_Alpha", 0f);

            Destroy(gameObject);
            _isRendering = false;
            SoundManager.instance.PitchEverything(1f, false);
            callback?.Invoke();
        }
    }
}