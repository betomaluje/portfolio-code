using Sirenix.OdinInspector;
using UnityEngine;

namespace Modifiers.Powerups {
    [CreateAssetMenu(menuName = "Aurora/Powerups/Attack Powerup")]
    [TypeInfoBox("Powerup that modifies the attack of the player for a certain amount of time")]
    public class AttackPowerup : PowerupConfig {
        public override void Activate(Transform target) {
            base.Activate(target);
            _playerStatsManager.AddStatModifier(_statType, EndValue);
        }

        public override void Deactivate() {
            base.Deactivate();
            _playerStatsManager.ResetStatModifier(_statType);
        }

        private void OnValidate() {
            _statType = Stats.StatType.Attack;
        }
    }
}