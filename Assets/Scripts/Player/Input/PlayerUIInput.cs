using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Input {
    public class PlayerUIInput : MonoBehaviour, NewControls.IUIActions {
        private NewControls _actions;

        public NewControls.UIActions UIActions => _actions.UI;

        public event Action InteractEvent = delegate { };
        public event Action CancelInteractEvent = delegate { };
        public event Action CancelEvent = delegate { };
        public event Action OpenMenuEvent = delegate { };
        public event Action NextViewEvent = delegate { };
        public event Action PreviousViewEvent = delegate { };

        public bool IsGamepad => _isGamepad;

        private bool _isGamepad;

        private void Awake() {
            _actions ??= new NewControls();

            _actions.UI.SetCallbacks(this);
        }

        // Called from the Editor on the Player Input component
        public void ChangeControlScheme(UnityEngine.InputSystem.PlayerInput playerInput) {
            _isGamepad = playerInput.devices.Any(d => d is Gamepad);
        }

        private void OnEnable() {
            UIActions.Enable();
        }

        private void OnDisable() {
            UIActions.Disable();
        }

        public void OnNavigate(InputAction.CallbackContext context) { }

        public void OnSubmit(InputAction.CallbackContext context) {
            if (context.performed) {
                InteractEvent?.Invoke();
            }
            else if (context.canceled) {
                CancelInteractEvent?.Invoke();
            }
        }

        public void OnCancel(InputAction.CallbackContext context) {
            if (context.performed) {
                CancelEvent?.Invoke();
            }
        }

        public void OnClick(InputAction.CallbackContext context) {
            if (context.performed) {
                InteractEvent?.Invoke();
            }
            else if (context.canceled) {
                CancelInteractEvent?.Invoke();
            }
        }

        public void OnNextCategory(InputAction.CallbackContext context) {
            if (context.performed) {
                NextViewEvent?.Invoke();
            }
        }

        public void OnPreviousCategory(InputAction.CallbackContext context) {
            if (context.performed) {
                PreviousViewEvent?.Invoke();
            }
        }

        public void OnStart(InputAction.CallbackContext context) {
            if (context.performed) {
                OpenMenuEvent?.Invoke();
            }
        }
    }
}