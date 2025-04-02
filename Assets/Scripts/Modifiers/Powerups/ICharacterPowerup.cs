using System;

namespace Modifiers.Powerups {
    public interface ICharacterPowerup {
        public event Action<PowerupConfig> OnPowerupEquipped;
        public event Action<PowerupConfig> OnPowerupActivated;
        public event Action<PowerupConfig> OnPowerupDeactivated;
    }
}