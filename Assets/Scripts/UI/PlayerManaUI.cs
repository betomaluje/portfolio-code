using Modifiers.Skills;

namespace UI {
    public class PlayerManaUI : ModifierUI<ICharacterSkills> {
        private void OnEnable() {
            if (_playerModifierController != null) {
                _playerModifierController.OnSkillEquipped += HandleSkillEquipped;
                _playerModifierController.OnSkillActivated += HandleSkillActivated;
            }
        }

        private void OnDisable() {
            if (_playerModifierController != null) {
                _playerModifierController.OnSkillEquipped -= HandleSkillEquipped;
                _playerModifierController.OnSkillActivated -= HandleSkillActivated;
            }
        }

        private void HandleSkillEquipped(SkillConfig config) {
            _modifierImage.sprite = config.Icon;
            _modifierImage.raycastTarget = false;
            _modifierImage.preserveAspect = true;

            ToggleModifierColors(true);
            _progressBar.ResetBar(true, 1);
        }

        private void HandleSkillActivated(SkillConfig config) {
            // we are not handling indefinite powerups
            // TODO: Handle multiple skills

            DoDurationProgress(config.Duration);
        }
    }

}