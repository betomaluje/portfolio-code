using BerserkPixel.StateMachine;
using UnityEngine;

namespace Shop.Keeper.States {
    public class ShopKeeperDead : State<ShopKeeperStateMachine> {
        public override void Enter(ShopKeeperStateMachine parent) {
            base.Enter(parent);
            parent.MakeBodyDynamic();
            parent.Animations.PlayDead();
            parent.Movement.Stop();
        }

        public override void Tick(float deltaTime) { }

        public override void FixedTick(float fixedDeltaTime) { }

        public override void ChangeState() {
            _machine.SetState(CreateInstance<ShopKeeperEmpty>());
        }
    }
}