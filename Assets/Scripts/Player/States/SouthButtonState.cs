using Base;
using BerserkPixel.StateMachine;
using UnityEngine;
using Utils;

namespace Player.States {
    [CreateAssetMenu(menuName = "Aurora/Player/States/South Button")]
    public class SouthButtonState : State<PlayerStateMachine> {
        [SerializeField]
        private LayerMask _interactMask;

        [SerializeField]
        private AttackConfig _attackConfig;

        private StateType _stateType = StateType.None;

        public override void Enter(PlayerStateMachine parent) {
            base.Enter(parent);

            _stateType = StateType.None;

            DetectButtonPressed();
        }

        private void DetectButtonPressed() {
            // we check first for attack, if nothing, we then check for interaction
            var hitCount = _machine.InteractCollider.DetectAll(_attackConfig.TargetMask, out var hits);
            if (hitCount != 0) {
                _stateType = StateType.Attack;
            }
            else {
                // detect interaction
                var hit = _machine.InteractCollider.Detect(_interactMask);
                if (hit) {
                    _stateType = StateType.Interact;
                }
            }
        }

        public override void ChangeState() {
            switch (_stateType) {
                case StateType.Interact:
                    _machine.SetState(typeof(InteractState));
                    return;

                case StateType.Attack:
                default:
                    _machine.SetState(typeof(AttackState));
                    return;
            }
        }
    }

    internal enum StateType {
        None,
        Attack,
        Interact
    }
}