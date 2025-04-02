using BerserkPixel.StateMachine;
using UnityEngine;

namespace NPCs.States {
    [CreateAssetMenu(menuName = "Aurora/NPC/States/Dead")]
    public class NPCDeadState : State<NPCStateMachine> {
        public override void Enter(NPCStateMachine parent) {
            base.Enter(parent);
            parent.MakeBodyDynamic();
            parent.Animations.PlayDead();
            parent.Movement.Stop();
        }

        public override void ChangeState() {
            _machine.SetState(CreateInstance<NPCEmptyState>());
        }
    }
}