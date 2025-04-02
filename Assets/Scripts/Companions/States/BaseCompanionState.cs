using System.Linq;
using BerserkPixel.StateMachine;
using Detection;
using Extensions;
using UnityEngine;

namespace Companions.States {
    public abstract class BaseCompanionState : State<CompanionStateMachine> {
        [SerializeField]
        [Min(0f)]
        private float _detectTime = 1.5f;

        protected TargetDetection _enemyDetection;

        private float _detectElapsedTime;

        public override void Enter(CompanionStateMachine parent) {
            base.Enter(parent);
            if (_enemyDetection == null) {
                var allDetections = GetComponentsInChildren<TargetDetection>();
                _enemyDetection = allDetections.FirstOrDefault(x => x.TargetType == TargetType.Enemy);
            }

            DetectEnemies(parent);
            _detectElapsedTime = 0f;
        }

        private void DetectEnemies(CompanionStateMachine parent) {
            if (_enemyDetection != null && _enemyDetection.GetTarget() && _machine.CurrentState != typeof(CompanionScaredState)) {
                parent.SetState(typeof(CompanionScaredState));
            }
        }

        public override void Tick(float deltaTime) {
            base.Tick(deltaTime);
            _detectElapsedTime += deltaTime;
            if (_detectElapsedTime > _detectTime) {
                DetectEnemies(_machine);
                _detectElapsedTime = 0f;
            }
        }
    }
}