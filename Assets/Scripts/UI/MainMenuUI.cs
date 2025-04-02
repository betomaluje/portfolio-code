using DG.Tweening;
using Scene_Management;
using Sirenix.OdinInspector;
using Sounds;
using UI.Options;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;
using Utils;

namespace UI {
    public class MainMenuUI : MonoBehaviour {
        [SerializeField]
        private SceneField _introScene;

        [SerializeField]
        private SceneField _villageScene;

        [SerializeField]
        private SceneField _editScene;

        [Header("Animation")]
        [SerializeField]
        private RectTransform _titleText;

        [SerializeField]
        private CanvasGroup _toHide;

        [SerializeField]
        private GameObject _firstButton;

        [SerializeField]
        private CanvasGroup _mainMenuCanvasGroup;

        [Header("Containers")]
        [SerializeField]
        private RectTransform _mainContainer;

        [SerializeField]
        private OptionsMenuUI _settingsContainer;

        [SerializeField]
        private RectTransform _controllerContainer;

        [Header("Wishlist")]
        [SerializeField]
        private bool _canWishlist;

        [ShowIf("_canWishlist")]
        [SerializeField]
        private Selectable _wishlistButton;

        private PreferencesStorage _preferencesStorage = new();

        #region Internet check
        private NotifyingCountdownTimer _timer;
        #endregion

        private void Awake() {
            _mainMenuCanvasGroup.alpha = 0f;
            _toHide.alpha = 0f;
            _firstButton.GetComponent<Selectable>().enabled = false;
            _titleText.anchoredPosition = new Vector2(0, 400);

            _timer = new NotifyingCountdownTimer(60, 10);
        }

        private void Start() {
            RestoreLastResolution();

            var firstTime = _preferencesStorage.GetFirstTime();

            _mainContainer.gameObject.SetActive(true);
            _settingsContainer.gameObject.SetActive(false);
            _controllerContainer.gameObject.SetActive(false);

            Sequence sequence = DOTween.Sequence();
            sequence.Append(_titleText.DOAnchorPosY(0, firstTime ? 1f : .5f));
            sequence.Append(_toHide.DOFade(1, .5f));
            sequence.SetEase(Ease.OutCubic);
            sequence.OnComplete(() => {
                if (_toHide.TryGetComponent<RectTransform>(out var rectTransform)) {
                    rectTransform.DOScale(1.1f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine).SetId("title_scale");
                }

                InputSystem.onAnyButtonPress.CallOnce(ctrl => {
                    _toHide.alpha = 0f;

                    DOTween.Kill("title_scale");

                    if (_canWishlist) {
                        _timer.Start();
                    }

                    SoundManager.instance.Play("button_click");
                    Sequence sequence = DOTween.Sequence();
                    sequence.Append(_toHide.DOFade(0, .5f));
                    sequence.Append(_mainMenuCanvasGroup.DOFade(1, .5f).SetEase(Ease.OutCubic));
                    sequence.OnStart(() => {
                        _toHide.gameObject.SetActive(false);
                        _firstButton.GetComponent<Selectable>().enabled = true;
                        EventSystem.current.SetSelectedGameObject(_firstButton);
                    });
                    sequence.Play();
                });
            });
            sequence.SetDelay(firstTime ? 2f : .8f);
            sequence.Play();
        }

        private void OnEnable() {
            _timer.OnInterval += OnTimerInterval;
            _timer.OnTimerStop += OnTimerStop;
        }

        private void OnDisable() {
            _timer.OnInterval -= OnTimerInterval;
            _timer.OnTimerStop -= OnTimerStop;
        }

        private void Update() {
            _timer.Tick(Time.deltaTime);
        }

        private void OnDestroy() {
            DOTween.KillAll();
            _timer.Stop();
        }

        private void OnTimerStop() {
            _timer.Reset();
            _timer.Start();
        }

        private async void OnTimerInterval() {
            bool hasInternet = await InternetConnection.HasInternet();

            _wishlistButton.gameObject.SetActive(hasInternet);
        }

        private void RestoreLastResolution() {
            var windowed = ResolutionSingleton.Instance.GetPrefsWindowed();
            var (width, height) = ResolutionSingleton.Instance.GetPrefsResolution();
            ResolutionSingleton.Instance.SetResolution(width, height, windowed);
        }

        public void Click_Play() {
            var firstTime = _preferencesStorage.GetFirstTime();
            if (firstTime) {
                SceneLoader.Instance.LoadScene(_introScene);
            }
            else {
                SceneLoader.Instance.LoadScene(_villageScene);
            }
        }

        public void Click_Edit() {
            SceneLoader.Instance.LoadScene(_editScene);
        }

        public void Click_Controller() {
            EventSystem.current.SetSelectedGameObject(null);
            _mainContainer.gameObject.SetActive(false);
            _controllerContainer.gameObject.SetActive(true);
            var backButton = _controllerContainer.GetComponentInChildren<Button>();
            if (backButton != null) {
                EventSystem.current.SetSelectedGameObject(backButton.gameObject);
            }
        }

        public void Click_Options() {
            EventSystem.current.SetSelectedGameObject(null);
            _mainContainer.gameObject.SetActive(false);
            _settingsContainer.gameObject.SetActive(true);

            _settingsContainer.ActivateFirstTime();
        }

        public void Click_Wishlist() {
            var wishlistUrl = "https://store.steampowered.com/app/3089100/Aurora_Genesis/";
            Application.OpenURL(wishlistUrl);
        }

        public void Click_Back() {
            EventSystem.current.SetSelectedGameObject(null);
            _mainContainer.gameObject.SetActive(true);
            _settingsContainer.gameObject.SetActive(false);
            _controllerContainer.gameObject.SetActive(false);
            EventSystem.current.SetSelectedGameObject(_firstButton);
        }

        public void Click_QuitGame() {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}