using Interactable;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace Detection {
    public class InteractionDetection : Detection {
        public UnityEvent<IInteractTag> OnInteractionDetected;
        public UnityEvent OnInteractionLost;

        private int _lastDetectedHash = -1;
        private float _elapsedTime;

        protected override void Start() {
            base.Start();
            _lastDetectedHash = -1;
        }

        private void ResetDetection() {
            _hasDetected = false;
            _lastDetectedHash = -1;
        }

        protected override void Update() {
            base.Update();
            if (_hasDetected) {
                _elapsedTime -= Time.deltaTime;
            }
        }

        protected override void Detect() {
            var hit = _collider.DetectWithAngle(_targetMask, _detectionAngle);

            if (hit) {
                var tag = hit.transform.GetComponentInChildren<IInteractTag>();
                if (hit.transform.GetHashCode() != _lastDetectedHash && tag != null) {
                    _lastDetectedHash = hit.transform.GetHashCode();
                    _hasDetected = true;
                    _elapsedTime = _resetDuration;
                    OnInteractionDetected?.Invoke(tag);
                }
            }
            else {
                if (_hasDetected && _elapsedTime <= 0) {
                    // nothing, check if it has detected something before
                    _hasDetected = false;
                    _lastDetectedHash = -1;
                    OnInteractionLost?.Invoke();
                }
            }
        }
    }
}