namespace Stats {
    public interface IStatFX {
        StatType StatType { get; }

        void DoFX(StatType type, float amount);

        void ResetFX(StatType type, float amount);
    }
}