using Sirenix.OdinInspector;
using UnityEngine;

namespace Modifiers.Powerups {
    [CreateAssetMenu(menuName = "Aurora/Powerups/CriticalHitChance Powerup")]
    [TypeInfoBox("Powerup that modifies the critical hit chance for a certain amount of time")]
    public class CriticalHitChancePowerup : PowerupConfig {
        public override void Activate(Transform target) {
            base.Activate(target);
            _playerStatsManager.AddStatModifier(_statType, EndValue);
        }

        public override void Deactivate() {
            base.Deactivate();
            _playerStatsManager.ResetStatModifier(_statType);
        }

        private void OnValidate() {
            _statType = Stats.StatType.CriticalHitChance;
        }
    }
}