using BerserkPixel.Health.FX;
using BerserkPixel.StateMachine;
using Detection;
using Extensions;
using UnityEngine;

namespace Enemies.States {
    [CreateAssetMenu(menuName = "Aurora/Enemy/States/Block")]
    public class EnemyBlockState : State<EnemyStateMachine> {
        [SerializeField]
        private float _blockCooldown = .8f;

        [SerializeField]
        private float _knockbackForce = 300f;

        private float _elapsedTime;
        private TargetDetection _enemyDetection;

        public override void Enter(EnemyStateMachine parent) {
            base.Enter(parent);
            if (_enemyDetection == null) {
                _enemyDetection = GetComponentInChildren<TargetDetection>();
            }

            // TODO: Add block sound
            parent.Block();
            parent.Animations.PlayBlock();

            if (parent.gameObject.FindInChildren<EnemyBlockFX>(out var blockParticles)) {
                blockParticles.DoFX(default);
            }

            var direction = (_enemyDetection.GetTargetPosition - parent.transform.position).normalized;

            if (parent.gameObject.FindInChildren<KnockbackFX>(out var knockbackFX)) {
                knockbackFX.DoFX(_knockbackForce, -direction);
            }

            parent.Movement.FlipSprite(direction);

            _elapsedTime = 0f;
        }

        public override void Tick(float deltaTime) => _elapsedTime += deltaTime;

        public override void ChangeState() {
            if (_elapsedTime < _blockCooldown) {
                return;
            }

            _machine.SetState(typeof(EnemyDetectState));
        }

        override public void Exit() {
            base.Exit();
            _machine.UnBlock();
        }
    }
}