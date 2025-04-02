using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI {
    public class ControllerMenuUI : MonoBehaviour {
        [SerializeField]
        private GameObject _controllerMenu;

        [SerializeField]
        private GameObject _keyboardMenu;

        // Called from the Editor on the Player Input component
        public void ChangeControlScheme(PlayerInput playerInput) {
            if (playerInput.devices.Any(d => d is Gamepad)) {
                _controllerMenu.SetActive(true);
                _keyboardMenu.SetActive(false);
            }
            else {
                _controllerMenu.SetActive(false);
                _keyboardMenu.SetActive(true);
            }
        }
    }
}