using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EditorTool {
    public class ToolUI : MonoBehaviour {
        [SerializeField]
        private Image _image;

        [SerializeField]
        private RectTransform _container;

        [SerializeField]
        private TextMeshProUGUI _amountText;

        [Header("FX")]
        [SerializeField]
        private float _animDuration = .5f;

        [SerializeField]
        private float _scaleFactor = 1.2f;

        [Header("Shake FX")]
        [SerializeField]
        private Vector3 _shakeStrength;

        [SerializeField]
        private int _shakeVibrato = 1;

        [HideInInspector]
        public string ID;
        private bool _hasBeenDisabled;

        public void SetupTool(Tool tool) {
            _image.sprite = tool.Icon();
            _image.preserveAspect = true;
            _image.transform.localScale = Vector3.one * tool.UIScale;

            ID = tool.ID;
            UpdateTool(tool);
            SetSelected(false);
        }

        public void UpdateTool(Tool tool) {
            _amountText.text = tool.CurrentAmount.ToString();

            if (tool.CurrentAmount <= 0) {
                _image.DOFade(0.5f, _animDuration);
                _container.DOScale(Vector2.one, _animDuration);
                _hasBeenDisabled = true;
            }
            else if (_hasBeenDisabled && tool.CurrentAmount > 0) {
                _hasBeenDisabled = false;
                _image.DOFade(1f, _animDuration);
                _container.DOScale(Vector2.one * _scaleFactor, _animDuration);
            }
        }

        public void Shake() {
            _container.DOShakeScale(_animDuration, _shakeStrength, _shakeVibrato);
        }

        public void SetSelected(bool selected) {
            if (!_container)
                return;

            var factor = selected ? _scaleFactor : 1f;
            _container.DOScale(Vector2.one * factor, _animDuration);
        }
    }
}