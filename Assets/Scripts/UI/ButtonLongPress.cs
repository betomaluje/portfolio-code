using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI {
    public class ButtonLongPress : Selectable, ISubmitHandler {
        [SerializeField]
        private float _holdTime = 1.5f; // Time required to trigger long press

        [SerializeField]
        private UnityEvent<float> OnPressPercentage;

        [SerializeField]
        private UnityEvent OnLongPress;

        private bool _isPressing = false;
        private float _pressDuration = 0f;
        private bool _isGamePad;

        protected override void Start() {
            base.Start();
            _isPressing = false;
            _isGamePad = false;
            _pressDuration = 0f;
            OnPressPercentage?.Invoke(0);
        }

        private bool IsSelected() {
            return EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject;
        }

        #region Mouse & Keyboard
        public override void OnPointerDown(PointerEventData eventData) {
            _isGamePad = false;
            _pressDuration = 0f;
            _isPressing = true;
        }

        public override void OnPointerUp(PointerEventData eventData) {
            ResetPress();
        }

        #endregion

        #region Gamepad
        public void OnSubmit(BaseEventData eventData) {
            if (IsSelected()) {
                _isGamePad = true;
                _pressDuration = 0f;
                _isPressing = true;
            }
        }
        #endregion

        private void Update() {
            if (_isPressing) {

                _pressDuration += Time.unscaledDeltaTime;

                // hack: to avoid long press when gamepad button is released
                if (_isGamePad && !Gamepad.current.buttonSouth.isPressed) {
                    ResetPress();
                    return;
                }

                OnPressPercentage?.Invoke(_pressDuration / _holdTime);

                if (_pressDuration >= _holdTime) {
                    OnLongPress?.Invoke();
                    ResetPress();
                }
            }
        }

        private void ResetPress() {
            if (_pressDuration > 0f) {
                _isGamePad = false;
                _isPressing = false;
                _pressDuration = 0f;
                OnPressPercentage?.Invoke(0);
            }
        }
    }
}