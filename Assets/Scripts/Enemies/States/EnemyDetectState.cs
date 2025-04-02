using System;
using BerserkPixel.StateMachine;
using UnityEngine;

namespace Enemies.States {
    [CreateAssetMenu(menuName = "Aurora/Enemy/States/Detect")]
    public class EnemyDetectState : State<EnemyStateMachine> {
        [SerializeField]
        private DetectType _nextState = DetectType.Chase;

        [SerializeField]
        private float _maxTimeToDetect = 1f;

        private float _elapsedTime;

        public override void Enter(EnemyStateMachine parent) {
            base.Enter(parent);
            parent.ExpressionManager.SetAlertExpression();
            _elapsedTime = 0f;
        }

        public override void Tick(float deltaTime) => _elapsedTime += deltaTime;

        public override void ChangeState() {
            if (_elapsedTime >= _maxTimeToDetect) {
                switch (_nextState) {
                    case DetectType.Chase:
                        _machine.SetState(typeof(EnemyChaseState));
                        break;
                    case DetectType.Attack:
                        _machine.SetState(typeof(EnemyAttackState));
                        break;
                    case DetectType.Allies:
                        _machine.SetState(typeof(EnemyAllyCheckState));
                        break;
                    case DetectType.Charge:
                        _machine.SetState(typeof(BossChargeTowardsTarget));
                        break;
                    default:
                        _machine.SetState(typeof(EnemyIdleState));
                        break;
                }
            }
        }
    }

    [Serializable]
    public enum DetectType {
        Chase,
        Attack,
        Allies,
        Charge
    }
}