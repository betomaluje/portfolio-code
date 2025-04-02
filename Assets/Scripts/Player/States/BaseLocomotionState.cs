using BerserkPixel.StateMachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Player.States {

    public abstract class BaseLocomotionState : State<PlayerStateMachine> {
        [SerializeField]
        protected MovementConfig _movementConfig;

        [SerializeField]
        protected float _detectionDistance = .5f;

        [SerializeField]
        protected float _detectionRadius = .5f;

        [SerializeField]
        protected LayerMask _blockMask;

        [SerializeField]
        protected LayerMask _wallMask;

        [Header("DEBUG")]
        [SerializeField]
        protected bool _debug = true;

        [ShowIf("_debug")]
        [SerializeField]
        protected Color _walkableColor = Color.blue;

        [ShowIf("_debug")]
        [SerializeField]
        protected Color _notWalkableColor = Color.red;

        protected bool IsWalkableCell(Vector2 direction) {
            var currentPosition = (Vector2)_machine.transform.position;

            var endPoint = currentPosition + (direction * _detectionDistance);

            // Check for walls along the path
            var hitOnPath = Physics2D.Linecast(currentPosition, endPoint, _wallMask);
            if (hitOnPath.collider != null) {
                return false;
            }

            var hitOnEdge = Physics2D.OverlapCircle(endPoint, _detectionRadius, _blockMask);
            return hitOnEdge == null;
        }

        public override void OnDrawGizmos() {
            if (!_debug || _machine == null) {
                return;
            }


            var direction = _machine.PlayerInputMovement;

            var currentPosition = (Vector2)_machine.transform.position;

            var endPoint = currentPosition + (direction * _detectionDistance);

            var hitOnPath = Physics2D.Linecast(currentPosition, endPoint, _wallMask);

            Gizmos.color = hitOnPath.collider == null ? _walkableColor : _notWalkableColor;
            Gizmos.DrawLine(currentPosition, endPoint);

            var hitOnEdge = Physics2D.OverlapCircle(endPoint, _detectionRadius, _blockMask);

            Gizmos.color = hitOnEdge == null ? _walkableColor : _notWalkableColor;
            Gizmos.DrawWireSphere(endPoint, _detectionRadius);
        }
    }
}