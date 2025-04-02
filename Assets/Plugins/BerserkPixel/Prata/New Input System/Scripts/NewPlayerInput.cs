using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Project.Scripts.Player
{
    public class NewPlayerInput : MonoBehaviour, PrataNewControls.IPlayerActions, PrataNewControls.IUIActions
    {
        // Shared


        // Gameplay
        public event UnityAction<Vector2> moveEvent = delegate { };
        public event UnityAction jumpEvent = delegate { };
        public event UnityAction jumpCanceledEvent = delegate { };
        public event UnityAction talkEvent = delegate { };
        public event UnityAction climbEvent = delegate { };

        // Dialogues
        public event UnityAction<Vector2> navigateEvent = delegate { };
        public event UnityAction cancelEvent = delegate { };
        public event UnityAction interactEvent = delegate { };

        private PrataNewControls gameInput;

        private void OnEnable()
        {
            if (gameInput == null)
            {
                gameInput = new PrataNewControls();
                gameInput.Player.SetCallbacks(this);
                gameInput.UI.SetCallbacks(this);
            }

            EnableGameplayInput();
        }

        private void OnDisable()
        {
            DisableAllInput();
        }

        public void DisableAllInput()
        {
            gameInput.Player.Disable();
            gameInput.UI.Disable();
        }

        public void EnableGameplayInput()
        {
            if (gameInput.Player.enabled) return;

            gameInput.UI.Disable();

            gameInput.Player.Enable();
        }

        public void EnableDialogueInput()
        {
            if (gameInput.UI.enabled) return;

            gameInput.Player.Disable();

            gameInput.UI.Enable();
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            moveEvent.Invoke(context.ReadValue<Vector2>());
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                jumpEvent.Invoke();

            if (context.phase == InputActionPhase.Canceled)
                jumpCanceledEvent.Invoke();
        }

        public void OnClimb(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                climbEvent.Invoke();
        }

        public void OnTalk(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                talkEvent.Invoke();
        }

        public void OnNavigate(InputAction.CallbackContext context)
        {
            navigateEvent.Invoke(context.ReadValue<Vector2>());
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                cancelEvent.Invoke();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                interactEvent.Invoke();
        }
    }
}