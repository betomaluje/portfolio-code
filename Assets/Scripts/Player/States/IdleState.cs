using BerserkPixel.StateMachine;
using UnityEngine;

namespace Player.States {
    [CreateAssetMenu(menuName = "Aurora/Player/States/Idle")]
    internal class IdleState : State<PlayerStateMachine> {
        public override void Enter(PlayerStateMachine parent) {
            base.Enter(parent);
            parent.Animations.PlayIdle();
            parent.Movement.Stop();
        }

        public override void ChangeState() {
            // if (_machine.AttackPressed) {
            //     _machine.SetState(typeof(AttackState));
            //     return;
            // }

            if (_machine.IsMoving) {
                _machine.SetState(typeof(MoveState));
            }
        }
    }
}