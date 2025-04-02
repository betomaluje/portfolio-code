using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Input {
    [DisallowMultipleComponent]
    public class PlayerBattleInput : MonoBehaviour, IPlayerInput, NewControls.IBattleActions {
        private NewControls _actions;

        public NewControls.BattleActions BattleActions => _actions.Battle;

        public event Action<Vector2> MovementEvent = delegate { };
        public event Action NorthButtonEvent = delegate { };
        public event Action<bool> SouthButtonEvent = delegate { };
        public event Action EastButtonEvent = delegate { };
        public event Action WestButtonEvent = delegate { };
        public event Action OpenMenuEvent = delegate { };
        public event Action NextWeaponEvent = delegate { };
        public event Action PreviousWeaponEvent = delegate { };

        public bool IsGamepad => _isGamepad;
        private bool _isGamepad;

        private void Awake() {
            _actions ??= new NewControls();

            _actions.Battle.SetCallbacks(this);
        }

        // Called from the Editor on Unity's PlayerInput component
        public void ChangeControlScheme(UnityEngine.InputSystem.PlayerInput playerInput) {
            _isGamepad = playerInput.devices.Any(d => d is Gamepad);
        }

        private void OnEnable() {
            BattleActions.Enable();
        }

        private void OnDisable() {
            BattleActions.Disable();
        }

        public Vector2 GetMoveDirection() => BattleActions.Movement.ReadValue<Vector2>().normalized;

        public Vector2 GetAimDirection() {
            if (!_isGamepad) {
                // get the mouse position
                var mousePosition = UnityEngine.Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                var dir = (mousePosition - transform.position).normalized;
                dir.z = 0;
                return dir;
            }
            else {
                return BattleActions.Movement.ReadValue<Vector2>().normalized;
            }
        }

        public void OnNorth(InputAction.CallbackContext context) {
            if (context.performed) {
                NorthButtonEvent?.Invoke();
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
                WestButtonEvent?.Invoke();
            }
        }

        public void OnMovement(InputAction.CallbackContext context) {
            MovementEvent?.Invoke(context.ReadValue<Vector2>().normalized);
        }

        public void OnNextCategory(InputAction.CallbackContext context) {
            if (context.performed) {
                NextWeaponEvent?.Invoke();
            }
        }

        public void OnPreviousCategory(InputAction.CallbackContext context) {
            if (context.performed) {
                PreviousWeaponEvent?.Invoke();
            }
        }

        public void OnStart(InputAction.CallbackContext context) {
            if (context.performed) {
                OpenMenuEvent?.Invoke();
            }
        }
    }
}