using BerserkPixel.StateMachine;
using Sounds;
using UnityEngine;

namespace Player.States {
    [CreateAssetMenu(menuName = "Aurora/Player/States/Hit")]
    internal class HitState : State<PlayerStateMachine> {
        [SerializeField]
        private float _timeToRecover = .8f;

        [SerializeField]
        private Transform[] _hitParticles;

        [SerializeField]
        [Range(0f, 1f)]
        private float _chanceOfLoosingCoins = .2f;

        private float _elapsedTime;
        private float _selectedChance;

        public override void Enter(PlayerStateMachine parent) {
            base.Enter(parent);
            SoundManager.instance.Play("hit");
            SpawnHitParticles();
            _elapsedTime = 0f;
            _selectedChance = Random.value;
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
            if (_selectedChance < _chanceOfLoosingCoins) {
                _machine.SetState(typeof(LoseCoinsState));
                return;
            }

            if (_machine.IsMoving) {
                _machine.SetState(typeof(MoveState));
                return;
            }

            if (_elapsedTime >= _timeToRecover) {
                _machine.SetState(typeof(IdleState));
            }
        }
    }
}