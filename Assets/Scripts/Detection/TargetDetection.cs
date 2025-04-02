using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace Detection {
    public class TargetDetection : Detection {
        [SerializeField]
        private TargetType _targetType = TargetType.Enemy;

        public UnityEvent<Transform> OnPlayerDetected;
        public UnityEvent OnPlayerLost;
        public bool IsPlayerNear { get; private set; }
        public Vector3 GetTargetPosition { get; private set; }
        public Transform Target { get; private set; }
        public TargetType TargetType => _targetType;

        protected override void Detect() {
            var hit = _collider.DetectWithAngle(_targetMask, _detectionAngle);
            IsPlayerNear = hit;
            if (hit) {
                GetTargetPosition = hit.point;
                Target = hit.transform;
                if (Target != null && !_hasDetected) {
                    _hasDetected = true;
                    OnPlayerDetected?.Invoke(Target);
                }
            }
            else {
                // nothing, check if it has detected something before
                if (_hasDetected) {
                    GetTargetPosition = transform.position;
                    _hasDetected = false;
                    Target = null;
                    OnPlayerLost?.Invoke();
                }
            }
        }

        public void ForceDetection() {
            Detect();
        }
    }
}