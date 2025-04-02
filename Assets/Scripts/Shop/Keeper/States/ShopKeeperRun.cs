using BerserkPixel.StateMachine;
using Extensions;
using NPCs.Expressions;
using UnityEngine;

namespace Shop.Keeper.States {
    internal class ShopKeeperRun : State<ShopKeeperStateMachine> {
        private float _distanceFromPlayer = 6f;
        private float _moveSpeed = 2;
        private float _elapsedTime;
        private float _moveDuration = .25f;
        private float _distanceTo;

        private Vector2 _randomPoint, _currentPosition;

        public override void Enter(ShopKeeperStateMachine parent) {
            base.Enter(parent);
            parent.MakeBodyDynamic();
            parent.SetExpression(ExpressionType.Angry);
            parent.Animations.PlayRun();

            _currentPosition = (Vector2)parent.transform.position;
            _randomPoint = _currentPosition.GetRandomPosition(_distanceFromPlayer);
            _elapsedTime = 0f;

            _distanceTo = Vector2.Distance(_randomPoint, _currentPosition);

            // speed = distance / time => time = distance / speed
            _moveDuration = _distanceTo / _moveSpeed;

            var direction = (_randomPoint - _currentPosition).normalized;
            parent.Movement.FlipSprite(direction);
            parent.Movement.ApplyForce(direction, _moveSpeed, _moveDuration);
        }

        public override void Tick(float deltaTime) {
            _elapsedTime += deltaTime;
            _distanceTo = Vector2.Distance(_randomPoint, _currentPosition);
        }

        public override void ChangeState() {
            if (_distanceTo < .1f || _elapsedTime >= _moveDuration) {
                // arriving to destination
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