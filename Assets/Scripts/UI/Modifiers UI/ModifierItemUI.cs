using Modifiers;
using Modifiers.Powerups;
using Modifiers.Skills;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI {
    public class ModifierItemUI : Selectable, IPointerClickHandler, ISubmitHandler {
        [SerializeField]
        private Image _background;

        [SerializeField]
        private Image _icon;

        private IModifierScreen _equippedModifiers;
        private IModifier _modifier;

#if UNITY_EDITOR
        protected override void OnValidate() {
            if (!TryGetComponent<Image>(out var imageComponent))
                imageComponent = gameObject.AddComponent<Image>();

            targetGraphic = imageComponent;
        }
#endif
        #region Mouse & Keyboard
        public void OnPointerClick(PointerEventData eventData) {
            DoStateTransition(SelectionState.Pressed, true);

            HandlingInput();

            DoStateTransition(currentSelectionState, false);
        }

        public override void OnPointerEnter(PointerEventData eventData) {
            base.OnPointerEnter(eventData);
            DoHover();
        }

        public override void OnPointerExit(PointerEventData eventData) {
            base.OnPointerExit(eventData);

            Undohover();
        }
        #endregion

        #region Gamepad
        public void OnSubmit(BaseEventData eventData) {
            HandlingInput();
        }

        public override void OnSelect(BaseEventData eventData) {
            base.OnSelect(eventData);
            DoHover();
        }

        public override void OnDeselect(BaseEventData eventData) {
            base.OnDeselect(eventData);
            Undohover();
        }
        #endregion

        #region Actions
        private void DoHover() {
            if (_modifier == null) {
                _equippedModifiers.HideModifierDetails();
                return;
            }

            _equippedModifiers.ShowModifierDetails(_modifier);
        }

        private void Undohover() {
            _equippedModifiers.HideModifierDetails();
        }

        private void HandlingInput() {
            if (_modifier == null) {
                _equippedModifiers.HideModifierDetails();
                return;
            }

            _equippedModifiers.SelectModifier(_modifier);
        }
        #endregion

        public void SetModifier(IModifierScreen equippedModifiers, IModifier modifier) {
            _equippedModifiers = equippedModifiers;

            if (modifier == null) {
                SetIcon(null);
                SetBackground(Color.clear);
                return;
            }

            _modifier = modifier;

            if (modifier is PowerupConfig powerup) {
                SetIcon(powerup.Icon);
                SetBackground(powerup.GetTagColor());

                return;
            }

            if (modifier is SkillConfig skill) {
                SetIcon(skill.Icon);
                SetBackground(skill.GetTagColor());

                return;
            }

            if (modifier is WeaponModifier weapon) {
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


    }
}