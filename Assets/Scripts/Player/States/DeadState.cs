using BerserkPixel.StateMachine;
using UnityEngine;

namespace Player {
    [CreateAssetMenu(menuName = "Aurora/Player/States/Dead")]
    public class DeadState : State<PlayerStateMachine> {
        public override void Enter(PlayerStateMachine parent) {
            base.Enter(parent);
            parent.Animations.PlayDead();
            parent.Movement.Stop();
        }

        public override void ChangeState() {
            _machine.SetState(CreateInstance<EmptyState>());
            _machine.enabled = false;
        }

    }
}