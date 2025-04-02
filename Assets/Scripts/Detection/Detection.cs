using UnityEngine;
using Utils;

namespace Detection {
    public abstract class Detection : MonoBehaviour {
        [SerializeField]
        protected LayerMask _targetMask;

        [SerializeField]
        protected Collider2D _collider;

        [SerializeField]
        [Range(0, 360)]
        protected int _detectionAngle = 30;

        [SerializeField]
        protected float _resetDuration = .5f;

        protected bool _hasDetected;

        private const int _frameInterval = 3;

        private CountdownTimer _timer;

        private void Awake() {
            _timer = new CountdownTimer(_resetDuration);
            _timer.OnTimerStop += DetectionReset;
        }

        protected virtual void Start() {
            _hasDetected = false;
            _timer?.Start();
        }

        private void OnValidate() {
            if (_collider == null) {
                _collider = GetComponent<Collider2D>();
                _collider.isTrigger = true;
            }
        }

        protected virtual void OnDestroy() {
            if (_timer != null) {
                _timer.Reset(0);
                _timer.OnTimerStop -= DetectionReset;
            }
        }

        private void DetectionReset() {
            _hasDetected = false;
            _timer?.Reset();
        }

        protected virtual void Update() {
            _timer?.Tick(Time.deltaTime);
        }

        private void LateUpdate() {
            if (_collider == null) {
                return;
            }

            if (Time.frameCount % _frameInterval == 0) {
                Detect();
            }
        }

        protected abstract void Detect();
    }
}