using Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modifiers.Conditions {
    [CreateAssetMenu(menuName = "Aurora/Modifiers/Conditions/Don Not Move")]
    [TypeInfoBox("Condition triggered when the entity doesn't move for a certain amount of time")]
    public class DoesNotMoveCondition : BaseCondition {
        [Tooltip("The threshold value for the movement speed. If the entity's movement speed is less than this value, the condition is triggered.")]
        [SerializeField]
        private float _threshold = 1f;

        private PlayerStateMachine _machine;
        private float _timer;
        private bool _internalTimerTriggered;

        public override void Setup(Transform owner) {
            if (owner.TryGetComponent(out _machine)) {
            }

            _timer = 0;
            _internalTimerTriggered = false;
        }

        public override void ResetCondition() {
            base.ResetCondition();
            _timer = 0;
            _internalTimerTriggered = false;
        }

        public override bool Check(float deltaTime) {
            if (_machine == null) {
                return false;
            }

            bool currentlyMoving = _machine.IsMoving;

            // if it's curerntly moving, we reset the timer
            if (currentlyMoving) {
                _timer = 0;
                return false;
            }

            // we are currently not moving, we need to check the timer using the threshold
            _timer += deltaTime;
            if (_timer >= _threshold) {
                // we reset the timer
                _timer = 0;
                _internalTimerTriggered = true;

                return _internalTimerTriggered;
            }

            return _internalTimerTriggered;
        }

        public override void Cleanup() {
            _machine = null;
        }
    }
}