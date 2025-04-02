using BerserkPixel.StateMachine;
using Extensions;
using UnityEngine;

namespace Enemies.States {
    [CreateAssetMenu(menuName = "Aurora/Enemy/States/Wander")]
    public class EnemyWanderState : State<EnemyStateMachine> {
        [SerializeField]
        private float _radius = 5;

        [SerializeField]
        private float _moveSpeed = 15;

        [SerializeField]
        private LayerMask _blockMask;

        private float _elapsedTime, _moveDuration = .25f;
        private Vector2 _randomPoint;

        public override void Enter(EnemyStateMachine parent) {
            base.Enter(parent);
            parent.MakeBodyDynamic();
            parent.Animations.PlayRun();

            var currentPosition = (Vector2)parent.transform.position;
            _randomPoint = currentPosition.GetRandomPosition(_radius);
            _elapsedTime = 0f;

            var hit = Physics2D.OverlapCircle(
               _randomPoint,
               .2f,
               _blockMask
           );

            if (hit == null) {
                // we can walk there
                var distance = Vector2.Distance(_randomPoint, currentPosition);

                // speed = distance / time => time = distance / speed
                _moveDuration = distance / _moveSpeed;

                var direction = (_randomPoint - currentPosition).normalized;
                parent.Movement.FlipSprite(direction);
                parent.Movement.ApplyForce(direction, _moveSpeed, _moveDuration);
            }
            else {
                parent.SetState(typeof(EnemyIdleState));
            }
        }

        public override void OnDrawGizmosSelected() {
            base.OnDrawGizmosSelected();
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_randomPoint, .5f);
        }

        public override void Tick(float deltaTime) => _elapsedTime += deltaTime;

        public override void ChangeState() {
            if (_elapsedTime >= _moveDuration) {
                _machine.SetState(typeof(EnemyIdleState));
            }
        }
    }
}