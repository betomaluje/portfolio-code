using BerserkPixel.StateMachine;
using Detection;
using Sirenix.OdinInspector;
using Sounds;
using UnityEngine;

namespace Enemies.States {
    [CreateAssetMenu(menuName = "Aurora/Enemy/States/Chase")]
    public class EnemyChaseState : State<EnemyStateMachine> {
        [SerializeField]
        [Range(0, 20)]
        private float _speed = 10f;

        [SerializeField]
        private bool _neverStopChasing = false;

        [SerializeField]
        private float _minChaseDistance = 1f;

        [HideIf("_neverStopChasing", true)]
        [SerializeField]
        private float _maxChaseDistance = 10f;

        private TargetDetection _enemyDetection;
        private Transform _target;

        override public void Enter(EnemyStateMachine parent) {
            base.Enter(parent);

            if (_enemyDetection == null) {
                _enemyDetection = GetComponentInChildren<TargetDetection>();
            }

            if (_enemyDetection == null) {
                _machine.SetState(typeof(EnemyWanderState));
                return;
            }

            _target = _enemyDetection.Target;
            if (_target == null) {
                _machine.SetState(typeof(EnemyWanderState));
                return;
            }

            SoundManager.instance.PlayWithPitchOnSpot("enemy_chase", parent.transform.position);
            parent.Animations.PlayRun();
            parent.MakeBodyDynamic();
        }

        public override void Tick(float deltaTime) {
            if (_target == null) {
                _machine.SetState(typeof(EnemyWanderState));
                return;
            }

            Vector2 direction = (_target.position - _machine.transform.position).normalized;
            _machine.Movement.Move(_speed * direction);
            _machine.Movement.FlipSprite(direction);
        }

        public override void ChangeState() {
            if (_target == null) {
                _machine.SetState(typeof(EnemyWanderState));
                return;
            }

            var distance = Vector3.Distance(_machine.transform.position, _target.position);
            if (!_neverStopChasing && distance >= _maxChaseDistance) {
                _machine.SetState(typeof(EnemyWanderState));
            }
            else if (distance <= _minChaseDistance) {
                _machine.OnTargetReached(_enemyDetection.TargetType);
            }
        }

    }
}