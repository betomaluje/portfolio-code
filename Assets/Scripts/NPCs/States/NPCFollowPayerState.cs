using BerserkPixel.StateMachine;
using BerserkPixel.Utils;
using NPCs.Expressions;
using UnityEngine;

namespace NPCs.States {
    [CreateAssetMenu(menuName = "Aurora/NPC/States/Follow")]
    internal class NPCFollowPayerState : State<NPCStateMachine> {
        [SerializeField]
        [Min(0f)]
        private float _distanceFromPlayer = 1f;

        [SerializeField]
        [Min(0f)]
        private float _minDistance = .1f;

        [SerializeField]
        [Range(0, 20)]
        private float _speed = 10f;

        private Vector2 _destination;
        private float _extraDistance, _distanceTo;

        public override void Enter(NPCStateMachine parent) {
            base.Enter(parent);
            parent.MakeBodyDynamic();
            parent.SetExpression(ExpressionType.Happy);
            _extraDistance = Random.Range(0, _distanceFromPlayer);
            _machine.Animations.PlayRun();

            _destination = GetDestination();
        }

        private Vector2 GetDestination() {
            return (Vector2)_machine.PlayerTransform.position + Random.insideUnitCircle * _extraDistance;
        }

        public override void Tick(float deltaTime) {
            _distanceTo = Vector2.Distance(_destination, _machine.transform.position);

            if (_distanceTo > (_minDistance + _extraDistance)) {
                Vector2 direction = (_destination - (Vector2)_machine.transform.position).normalized;
                _machine.Movement.Move(_speed * direction);
                _machine.Movement.FlipSprite(direction);
            }

            _destination = GetDestination();
        }

        public override void ChangeState() { }
    }
}