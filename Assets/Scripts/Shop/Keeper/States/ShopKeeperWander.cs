using BerserkPixel.StateMachine;
using Extensions;
using UnityEngine;

namespace Shop.Keeper.States {
    internal class ShopKeeperWander : State<ShopKeeperStateMachine> {
        [SerializeField]
        private float _radius = 3;

        [SerializeField]
        private float _moveSpeed = 2;

        private float _elapsedTime, _moveDuration = .25f;
        private Vector2 _randomPoint;

        public override void Enter(ShopKeeperStateMachine parent) {
            base.Enter(parent);
            parent.MakeBodyDynamic();
            parent.Animations.PlayRun();

            var currentPosition = (Vector2)parent.transform.position;
            _randomPoint = currentPosition.GetRandomPosition(_radius);
            _elapsedTime = 0f;

            var distance = Vector2.Distance(_randomPoint, currentPosition);

            // speed = distance / time => time = distance / speed
            _moveDuration = distance / _moveSpeed;

            var direction = (_randomPoint - currentPosition).normalized;
            parent.Movement.FlipSprite(direction);
            parent.Movement.ApplyForce(direction, _moveSpeed, _moveDuration);
        }

        public override void Tick(float deltaTime) => _elapsedTime += deltaTime;

        public override void ChangeState() {
            if (_elapsedTime >= _moveDuration) {
                _machine.SetState(typeof(ShopKeeperIdle));
            }
        }

        public override void OnDrawGizmosSelected() {
            base.OnDrawGizmosSelected();
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_randomPoint, .5f);
        }
    }
}