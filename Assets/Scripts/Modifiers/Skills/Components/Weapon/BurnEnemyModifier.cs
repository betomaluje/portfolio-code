using BerserkPixel.Health;
using UnityEngine;
using Utils;

namespace Modifiers.Skills.Components {
    [CreateAssetMenu(menuName = "Aurora/Skills/Weapons/Burn Modifier")]
    public class BurnEnemyModifier : WeaponModifier {
        [Header("Custom Config")]
        [SerializeField]
        private float _interval = 1f;

        [SerializeField]
        private int _healthImpact = 2;

        private NotifyingCountdownTimer _internalTimer;

        private Transform _owner;

        public override void Setup(Transform owner) {
            base.Setup(owner);
            _owner = owner;
            var duration = Indefinite ? MAX_DURATION : Duration;
            _internalTimer = new NotifyingCountdownTimer(duration, _interval);
            _internalTimer.OnInterval += DecreaseHealth;
        }

        private void DecreaseHealth() {
            var hitData = new HitDataBuilder()
                   .WithDamage(_healthImpact)
                   .WithDirection(Vector2.zero)
                   .Build(_owner, _owner);

            _target?.Health?.PerformDamage(hitData);
        }

        public override void Activate(Transform target) {
            base.Activate(target);
            _internalTimer.Start();
            _target?.Movement?.SetMovementInfluence(EndValue);
        }

        public override void Tick(float deltaTime) {
            base.Tick(deltaTime);
            _internalTimer.Tick(deltaTime);
        }

        public override void Deactivate() {
            base.Deactivate();
            _internalTimer.Reset();
            _internalTimer.OnInterval -= DecreaseHealth;
            _target?.Movement?.ResetMovementInfluence();
        }
    }
}