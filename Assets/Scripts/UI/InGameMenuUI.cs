using DG.Tweening;
using Player.Input;
using Scene_Management;
using Sounds;
using UI.Options;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI {
    public class InGameMenuUI : MonoBehaviour {
        [Header("Background")]
        [SerializeField]
        private Image _panelBackround;

        [SerializeField]
        [Range(0f, 1f)]
        private float _bgFinalAlpha = .8f;

        [SerializeField]
        [Min(0f)]
        private float _bgFadeTime = .5f;

        [SerializeField]
        [Range(0f, 1f)]
        private float _bgDelayTime = .3f;

        [Header("Main Container")]
        [SerializeField]
        private PlayerUIInput _playerInput;

        [SerializeField]
        private GameObject _inGameMenu;

        [SerializeField]
        private GameObject _firstSelected;

        [Header("Settings")]
        [SerializeField]
        private OptionsMenuUI _inGameSettings;

        [Header("Scene Management")]
        [SerializeField]
        private SceneField _villageScene;

        [SerializeField]
        private SceneField _mainMenuScene;

        private PlayerBattleInput _input;

        private void Awake() {
            _input = FindFirstObjectByType<PlayerBattleInput>();
            _panelBackround.gameObject.SetActive(true);
            _panelBackround.DOFade(0, 0).SetUpdate(true).OnComplete(() => _panelBackround.gameObject.SetActive(false));
        }

        private void Start() {
            _inGameMenu.SetActive(false);
            _inGameSettings.gameObject.SetActive(false);
        }

        private void OnValidate() {
            if (TryGetComponent(out Canvas canvas) && canvas.renderMode == RenderMode.ScreenSpaceCamera) {
                canvas.worldCamera = UnityEngine.Camera.main;
            }
        }

        /// <summary>
        /// Called from EditorTool when an object is placed on the grid
        /// </summary>
        /// <param name="placedObject">The Object that was placed</param>
        public void OnObjectPlaced(GameObject placedObject) {
            if (placedObject.CompareTag("Player")) {
                _input = placedObject.GetComponent<PlayerBattleInput>();
                _input.OpenMenuEvent += ToggleInGameMenu;
            }
        }

        private void OnEnable() {
            if (_input != null) {
                _input.OpenMenuEvent += ToggleInGameMenu;
            }

            _playerInput.CancelEvent += CancelInGameMenu;
        }

        private void OnDisable() {
            if (_input != null) {
                _input.OpenMenuEvent -= ToggleInGameMenu;
            }
            _playerInput.CancelEvent -= CancelInGameMenu;
        }

        private void CancelInGameMenu() {
            _inGameMenu.SetActive(false);
            _inGameSettings.gameObject.SetActive(false);
            _panelBackround.DOFade(0, _bgFadeTime).SetUpdate(true).OnComplete(() => _panelBackround.gameObject.SetActive(false));
            Time.timeScale = 1;
            if (_input != null) {
                _input.BattleActions.Enable();
            }
        }

        private void ToggleInGameMenu() {
            var alreadyThere = _inGameMenu.activeInHierarchy;
            _inGameMenu.SetActive(!alreadyThere);

            if (_inGameMenu.activeInHierarchy) {
                SoundManager.instance.Play("menu_open");
                EventSystem.current.firstSelectedGameObject = _firstSelected;
                EventSystem.current.SetSelectedGameObject(_firstSelected);

                _panelBackround.gameObject.SetActive(true);
                _panelBackround.DOFade(_bgFinalAlpha, _bgFadeTime).SetDelay(_bgDelayTime).SetUpdate(true);

                Time.timeScale = 0;
                _input.BattleActions.Disable();
            }
            else {
                CancelInGameMenu();
            }
        }

        #region Button Callbacks

        public void Click_ResumeGame() {
            CancelInGameMenu();
        }

        public void Click_Options() {
            EventSystem.current.SetSelectedGameObject(null);
            _inGameMenu.SetActive(false);
            _inGameSettings.gameObject.SetActive(true);

            _inGameSettings.ActivateFirstTime();
        }

        public void Click_Previous() {
            EventSystem.current.SetSelectedGameObject(null);
            _inGameSettings.gameObject.SetActive(false);
            _inGameMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(_firstSelected);
        }

        public void Click_BackToVillage() {
            Time.timeScale = 1;
            SceneLoader.Instance.LoadScene(_villageScene);
        }

        public void Click_BackToMenu() {
            Time.timeScale = 1;
            SceneLoader.Instance.LoadScene(_mainMenuScene);
        }

        #endregion
    }
}
