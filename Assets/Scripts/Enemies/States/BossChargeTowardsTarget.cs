using Base.MovementPrediction;
using BerserkPixel.StateMachine;
using Detection;
using Enemies.Bosses;
using Player;
using UnityEngine;

namespace Enemies.States {
    [CreateAssetMenu(menuName = "Aurora/Enemy/States/Charge Towards")]
    public class BossChargeTowardsTarget : State<EnemyStateMachine> {
        [SerializeField]
        private MovementConfig _movementConfig;

        [SerializeField]
        private PredictionConfig _predictionConfig;

        [Header("DEBUG")]
        [SerializeField]
        private bool _debug = true;

        private TargetDetection _enemyDetection;

        private float _elapsedTime;

        private float _dashDuration = .5f;

        private float _dashSpeed = 50f;
        private MovementPredictor _movementPredictor;

        public override void Enter(EnemyStateMachine parent) {
            base.Enter(parent);

            _dashSpeed = _movementConfig.RollSpeed;
            _dashDuration = _movementConfig.RollDuration;

            if (_enemyDetection == null) {
                _enemyDetection = GetComponentInChildren<TargetDetection>();
            }

            _elapsedTime = 0f;

            if (_movementPredictor == null && _predictionConfig != null && _enemyDetection.Target != null) {
                _movementPredictor = new MovementPredictor(_enemyDetection.Target, parent.transform, _predictionConfig.Chance);
            }

            Vector3 direction;
            if (_movementPredictor != null) {
                direction = _movementPredictor.PredictTargetDirection(isDebug: _debug);
            }
            else {
                direction = (_enemyDetection.GetTargetPosition - parent.transform.position).normalized;
            }
            
            parent.MakeBodyDynamic();
            parent.Animations.PlayRun();
            parent.Movement.FlipSprite(direction);
            parent.Movement.ApplyForce(direction, _dashSpeed, _dashDuration);

            if (!_debug) {
                return;
            }

            var startingPos = parent.transform.position;
            // d = v * t
            var endDirection = startingPos + _dashSpeed * _dashDuration * direction;

            Debug.DrawLine(
                startingPos,
                endDirection,
                Color.blue,
                1f
            );
        }

        public override void Tick(float deltaTime) => _elapsedTime += deltaTime;

        public override void ChangeState() {
            if (_elapsedTime >= _dashDuration) {
                if (_enemyDetection.IsPlayerNear) {
                    _machine.SetState(typeof(RandomAttackState));
                }
                else {
                    _machine.SetState(typeof(EnemyIdleState));
                }
            }
        }
    }
}