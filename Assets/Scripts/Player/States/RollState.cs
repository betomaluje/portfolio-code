using UnityEngine;

namespace Player.States {
    [CreateAssetMenu(menuName = "Aurora/Player/States/Roll")]
    internal class RollState : BaseLocomotionState {
        private float _rollDuration = .5f;

        private float _rollSpeed = 50f;
        protected float _elapsedTime;

        public override void Enter(PlayerStateMachine parent) {
            base.Enter(parent);
            _rollSpeed = _movementConfig.RollSpeed;
            _rollDuration = _movementConfig.RollDuration;

            parent.Animations.PlayRoll();
            parent.Sounds.PlayRoll();
            parent.CharacterCollider.enabled = false;

            _elapsedTime = 0f;

            if (IsWalkableCell(parent.PlayerInputMovement)) {
                parent.Movement.ApplyForce(parent.PlayerInputMovement, _rollSpeed, _rollDuration);
            }
            else {
                parent.Movement.Stop();
            }
        }

        public override void Tick(float deltaTime) => _elapsedTime += deltaTime;

        public override void ChangeState() {
            if (_elapsedTime >= _rollDuration) {
                _machine.SetState(typeof(IdleState));
            }
        }

        public override void Exit() {
            base.Exit();
            _machine.CharacterCollider.enabled = true;
        }
    }
}