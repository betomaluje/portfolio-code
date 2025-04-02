using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class ButtonProgress : MonoBehaviour {
        [SerializeField]
        private RectTransform _buttonTransform;

        [SerializeField]
        private Vector2 _maxShakeAmount = new(10f, 2f);

        [SerializeField]
        private float _speed = 50f;

        [SerializeField]
        private Image _progressImage;

        [SerializeField]
        private float _resetAnimDuration = 0.5f;

        private Vector3 originalPosition;

        private void Start() {
            originalPosition = _buttonTransform.localPosition;
        }

        public void OnProgress(float percentage) {
            if (percentage > 0) {
                _progressImage.fillAmount = percentage;

                float shakeX = Mathf.Sin(Time.unscaledTime * _speed) * _maxShakeAmount.x * percentage;
                float shakeY = Mathf.Cos(Time.unscaledTime * _speed) * _maxShakeAmount.y * percentage;
                _buttonTransform.localPosition = originalPosition + new Vector3(shakeX, shakeY);
            }
            else {
                _progressImage.DOFillAmount(0f, _resetAnimDuration).SetUpdate(true);
                _buttonTransform.DOLocalMove(originalPosition, _resetAnimDuration).SetUpdate(true);
            }
        }
    }
}