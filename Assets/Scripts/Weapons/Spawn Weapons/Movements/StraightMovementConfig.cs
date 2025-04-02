using UnityEngine;

namespace Weapons {
    public class StraightMovementConfig : MovementConfig {
        private readonly AnimationCurve _movementCurve;
        private readonly Vector2 _targetPosition;
        private readonly float _duration = 1.0f;

        private float _movementTime;

        public StraightMovementConfig(Transform transform, Vector2 targetPosition, float duration, AnimationCurve movementCurve) : base(transform) {
            _movementTime = 0;
            _targetPosition = targetPosition;
            _duration = duration;
            _movementCurve = movementCurve;
        }

        public override void Move(float deltaTime) {
            _movementTime += deltaTime;

            transform.position = Vector3.Lerp(transform.position, _targetPosition, _movementCurve.Evaluate(_movementTime / _duration));

            var distance = Vector2.Distance(transform.position, _targetPosition);
            if (distance <= .2f) {
                OnMoveEnd?.Invoke();
            }

            if (_movementTime >= _duration) {
                _movementTime = 0;
                OnTimerEnd?.Invoke();
            }
        }
    }
}