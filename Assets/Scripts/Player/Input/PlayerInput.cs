using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Input {
    [DisallowMultipleComponent]
    public class PlayerInput : MonoBehaviour, IPlayerInput, NewControls.IPlayerActions {
        private NewControls _actions;

        public NewControls.PlayerActions PlayerActions => _actions.Player;

        public event Action<Vector2> MovementEvent = delegate { };
        public event Action NorthButtonEvent = delegate { };
        public event Action<bool> SouthButtonEvent = delegate { };
        public event Action EastButtonEvent = delegate { };
        public event Action WestButtonEvent = delegate { };
        public event Action OpenMenuEvent = delegate { };

        public bool IsGamepad => _isGamepad;
        private bool _isGamepad;

        private void Awake() {
            _actions ??= new NewControls();

            _actions.Player.SetCallbacks(this);
        }

        // Called from the Editor on the Player Input component
        public void ChangeControlScheme(UnityEngine.InputSystem.PlayerInput playerInput) {
            _isGamepad = playerInput.devices.Any(d => d is Gamepad);
        }

        private void OnEnable() {
            PlayerActions.Enable();
        }

        private void OnDisable() {
            PlayerActions.Disable();
        }

        public Vector2 GetMoveDirection() => PlayerActions.Movement.ReadValue<Vector2>().normalized;

        public Vector2 GetAimDirection() {
            if (!_isGamepad) {
                // get the mouse position and convert it to a direction vector
                Vector2 mousePosition = Mouse.current.position.ReadValue();
                Vector2 direction = Vector2.zero;
                if (mousePosition.x > Screen.width / 2) {
                    direction.x = 1;
                }
                else if (mousePosition.x < Screen.width / 2) {
                    direction.x = -1;
                }
                if (mousePosition.y > Screen.height / 2) {
                    direction.y = 1;
                }
                else if (mousePosition.y < Screen.height / 2) {
                    direction.y = -1;
                }
                return direction.normalized;
            }
            else {
                return PlayerActions.Movement.ReadValue<Vector2>().normalized;
            }
        }

        /// <summary>
        ///     We disable UI controls and enable player input
        /// </summary>
        public void EnableGameplayInput() {
            if (_actions.Player.enabled) {
                return;
            }

            _actions.Player.Enable();
        }

        #region Input Callbacks

        public void OnMovement(InputAction.CallbackContext context) {
            MovementEvent?.Invoke(context.ReadValue<Vector2>().normalized);
        }

        public void OnNorth(InputAction.CallbackContext context) {
            if (context.performed) {
                // do something
            }
        }

        public void OnSouth(InputAction.CallbackContext context) {
            if (context.performed) {
                SouthButtonEvent?.Invoke(true);
            }
            else if (context.canceled) {
                SouthButtonEvent?.Invoke(false);
            }
        }

        public void OnEast(InputAction.CallbackContext context) {
            if (context.performed) {
                EastButtonEvent?.Invoke();
            }
        }

        public void OnWest(InputAction.CallbackContext context) {
            if (context.performed) {
                NorthButtonEvent?.Invoke();
            }
        }

        public void OnStart(InputAction.CallbackContext context) {
            if (context.performed) {
                OpenMenuEvent?.Invoke();
            }
        }

        #endregion
    }
}