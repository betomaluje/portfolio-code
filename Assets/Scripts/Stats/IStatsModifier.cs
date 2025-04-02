using System;

namespace Stats {
    public interface IStatsModifier {
        void SaveStats();
        void AddStatModifier(StatType type, float amount, float duration = 0);
        void ResetStatModifier(StatType type);

        event Action<StatType, float> OnStatModifierAdded;
        event Action<StatType, float> OnStatModifierReset;
    }

    [Serializable]
    public enum StatType {
        Speed,
        Attack,
        Defense,
        HealRate,
        CriticalHitChance,
        DamageOthers
    }
}