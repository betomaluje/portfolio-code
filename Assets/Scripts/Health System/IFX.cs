using System;

namespace BerserkPixel.Health.FX {
    public interface IFX {
        FXType GetFXType();

        FXLifetime LifetimeFX { get; }

        void DoFX(HitData hitData);
    }

    [Flags]
    public enum FXType {
        Always = 0,
        OnlyImmune = 1 << 0,
        OnlyNotImmune = 1 << 1,
    }

    public enum FXLifetime {
        Always = 0,
        OnlyAlive = 1,
        OnlyDead = 2,
    }
}