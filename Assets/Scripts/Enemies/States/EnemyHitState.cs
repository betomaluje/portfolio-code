using BerserkPixel.StateMachine;
using Detection;
using Sounds;
using UnityEngine;

namespace Enemies.States {
    [CreateAssetMenu(menuName = "Aurora/Enemy/States/Hit")]
    public class EnemyHitState : State<EnemyStateMachine> {
        [SerializeField]
        private float _timeToRecover = .8f;

        [SerializeField]
        private Transform[] _hitParticles;

        private float _elapsedTime;

        private TargetDetection _enemyDetection;

        public override void Enter(EnemyStateMachine parent) {
            base.Enter(parent);

            if (_enemyDetection == null) {
                _enemyDetection = GetComponentInChildren<TargetDetection>();
            }

            SoundManager.instance.PlayWithPitch("enemy_hit");

            parent.MakeBodyDynamic();
            parent.Animations.PlayHurt();
            SpawnHitParticles();
            _elapsedTime = 0f;
        }

        private void SpawnHitParticles() {
            if (_hitParticles is not { Length: > 0 }) {
                return;
            }

            var index = Random.Range(0, _hitParticles.Length);
            Instantiate(_hitParticles[index], _machine.transform.position, Quaternion.identity);
        }

        public override void Tick(float deltaTime) => _elapsedTime += deltaTime;

        public override void ChangeState() {
            if (_elapsedTime >= _timeToRecover) {
                if (_machine.HasState(typeof(EnemyRandomWeightedStates))) {
                    _machine.SetState(typeof(EnemyRandomWeightedStates));
                }
                else {
                    _machine.SetState(typeof(EnemyAttackState));
                }
            }
        }
    }
}