using UnityEngine;

namespace Companions.States {
    [CreateAssetMenu(menuName = "Aurora/Companions/States/Sleep")]
    public class CompanionSleepState : BaseCompanionState {
        [SerializeField]
        [Min(0f)]
        private float _minTime;

        [SerializeField]
        private float _maxTime = 10f;

        private float _elapsedTime;
        private float _waitingTime;

        public override void Enter(CompanionStateMachine parent) {
            base.Enter(parent);

            parent.Animations.Play("Sleep");
            parent.Sounds.PlaySleep();
            parent.Movement.Stop();
            parent.MakeBodyKinematic();

            _elapsedTime = 0f;
            _waitingTime = Random.Range(_minTime, _maxTime);
        }

        public override void Tick(float deltaTime) {
            base.Tick(deltaTime);
            _elapsedTime += deltaTime;
        }

        public override void ChangeState() {
            if (_elapsedTime >= _waitingTime) {
                _machine.SetState(typeof(CompanionIdleState));
                return;
            }
        }
    }
}