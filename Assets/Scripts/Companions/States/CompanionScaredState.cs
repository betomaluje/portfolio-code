using BerserkPixel.StateMachine;
using Detection;
using Extensions;
using UnityEngine;

namespace Companions.States {
    [CreateAssetMenu(menuName = "Aurora/Companions/States/Scared")]
    public class CompanionScaredState : State<CompanionStateMachine> {
        [SerializeField]
        [Min(0f)]
        private float _minTime;

        private float _elapsedTime;

        private TargetDetection _enemyDetection;

        public override void Enter(CompanionStateMachine parent) {
            base.Enter(parent);

            _enemyDetection = this.GetEnemy<State<CompanionStateMachine>, CompanionStateMachine>();

            if (_enemyDetection == null) {
                parent.SetState(typeof(CompanionIdleState));
                return;
            }

            if (_enemyDetection.Target != null) {
                var direction = (_enemyDetection.Target.position - parent.transform.position).normalized;
                parent.Movement.FlipSprite(direction);
            }

            parent.Animations.Play("Scared");
            parent.Movement.Stop();

            _elapsedTime = 0;
        }

        public override void Tick(float deltaTime) => _elapsedTime += deltaTime;

        public override void ChangeState() {
            if (_elapsedTime >= _minTime) {
                if (_enemyDetection != null) {
                    _machine.SetState(typeof(CompanionChaseEnemyState));
                }
                else {
                    _machine.SetState(typeof(CompanionFollowState));
                }
            }
        }
    }
}