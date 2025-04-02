using BerserkPixel.StateMachine;

namespace Player {
    public class EmptyState : State<PlayerStateMachine> {
        public override void ChangeState() { }
    }
}