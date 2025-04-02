using DG.Tweening;
using BerserkTools.Health.UI;
using Player.Input;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public abstract class ModifierUI<T> : MonoBehaviour {
        [SerializeField]
        private Image _containerImage;

        [SerializeField]
        protected Image _modifierImage;

        [SerializeField]
        protected ProgressbarBehaviour _progressBar;

        [Header("Animations")]
        [SerializeField]
        private float _animationDuration = .5f;

        [SerializeField]
        private Ease _animationEase = Ease.InOutCirc;

        [FoldoutGroup("Shake Settings")]
        [SerializeField]
        private float _shakeStrength = 15f;

        [FoldoutGroup("Shake Settings")]
        [SerializeField]
        private int _shakeVibrato = 20;

        [FoldoutGroup("Shake Settings")]
        [SerializeField]
        private float _shakeRandomness = 10f;

        [FoldoutGroup("Shake Settings")]
        [SerializeField]
        private ShakeRandomnessMode _shakeFadeOut = ShakeRandomnessMode.Harmonic;

        [FoldoutGroup("Scale Settings")]
        [SerializeField]
        private Vector2 _modifierScale = new(.5f, .5f);

        [FoldoutGroup("Scale Settings")]
        [SerializeField]
        private int _scaleVibrato = 10;

        private readonly int MAT_GREYSCALE = Shader.PropertyToID("_GreyscaleBlend");
        private Vector2 _modifierShakeStrength;

        private Material _modifierMaterial;
        protected T _playerModifierController;
        private RectTransform _rectTransform;

        private void Awake() {
            var player = FindFirstObjectByType<PlayerBattleInput>();
            if (player != null) {
                _playerModifierController = player.GetComponent<T>();
            }

            if (_containerImage != null) {
                _modifierMaterial = new Material(_containerImage.material);
                _rectTransform = _containerImage.rectTransform;
            }

            _modifierShakeStrength = new Vector2(_shakeStrength, 0);
        }

        private void OnValidate() {
            if (_progressBar == null) {
                _progressBar = GetComponentInChildren<ProgressbarBehaviour>();
            }
        }

        private void Start() {
            ToggleModifierColors(false);

        }

        protected void ToggleModifierColors(bool enable) {
            _modifierMaterial.SetFloat(MAT_GREYSCALE, enable ? 0f : 1f);

            Color color = _modifierImage.color;
            color.a = enable ? 1f : 0f;
            _modifierImage.color = color;
        }

        protected void DoDurationProgress(float duration) {
            Shake();

            Fill(duration);
        }

        [Button("Fill")]
        private void Fill(float duration) {
            _progressBar.ResetBar(true);
            _progressBar.ChangePercentage(1, duration, _animationEase);
        }

        [Button("Shake")]
        private void Shake() {
            _rectTransform.DOShakeAnchorPos(
               _animationDuration,
               _modifierShakeStrength,
               _shakeVibrato,
               _shakeRandomness,
               randomnessMode: _shakeFadeOut
           );

            _rectTransform.DOPunchScale(_modifierScale, _animationDuration, _scaleVibrato);
        }
    }

}