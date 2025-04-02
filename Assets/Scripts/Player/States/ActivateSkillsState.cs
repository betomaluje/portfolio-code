using BerserkPixel.StateMachine;
using UnityEngine;

namespace Player.States {
    [CreateAssetMenu(menuName = "Aurora/Player/States/Powerup")]
    internal class ActivateSkillsState : State<PlayerStateMachine> {
        public override void Enter(PlayerStateMachine parent) {
            base.Enter(parent);
            parent.Movement.Stop();
            parent.Skills.ActivateSkills();
        }

        public override void ChangeState() {
            _machine.SetState(typeof(IdleState));
        }
    }
}