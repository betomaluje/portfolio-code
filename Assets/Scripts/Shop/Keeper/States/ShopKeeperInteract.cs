using BerserkPixel.StateMachine;
using NPCs.Expressions;
using UnityEngine;

namespace Shop.Keeper.States {
    internal class ShopKeeperInteract : State<ShopKeeperStateMachine> {
        public override void Enter(ShopKeeperStateMachine parent) {
            base.Enter(parent);
            parent.Animations.PlayIdle();
            parent.Movement.Stop();
            parent.SetExpression(ExpressionType.Surprised);
            Time.timeScale = 0f;
        }

        public override void ChangeState() { }

		public override void Exit() {
			base.Exit();
            Time.timeScale = 1f;
        }
	}
}