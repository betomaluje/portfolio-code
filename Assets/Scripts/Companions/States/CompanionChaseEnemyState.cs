using System.Linq;
using BerserkPixel.StateMachine;
using Detection;
using Extensions;
using UnityEngine;

namespace Companions.States {
    [CreateAssetMenu(menuName = "Aurora/Companions/States/Chase Enemy")]
    public class CompanionChaseEnemyState : State<CompanionStateMachine> {
        [SerializeField]
        [Range(0, 20)]
        private float _speed = 12f;

        [SerializeField]
        private float _maxChaseDistance = 10f;

        private TargetDetection _enemyDetection;
        private Transform _target;

        public override void Enter(CompanionStateMachine parent) {
            base.Enter(parent);
            if (_enemyDetection == null) {
                var allDetections = GetComponentsInChildren<TargetDetection>();
                _enemyDetection = allDetections.FirstOrDefault(x => x.TargetType == TargetType.Enemy);
            }
            if (_enemyDetection == null) {
                parent.SetState(typeof(CompanionFollowState));
                return;
            }

            _target = _enemyDetection.GetTarget();

            parent.Animations.PlayRun();
            parent.MakeBodyDynamic();
        }

        public override void Tick(float deltaTime) {
            if (_target == null) {
                return;
            }

            Vector2 direction = (_target.position - _machine.transform.position).normalized;
            
            _machine.Movement.Move(_speed * direction);
            _machine.Movement.FlipSprite(direction);
        }

        public override void ChangeState() {
            if (_target == null) {
                _machine.SetState(typeof(CompanionFollowState));
                return;
            }

            var distance = Vector3.Distance(_machine.transform.position, _target.position);
            if (distance >= _maxChaseDistance) {
                _machine.SetState(typeof(CompanionFollowState));
                return;
            }

            if (_enemyDetection.IsPlayerNear) {
                _machine.SetState(typeof(CompanionAttackState));
            }
        }
    }
}