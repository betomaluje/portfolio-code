using Base.MovementPrediction;
using BerserkPixel.StateMachine;
using Detection;
using UnityEngine;

namespace Enemies.States {
    [CreateAssetMenu(menuName = "Aurora/Enemy/States/Dash")]
    public class EnemyDashState : State<EnemyStateMachine> {
        [SerializeField]
        [Range(0, 20)]
        private float _speed = 10f;

        [SerializeField]
        private float _duration = .8f;

        [SerializeField]
        private bool _invertDetectionDirection = false;

        [SerializeField]
        private PredictionConfig _predictionConfig;

        [Header("DEBUG")]
        [SerializeField]
        private bool _debug = true;

        private TargetDetection _enemyDetection;
        private MovementPredictor _movementPredictor;

        private float _elapsedTime;

        public override void Enter(EnemyStateMachine parent) {
            base.Enter(parent);

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

            if (_invertDetectionDirection) {
                direction = -direction;
            }

            parent.MakeBodyDynamic();
            parent.Animations.PlayRun();
            parent.Movement.FlipSprite(direction);
            parent.Movement.ApplyForce(direction, _speed, _duration);

            if (!_debug) {
                return;
            }

            var startingPos = parent.transform.position;
            // d = v * t
            var endDirection = startingPos + _speed * _duration * direction;

            Debug.DrawLine(
                startingPos,
                endDirection,
                Color.blue,
                1f
            );
        }

        public override void Tick(float deltaTime) => _elapsedTime += deltaTime;

        public override void ChangeState() {
            if (_elapsedTime >= _duration) {
                _machine.SetState(typeof(EnemyIdleState));
            }
        }
    }
}