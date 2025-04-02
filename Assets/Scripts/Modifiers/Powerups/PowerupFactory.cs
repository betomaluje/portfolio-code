using Stats;
using UnityEngine;

namespace Modifiers.Powerups {
	public static class PowerupFactory {
		public static PowerupConfig CreatePowerup(StatType type) {
			return type switch {
				StatType.Speed => ScriptableObject.CreateInstance<SpeedPowerup>(),
				StatType.Attack => ScriptableObject.CreateInstance<AttackPowerup>(),
				StatType.Defense => ScriptableObject.CreateInstance<DefensePowerup>(),
				StatType.HealRate => ScriptableObject.CreateInstance<HealByTimePowerup>(),
				StatType.CriticalHitChance => ScriptableObject.CreateInstance<CriticalHitChancePowerup>(),
				StatType.DamageOthers => ScriptableObject.CreateInstance<DamageOthersPowerup>(),
				_ => null,
			};
		}
	}
}