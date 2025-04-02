using UnityEngine;

namespace Companions.States {
    [CreateAssetMenu(menuName = "Aurora/Companions/States/Idle")]
    public class CompanionIdleState : BaseCompanionState {
        [SerializeField]
        [Min(0f)]
        private float _minTime;

        [SerializeField]
        private float _maxTime = 10f;

        [SerializeField]
        [Range(0f, 1f)]
        private float _chanceOfWander = .5f;

        private float _elapsedTime;
        private float _waitingTime;

        public override void Enter(CompanionStateMachine parent) {
            base.Enter(parent);

            parent.Animations.PlayIdle();
            parent.Movement.Stop();
            parent.MakeBodyKinematic();

            if (!parent.IsWild()) {
                _chanceOfWander = 0f;
            }

            _elapsedTime = 0f;
            _waitingTime = Random.Range(_minTime, _maxTime);
        }

        public override void Tick(float deltaTime) {
            base.Tick(deltaTime);
            _elapsedTime += deltaTime;
        }

        public override void ChangeState() {
            if (_elapsedTime >= _waitingTime) {
                if (Random.value <= _chanceOfWander) {
                    _machine.SetState(typeof(CompanionWanderState));
                }
                else {
                    _machine.SetState(typeof(CompanionSleepState));
                }
            }
        }
    }
}