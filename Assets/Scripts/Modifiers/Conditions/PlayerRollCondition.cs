using Player;
using Player.Input;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modifiers.Conditions {
    [CreateAssetMenu(fileName = "On Roll Condition", menuName = "Aurora/Modifiers/Conditions/On Roll Condition")]
    [TypeInfoBox("Condition that triggers when the player rolls a certain amount of times")]
    public class PlayerRollCondition : BaseCondition {
        [Tooltip("The amount of rolls to trigger the condition.")]
        [SerializeField]
        [Min(0)]
        private int _amountOfRolls = 1;

        private IPlayerInput _playerInput;
        private PlayerStateMachine _machine;
        private bool _rollPressed;
        private int _currentRolls;

        public override void Setup(Transform owner) {
            if (owner.TryGetComponent(out _machine)) {
                _playerInput = _machine.GetComponent<IPlayerInput>();
                _playerInput.EastButtonEvent += OnRollPressed;
            }

            _currentRolls = 0;
        }

        private void OnRollPressed() {
            if (_rollPressed) {
                _currentRolls = 0;
                return;
            }

            // the player must press roll && move/point to a direction
            bool currentlyMoving = _machine.IsMoving;

            if (!currentlyMoving) {
                _currentRolls = 0;
                return;
            }

            _currentRolls++;
            if (_currentRolls >= _amountOfRolls) {
                _rollPressed = true;
                _currentRolls = 0;
            }
        }

        public override void ResetCondition() {
            base.ResetCondition();
            _rollPressed = false;
        }

        public override bool Check(float deltaTime) => _playerInput != null && _rollPressed;

        public override void Cleanup() {
            if (_playerInput != null) {
                _playerInput.EastButtonEvent -= OnRollPressed;
            }
            _rollPressed = false;
        }
    }
}