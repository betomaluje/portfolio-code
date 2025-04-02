using Modifiers;
using Modifiers.Powerups;
using Modifiers.Skills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class ModifierDetailsUI : MonoBehaviour {
        [SerializeField]
        private Image _background;

        [SerializeField]
        private Image _icon;

        [SerializeField]
        private TextMeshProUGUI _name;

        [SerializeField]
        private TextMeshProUGUI _description;

        private void Awake() {
            HideModifierDetails();
        }

        private void SetModifier(IModifier modifier) {
            if (modifier == null) {
                HideModifierDetails();
                return;
            }

            if (modifier is PowerupConfig powerup) {
                _name.text = powerup.Name;
                _description.text = powerup.Description;

                SetIcon(powerup.Icon);
                SetBackground(powerup.GetTagColor());

                return;
            }

            if (modifier is SkillConfig skill) {
                _name.text = skill.Name;
                _description.text = skill.Description;

                SetIcon(skill.Icon);
                SetBackground(skill.GetTagColor());

                return;
            }

            if (modifier is WeaponModifier weapon) {
                _name.text = weapon.Name;
                _description.text = weapon.Description;

                SetIcon(weapon.Icon);
                SetBackground(weapon.GetTagColor());

                return;
            }
        }

        private void SetIcon(Sprite sprite) {
            if (sprite == null) {
                _icon.gameObject.SetActive(false);
                return;
            }

            _icon.gameObject.SetActive(true);
            _icon.sprite = sprite;
        }

        private void SetBackground(Color color) {
            if (color == Color.clear) {
                _background.gameObject.SetActive(false);
                return;
            }

            _background.gameObject.SetActive(true);
            _background.color = color;
        }

        public void HideModifierDetails() {
            _name.text = "";
            _description.text = "";

            SetIcon(null);

            SetBackground(Color.clear);
        }

        public void ShowModifierDetails(IModifier modifier) {
            SetModifier(modifier);
        }
    }
}