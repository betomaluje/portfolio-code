using BerserkPixel.StateMachine;
using Extensions;
using UnityEngine;

namespace Player.States {
    [CreateAssetMenu(menuName = "Aurora/Player/States/LoseCoins")]
    public class LoseCoinsState : State<PlayerStateMachine> {
        [SerializeField]
        private ParticleSystem _coinParticles;

        [SerializeField]
        [Range(1, 100)]
        private int _coinsToLose = 10;

        override public void Enter(PlayerStateMachine parent) {
            base.Enter(parent);
            if (_coinParticles != null) {
                ParticleSystem coinParticles = Instantiate(_coinParticles, parent.transform.position, Quaternion.identity);
                var direction = new Vector2(-parent.Movement.LastX, 0);

                coinParticles.transform.RotateTo(direction);
                coinParticles.Play();
            }

            if (parent.TryGetComponent(out PlayerMoneyManager moneyManager)) {
                moneyManager.TakeAmount(_coinsToLose);
            }
        }

        public override void ChangeState() {
            _machine.SetState(typeof(IdleState));
        }
    }
}