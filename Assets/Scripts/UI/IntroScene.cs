using System;
using DG.Tweening;
using Scene_Management;
using Sounds;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

namespace UI {
    public class IntroScene : MonoBehaviour {
        [SerializeField]
        private Step[] _steps;

        [SerializeField]
        private float _animationYPos = 90;

        [SerializeField]
        private SceneField _villageScene;

        [Header("UI References")]
        [SerializeField]
        private TextMeshProUGUI _dialogText;

        [SerializeField]
        private Image _backgroundImage;

        [SerializeField]
        private RectTransform _pressAnyKeyTransform;

        private int _totalSteps;
        private int _currentStep;
        private Action _onKeyPressed;
        private IDisposable _anyKeyDisposable;

        private void Awake() {
            _totalSteps = _steps.Length;
            _currentStep = 0;

            _onKeyPressed = HandleClickEvent;
        }

        private void Start() {
            Step step = _steps[_currentStep];

            ChangeStep(_currentStep);
            _pressAnyKeyTransform.DOAnchorPosY(_animationYPos, 1f).SetId("any_key").SetDelay(step.Speed / 2f).SetLoops(-1, LoopType.Yoyo);
        }

        private void OnDestroy() {
            _anyKeyDisposable?.Dispose();
            DOTween.Kill("any_key");
            DOTween.KillAll();
        }

        private void HandleClickEvent() {
            _currentStep++;
            SoundManager.instance.Play("button_click");
            if (_currentStep < _totalSteps) {
                ChangeStep(_currentStep);
            }
            else {
                OnDialogueFinish();
            }
        }

        private void ChangeStep(int index) {
            if (index < 0 || index >= _steps.Length) {
                return;
            }
            _anyKeyDisposable?.Dispose();

            _anyKeyDisposable = InputSystem.onAnyButtonPress.CallOnce(ctrl => { _onKeyPressed?.Invoke(); });

            Step step = _steps[index];

            _dialogText.text = "";
            _dialogText.DOText(step.Text, step.Speed);

            if (step.Background != null) {
                _backgroundImage.sprite = step.Background;
                _backgroundImage.DOFade(0f, 0f);
                _backgroundImage.DOFade(1f, .5f);
            }
        }

        private void OnDialogueFinish() {
            SceneLoader.Instance.LoadScene(_villageScene);
        }
    }

    [Serializable]
    public struct Step {
        [TextArea(1, 3)]
        public string Text;
        public float Speed;
        public Sprite Background;
    }
}