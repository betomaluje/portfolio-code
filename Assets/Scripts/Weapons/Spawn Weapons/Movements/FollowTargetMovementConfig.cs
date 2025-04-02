using UnityEngine;

namespace Weapons {
    public class FollowTargetMovementConfig : MovementConfig {
        private readonly Transform _target;
        private readonly AnimationCurve _movementCurve;
        private readonly float _duration = 1.0f;
        private readonly float _lifeTime = 3.0f;

        private float _movementTime;
        private Vector2 _targetPosition;

        public FollowTargetMovementConfig(Transform transform, Transform target, float lifeTime, float duration, AnimationCurve movementCurve) : base(transform) {
            _target = target;
            _lifeTime = lifeTime;
            _duration = duration;
            _movementCurve = movementCurve;
            _movementTime = 0;
        }

        public override void Move(float deltaTime) {
            _movementTime += deltaTime;

            _targetPosition = _target.position;

            transform.position = Vector3.Lerp(transform.position, _targetPosition, _movementCurve.Evaluate(_movementTime / _duration));

            var distance = Vector2.Distance(transform.position, _targetPosition);
            if (distance <= .2f || _movementTime >= _lifeTime) {
                OnMoveEnd?.Invoke();
            }
        }
    }
}