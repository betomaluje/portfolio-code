using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

namespace Camera {
    public class CameraShockwave : Singleton<CameraShockwave> {
        [SerializeField]
        private float _shockwaveDuration = 1f;

        [SerializeField]
        private Transform _shockwaveContainer;

        private static readonly int _waveDistanceFromCenter = Shader.PropertyToID("_WaveDistanceFromCenter");
        private Material _material;
        private Coroutine _coroutine;

        protected override void Awake() {
            base.Awake();

            if (_shockwaveContainer != null && _shockwaveContainer.TryGetComponent(out SpriteRenderer spriteRenderer)) {
                _material = spriteRenderer.sharedMaterial;
            }
        }

        private void Start() {
            _shockwaveContainer.gameObject.SetActive(false);
        }

        private void OnDisable() {
            if (_material != null) {
                _material.SetFloat(_waveDistanceFromCenter, -0.1f);
            }
        }

        private void OnDestroy() {
            if (_coroutine != null) {
                StopCoroutine(_coroutine);
            }
        }

        public void DoShockwave(Vector2 position) {
            DoShockwave(position, -0.1f, 1f);
        }

        public void DoShockwave(Vector2 position, float from, float to) {
            _shockwaveContainer.position = position;
            _shockwaveContainer.gameObject.SetActive(true);

            if (_coroutine != null) {
                StopCoroutine(_coroutine);
            }

            _coroutine = StartCoroutine(Shockwave(from, to));
        }

        private IEnumerator Shockwave(float from, float to) {
            var elapsedTime = 0f;

            while (elapsedTime < _shockwaveDuration) {
                elapsedTime += Time.deltaTime;
                var percentage = Mathf.Lerp(from, to, elapsedTime / _shockwaveDuration);
                _material.SetFloat(_waveDistanceFromCenter, percentage);
                yield return null;
            }

            _material.SetFloat(_waveDistanceFromCenter, to);

            _shockwaveContainer.gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        [Button("Boom boom")]
        private void Shockwave() {
            if (_shockwaveContainer != null && _shockwaveContainer.TryGetComponent(out SpriteRenderer spriteRenderer)) {
                _material = spriteRenderer.sharedMaterial;
                DoShockwave(transform.position);
            }
        }
#endif

    }
}