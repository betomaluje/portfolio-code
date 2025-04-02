using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

namespace Modifiers.Powerups {
    [CreateAssetMenu(menuName = "Aurora/Powerups/Heal Powerup")]
    [TypeInfoBox("Powerup that heals the player over time")]
    public class HealByTimePowerup : PowerupConfig {
        [BoxGroup("Timer")]
        [SerializeField]
        private float _interval = 1f;

        private NotifyingCountdownTimer _internalTimer;

        public override void Setup(Transform owner) {
            base.Setup(owner);
            _internalTimer = new NotifyingCountdownTimer(Duration, _interval);
            _internalTimer.OnInterval += IncreaseHealth;
            _internalTimer.OnTimerStop += ResetTimer;
        }

        public override void Activate(Transform target) {
            base.Activate(target);
            if (_internalTimer.IsRunning) {
                return;
            }

            _internalTimer.Start();
        }

        public override void Tick(float deltaTime) {
            base.Tick(deltaTime);
            _internalTimer.Tick(deltaTime);
        }

        private void IncreaseHealth() {
            _playerStatsManager.AddStatModifier(_statType, EndValue);
        }

        private void ResetTimer() {
            _internalTimer.Reset(Duration);
            _internalTimer.Start();
        }

        public override void Deactivate() {
            base.Deactivate();
            _internalTimer.Reset(Duration);
            _playerStatsManager.ResetStatModifier(_statType);
        }

        public override void Cleanup() {
            base.Cleanup();
            _internalTimer.OnInterval -= IncreaseHealth;
            _internalTimer.OnTimerStop -= ResetTimer;
        }

        private void OnValidate() {
            _statType = Stats.StatType.HealRate;
        }
    }
}