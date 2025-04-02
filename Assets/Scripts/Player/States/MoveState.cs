using BerserkPixel.StateMachine;
using UnityEngine;

namespace Player.States {
    [CreateAssetMenu(menuName = "Aurora/Player/States/Move")]
    internal class MoveState : BaseLocomotionState {
        private Vector2 _playerInput;
        // private Vector2 _playerAimDirection;

        private float _speed;

        public override void Enter(PlayerStateMachine parent) {
            base.Enter(parent);
            _speed = _movementConfig.Speed;
            _playerInput = _machine.PlayerInputMovement;
            // _playerAimDirection = _machine.PlayerAimDirection;
            if (IsWalkableCell(_playerInput)) {
                parent.Animations.PlayRun();
            }
        }

        public override void AnimationTriggerEvent(AnimationTriggerType triggerType) {
            base.AnimationTriggerEvent(triggerType);
            if (triggerType.Equals(AnimationTriggerType.Footstep)) {
                _machine.Sounds.PlayFootstep();
            }
        }

        public override void Tick(float deltaTime) {
            _playerInput = _machine.PlayerInputMovement;
            // _playerAimDirection = _machine.PlayerAimDirection;

            if (IsWalkableCell(_playerInput)) {
                _machine.Movement.Move(_speed * _playerInput);
                // _machine.Movement.FlipSprite(_playerAimDirection);
            }
        }

        public override void ChangeState() {
            if (!_machine.IsMoving) {
                _machine.SetState(typeof(IdleState));
                return;
            }
        }
    }
}