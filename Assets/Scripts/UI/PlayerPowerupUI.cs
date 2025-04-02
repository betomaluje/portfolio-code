using Modifiers.Powerups;

namespace UI {
    public class PlayerPowerupUI : ModifierUI<ICharacterPowerup> {
        private int _lastUsedPowerupIndex = -1;

        private void OnEnable() {
            if (_playerModifierController != null) {
                _playerModifierController.OnPowerupEquipped += HandlePowerupEquipped;
                _playerModifierController.OnPowerupActivated += HandlePowerupActivated;
            }
        }

        private void OnDisable() {
            if (_playerModifierController != null) {
                _playerModifierController.OnPowerupEquipped -= HandlePowerupEquipped;
                _playerModifierController.OnPowerupActivated -= HandlePowerupActivated;
            }
        }

        private void HandlePowerupEquipped(PowerupConfig config) {
            if (config == null || _lastUsedPowerupIndex == config.ID) {
                return;
            }

            _lastUsedPowerupIndex = config.ID;

            UpdatePowerupIcon(config);

            ToggleModifierColors(true);

            _progressBar.ResetBar(true, 1);
        }

        private void HandlePowerupActivated(PowerupConfig config) {
            // we are not handling indefinite powerups
            // TODO: Handle multiple skills
            UpdatePowerupIcon(config);

            DoDurationProgress(config.Cooldown);
        }


        private void UpdatePowerupIcon(PowerupConfig config) {
            _modifierImage.sprite = config.Icon;
            _modifierImage.raycastTarget = false;
            _modifierImage.preserveAspect = true;
        }
    }
}