using UnityEngine;

namespace Weapons {
    public class StraightSpawnWeapon : BaseSpawnWeapon {
        [SerializeField]
        private float _duration = 1.0f;

        [SerializeField]
        private AnimationCurve _movementCurve;

        private MovementConfig _movementConfig;
        protected override MovementConfig MovementConfig => _movementConfig;

        public override void Shoot() {
            var hitOnEdge = Physics2D.OverlapCircle(transform.position, _weapon.Range, _targetMask);
            var targetPosition = transform.position;
            if (hitOnEdge) {
                targetPosition = hitOnEdge.transform.position;
            }

            targetPosition += _direction * _weapon.Range;

            _movementConfig = new StraightMovementConfig(
                transform,
                targetPosition,
                _duration,
                _movementCurve
            );

            _movementConfig.SetTimerEndAction(Deactivate);
        }

        private void Update() => _movementConfig.Move(Time.deltaTime);

        private void OnTriggerEnter2D(Collider2D other) {
            if (CheckCollision(other)) { }
        }

        private void OnDrawGizmosSelected() {
            if (_weapon == null) return;

            var currentPosition = (Vector2)transform.position;

            var hitOnEdge = Physics2D.OverlapCircle(transform.position, _weapon.Range, _targetMask);

            Gizmos.color = hitOnEdge != null ? Color.red : Color.blue;
            Gizmos.DrawLine(currentPosition, hitOnEdge != null ? hitOnEdge.transform.position : currentPosition);
        }

    }
}