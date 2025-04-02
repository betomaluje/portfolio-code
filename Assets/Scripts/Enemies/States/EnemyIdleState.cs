using BerserkPixel.StateMachine;
using Detection;
using UnityEngine;

namespace Enemies.States {
    [CreateAssetMenu(menuName = "Aurora/Enemy/States/Idle")]
    public class EnemyIdleState : State<EnemyStateMachine> {
        [SerializeField]
        [Min(0f)]
        private float _minTime;

        [SerializeField]
        private float _maxTime = 5f;

        private float _elapsedTime;
        private float _waitingTime;

        private TargetDetection _enemyDetection;

        public override void Enter(EnemyStateMachine parent) {
            base.Enter(parent);

            if (_enemyDetection == null) {
                _enemyDetection = GetComponentInChildren<TargetDetection>();
            }

            parent.Animations.PlayIdle();
            parent.Movement.Stop();

            _elapsedTime = 0f;
            _waitingTime = Random.Range(_minTime, _maxTime);
        }

        public override void Tick(float deltaTime) => _elapsedTime += deltaTime;

        public override void ChangeState() {
            if (_enemyDetection != null && _enemyDetection.IsPlayerNear) {
                _machine.SetState(typeof(EnemyDetectState));
            }

            if (_elapsedTime >= _waitingTime) {
                _machine.SetState(typeof(EnemyWanderState));
                return;
            }
        }
    }
}