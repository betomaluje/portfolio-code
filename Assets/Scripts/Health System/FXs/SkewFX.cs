using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BerserkPixel.Health.FX {
    public class SkewFX : MonoBehaviour, IFX {
        [SerializeField]
        private Transform _target;

        [SerializeField]
        private SkewMode _skewMode = SkewMode.Mode2D;

        [Tooltip("Skew factor to switch to")]
        [SerializeField]
        private float skewFactor = .4f;

        [Tooltip("Duration of the flash.")]
        [SerializeField]
        private float duration = .3f;

        // The currently running coroutine.
        private Coroutine _skewRoutine;
        private WaitForSeconds _waitingTime;
        private Vector3 _originalScale;
        private bool _isSkewing = false;

        public FXType GetFXType() => FXType.OnlyNotImmune;

        public FXLifetime LifetimeFX => FXLifetime.Always;

        private void Awake() {
            _waitingTime = new WaitForSeconds(duration);
            _originalScale = _target.localScale;
        }

        private void OnValidate() {
            if (_target == null) {
                var spriteObject = transform.parent.Find("Sprite");
                _target = spriteObject;
            }
        }

        private void OnDestroy() {
            _target.localScale = _originalScale;
        }

        public void DoFX(HitData hitData) {
            if (_isSkewing) {
                return;
            }

            _isSkewing = true;

            if (_skewRoutine != null) {
                StopCoroutine(_skewRoutine);
            }
            _skewRoutine = StartCoroutine(SkewRoutine(Mathf.Sign(hitData.direction.x)));
        }

        private IEnumerator SkewRoutine(float hitDirectionX) {
            // we flip the sprite if we are hitting the target from the other direction
            if (Mathf.Sign(_originalScale.x) == hitDirectionX) {
                _originalScale.x *= -1;
            }

            // Swap to the skewFactor.
            var x = _originalScale.x + Random.Range(-skewFactor, skewFactor);
            var y = _originalScale.y + Random.Range(-skewFactor, skewFactor);
            var z = _originalScale.z;

            if (_skewMode == SkewMode.Mode3D) {
                z = _originalScale.z + Random.Range(-skewFactor, skewFactor);
            }

            _target.localScale = new Vector3(x, y, z);

            // Pause the execution of this function for "duration" seconds.
            yield return _waitingTime;

            // After the pause, swap back to the original scale.
            _target.localScale = _originalScale;

            // Set the routine to null, signaling that it's finished.
            _skewRoutine = null;

            _isSkewing = false;
        }
    }

    [Serializable]
    public enum SkewMode {
        Mode2D = 0,
        Mode3D = 1
    }
}