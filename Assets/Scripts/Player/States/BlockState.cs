using BerserkPixel.StateMachine;
using UnityEngine;

namespace Player.States {
    [CreateAssetMenu(menuName = "Aurora/Player/States/Block")]
    public class BlockState : State<PlayerStateMachine> {
        [SerializeField]
        private float _blockCooldown = .8f;

        private float _elapsedTime;

        public override void Enter(PlayerStateMachine parent) {
            base.Enter(parent);
            parent.Animations.PlayBlock();
            // TODO: Add block sound
            parent.Block();
            _elapsedTime = 0f;
        }

        public override void Tick(float deltaTime) => _elapsedTime += deltaTime;

        public override void ChangeState() {
            if (_elapsedTime < _blockCooldown) {
                return;
            }

            _machine.SetState(typeof(IdleState));
        }

        override public void Exit() {
            base.Exit();
            _machine.UnBlock();
        }
    }
}