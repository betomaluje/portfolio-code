using BerserkPixel.StateMachine;
using UnityEngine;

namespace Shop.Keeper.States {
    internal class ShopKeeperIdle : State<ShopKeeperStateMachine> {
        public override void Enter(ShopKeeperStateMachine parent) {
            base.Enter(parent);
            parent.MakeBodyDynamic();
            parent.Animations.PlayIdle();
            parent.Movement.Stop();
        }

        public override void FixedTick(float fixedDeltaTime) { }

        public override void ChangeState() { }
    }
}