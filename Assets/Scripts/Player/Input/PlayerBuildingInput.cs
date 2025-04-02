using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Input {
    public class PlayerBuildingInput : MonoBehaviour, NewControls.IBuildingActions {
        private NewControls _actions;

        public NewControls.BuildingActions BuildingActions => _actions.Building;

        public event Action<Vector2> MovementEvent = delegate { };
        public event Action PlaceBuilding = delegate { };
        public event Action DeleteEvent = delegate { };
        public event Action NextCategoryEvent = delegate { };
        public event Action PreviousCategoryEvent = delegate { };
        public event Action OpenMenuEvent = delegate { };

        private void Awake() {
            _actions ??= new NewControls();

            _actions.Building.SetCallbacks(this);
        }

        private void OnEnable() {
            _actions.Building.Enable();
        }

        private void OnDisable() {
            _actions.Building.Disable();
        }

        public void OnMovement(InputAction.CallbackContext context) {
            MovementEvent?.Invoke(context.ReadValue<Vector2>().normalized);
        }

        public void OnNextCategory(InputAction.CallbackContext context) {
            if (context.performed) {
                NextCategoryEvent?.Invoke();
            }
        }

        public void OnNorth(InputAction.CallbackContext context) {
            if (context.performed) {
                // do something
            }
        }

        public void OnSouth(InputAction.CallbackContext context) {
            if (context.performed) {
                PlaceBuilding?.Invoke();
            }
        }

        public void OnEast(InputAction.CallbackContext context) {
        }

        public void OnWest(InputAction.CallbackContext context) {
            if (context.performed) {
                DeleteEvent?.Invoke();
            }
        }

        public void OnPreviousCategory(InputAction.CallbackContext context) {
            if (context.performed) {
                PreviousCategoryEvent?.Invoke();
            }
        }

        public void OnStart(InputAction.CallbackContext context) {
            if (context.performed) {
                OpenMenuEvent?.Invoke();
            }
        }
    }
}