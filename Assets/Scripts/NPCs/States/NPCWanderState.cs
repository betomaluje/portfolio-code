using BerserkPixel.StateMachine;
using Extensions;
using NPCs.Expressions;
using UnityEngine;

namespace NPCs.States {
    [CreateAssetMenu(menuName = "Aurora/NPC/States/Wander")]
    internal class NPCWanderState : State<NPCStateMachine> {
        [SerializeField]
        private float _radius = 3;

        [SerializeField]
        private float _moveSpeed = 2;

        private float _elapsedTime, _moveDuration = .25f;
        private Vector2 _randomPoint;

        public override void Enter(NPCStateMachine parent) {
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

        public override void OnDrawGizmosSelected() {
            base.OnDrawGizmosSelected();
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_randomPoint, .5f);
        }

        public override void Tick(float deltaTime) => _elapsedTime += deltaTime;

        public override void ChangeState() {
            if (_machine.HasBeenRescued) {
                _machine.SetExpression(ExpressionType.Surprised);
                _machine.SetState(typeof(NPCFollowPayerState));
                return;
            }

            if (_elapsedTime >= _moveDuration) {
                _machine.SetState(typeof(NPCIdleState));
            }
        }
    }
}