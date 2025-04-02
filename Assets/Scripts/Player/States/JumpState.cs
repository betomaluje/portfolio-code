using BerserkPixel.StateMachine;
using Sounds;
using UnityEngine;

namespace Player.States {
    [CreateAssetMenu(menuName = "Aurora/Player/States/Jump")]

    public class JumpState : State<PlayerStateMachine> {
        [SerializeField]
        private float _detectionDistance = .5f;

        [SerializeField]
        private float _detectionRadius = .5f;

        [SerializeField]
        private LayerMask _blockMask;

        [SerializeField]
        private float _animationDuration = 1f;

        [SerializeField]
        private float _translationDuration = 1.5f;

        [Header("DEBUG")]
        [SerializeField]
        private bool _debug = true;

        private float _elapsedTime;

        public override void Enter(PlayerStateMachine parent) {
            base.Enter(parent);

            _elapsedTime = 0f;

            if (IsWalkableCell(parent.PlayerInputMovement)) {     
                parent.Sounds.PlayJump();
                parent.Animations.Play("Jump");
            }
        }

        private bool IsWalkableCell(Vector2 direction) {
            var currentPosition = (Vector2)_machine.transform.position;

            var endPoint = currentPosition + (direction * _detectionDistance);
            var hitOnEdge = Physics2D.OverlapCircle(endPoint, _detectionRadius, _blockMask);

            if (hitOnEdge == null) {
                _machine.Movement.MoveToPoint(endPoint, _translationDuration);
                return true;
            }

            return false;
        }

        public override void OnDrawGizmos() {
            if (!_debug || _machine == null) {
                return;
            }

            var direction = _machine.PlayerInputMovement;

            var currentPosition = (Vector2)_machine.transform.position;

            var endPoint = currentPosition + (direction * _detectionDistance);
            var hitOnEdge = Physics2D.OverlapCircle(endPoint, _detectionRadius, _blockMask);

            Gizmos.color = hitOnEdge == null ? Color.blue : Color.red;
            Gizmos.DrawWireSphere(endPoint, _detectionRadius);
            Gizmos.DrawLine(currentPosition, endPoint);
        }

        public override void Tick(float deltaTime) => _elapsedTime += deltaTime;

        public override void ChangeState() {
            if (_elapsedTime >= _animationDuration) {
                _machine.SetState(typeof(IdleState));
            }
        }
    }
}