using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Level {
    public class RealmName : MonoBehaviour {
        [SerializeField]
        private TextMeshProUGUI _realmNameText;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private float _animDuration = .5f;

        [SerializeField]
        private float _timeToHide = 2f;

        private void OnEnable() {
            var realmName = PlayerPrefs.GetString("RealmName", "");

            if (string.IsNullOrWhiteSpace(realmName)) {
                _canvasGroup.alpha = 0;
                gameObject.SetActive(false);
                return;
            }

            Sequence sequence = DOTween.Sequence();
            sequence.Append(_canvasGroup.DOFade(1f, _animDuration));
            sequence.Append(_realmNameText.DOText(realmName, _animDuration));
            sequence.SetEase(Ease.InExpo);
            sequence.OnComplete(HideText);
            sequence.Play();
        }

        private void OnDisable() {
            PlayerPrefs.SetString("RealmName", "");
        }

        private async void HideText() {
            await UniTask.Delay((int)_timeToHide * 1000);

            _canvasGroup.DOFade(0f, _animDuration).OnComplete(() => gameObject.SetActive(false));
        }
    }
}