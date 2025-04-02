using BerserkPixel.StateMachine;

namespace Shop.Keeper {
    public class ShopKeeperEmpty : State<ShopKeeperStateMachine> {
        public override void Tick(float deltaTime) { }

        public override void FixedTick(float fixedDeltaTime) { }

        public override void ChangeState() { }
    }
}