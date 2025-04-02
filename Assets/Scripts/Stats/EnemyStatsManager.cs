using System;

namespace Stats {
    public class EnemyStatsManager : BaseStatsManager {
        public override event Action<StatType, float> OnStatModifierReset = delegate { };

        public override void ResetSpeed() {
            if (_characterHolder.Movement == null) {
                return;
            }

            _characterHolder.Movement.ResetMovementInfluence();
            OnStatModifierReset?.Invoke(StatType.Speed, 1);
        }

        public override void ResetAttack() {
            if (_characterHolder.WeaponManager == null) {
                return;
            }
            _characterHolder.WeaponManager.ResetStrengthInfluence();
            OnStatModifierReset?.Invoke(StatType.Attack, 1);
        }

        public override void ResetDefense() {
            OnStatModifierReset?.Invoke(StatType.Defense, 1);
        }

        public override void ResetHealRate() {
            if (_characterHolder.Health == null) {
                return;
            }
            _characterHolder?.Health?.GiveHealth(0);
            OnStatModifierReset?.Invoke(StatType.HealRate, 0);
        }

        public override void ResetCriticalHitChance() {
            OnStatModifierReset?.Invoke(StatType.CriticalHitChance, 0);
        }
    }
}