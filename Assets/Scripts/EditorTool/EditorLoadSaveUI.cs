using EditorTool.PlayerEditor;
using Player.Input;
using Scene_Management;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EditorTool {
    public class EditorLoadSaveUI : MonoBehaviour {
        [SerializeField]
        private GameObject _firstButton;

        [SerializeField]
        private Transform _container;

        [SerializeField]
        private PlayerEditorInput _playerEditorInput;

        [SerializeField]
        private PlayerUIInput _playerInput;

        [SerializeField]
        private PlayerEditorTool _playerEditorTool;

        [SerializeField]
        private SceneField _mainMenuScene;

        private void Awake() {
            Hide();
        }

        private void OnEnable() {
            _playerEditorInput.OpenMenuEvent += ToggleMenu;
            _playerInput.CancelEvent += Hide;
        }

        private void OnDisable() {
            _playerEditorInput.OpenMenuEvent -= ToggleMenu;
            _playerInput.CancelEvent -= Hide;
        }

        public void ToggleMenu() {
            _container.gameObject.SetActive(!_container.gameObject.activeInHierarchy);
            _playerEditorTool.enabled = !_container.gameObject.activeInHierarchy;

            if (_container.gameObject.activeInHierarchy) {
                EventSystem.current.SetSelectedGameObject(_firstButton);
            }
        }

        public void Show() {
            _container.gameObject.SetActive(true);
            _playerEditorTool.enabled = false;
        }

        public void Hide() {
            _container.gameObject.SetActive(false);
            _playerEditorTool.enabled = true;
        }

        public void Click_BackToMenu() {
            Time.timeScale = 1;
            SceneLoader.Instance.LoadScene(_mainMenuScene);
        }
    }
}