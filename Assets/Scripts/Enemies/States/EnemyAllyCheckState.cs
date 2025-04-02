using BerserkPixel.StateMachine;
using Sounds;
using UnityEngine;

namespace Enemies.States {
    // Created by code from EnemyStateMachine
    public class EnemyAllyCheckState : State<EnemyStateMachine> {
        [SerializeField]
        [Range(0, 20)]
        private float _speed = 16f;

        private float _maxTimeToDetect = 5f;
        private float _minDistance = .8f;
        private Transform _target;
        private Vector2 _direction;

        private float _elapsedTime;
        private float _distance;

        override public void Enter(EnemyStateMachine parent) {
            base.Enter(parent);

            // SoundManager.instance.PlayWithPitch("enemy_chase");
            _machine.Movement.Stop();

            if (_target == null) {
                _machine.SetState(typeof(EnemyIdleState));
                return;
            }

            _elapsedTime = 0f;
            _distance = 1000f;
        }

        public override void Tick(float deltaTime) {
            _elapsedTime += deltaTime;

            if (_target == null) {
                _elapsedTime = _maxTimeToDetect;
                return;
            }

            _distance = Vector3.Distance(_machine.transform.position, _target.position);
            // if (_distance > _minDistance) {
            _direction = (_target.position - _machine.transform.position).normalized;

            _machine.Movement.Move(_speed * _direction);
            _machine.Movement.FlipSprite(_direction);
            // }
        }

        public override void ChangeState() {
            // if we reached our target
            if (_distance <= _minDistance) {
                // tell state machine to do something about allies
                if (_machine.AllyTargetReached(_target)) {
                    // change of state is handled by the ActionDecorator
                    return;
                }

                _machine.SetState(typeof(EnemyIdleState));
                return;
            }

            if (_elapsedTime >= _maxTimeToDetect) {
                _machine.SetState(typeof(EnemyIdleState));
            }
        }

        public class Builder {
            Transform target = default;
            float maxTimeToDetect = 5f;
            float minDistance = .8f;
            float speed = 16f;

            public Builder WithTarget(Transform t) {
                target = t;
                return this;
            }

            public Builder WithMaxTimeToDetect(float t) {
                maxTimeToDetect = t;
                return this;
            }

            public Builder WithMinDistance(float t) {
                minDistance = t;
                return this;
            }

            public Builder WithSpeed(float t) {
                speed = t;
                return this;
            }

            public State<EnemyStateMachine> Build() {
                var state = CreateInstance<EnemyAllyCheckState>();
                state._target = target;
                state._maxTimeToDetect = maxTimeToDetect;
                state._minDistance = minDistance;
                state._speed = speed;
                return state;
            }
        }
    }

}