using BerserkPixel.StateMachine;
using UnityEngine;

namespace NPCs.States {
    [CreateAssetMenu(menuName = "Aurora/NPC/States/Idle")]
    internal class NPCIdleState : State<NPCStateMachine> {
        [SerializeField]
        [Min(0f)]
        private float _minTime;

        [SerializeField]
        private float _maxTime = 5f;

        private float _elapsedTime;
        private float _waitingTime;

        public override void Enter(NPCStateMachine parent) {
            base.Enter(parent);
            parent.MakeBodyKinematic();
            parent.Animations.PlayIdle();
            parent.Movement.Stop();

            _elapsedTime = 0f;
            _waitingTime = Random.Range(_minTime, _maxTime);
        }

        public override void Tick(float deltaTime) => _elapsedTime += deltaTime;

        public override void ChangeState() {
            if (_elapsedTime < _waitingTime) {
                return;
            }

            if (_machine.HasBeenRescued) {
                _machine.SetState(typeof(NPCFollowPayerState));
                return;
            }

            _machine.SetState(typeof(NPCWanderState));
        }
    }
}