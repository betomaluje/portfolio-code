using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sounds;
using TMPro;
using UnityEngine;

namespace Level {

    public class BossName : MonoBehaviour {
        [SerializeField]
        private TextMeshProUGUI _realmNameText;

        [SerializeField]
        private string _bossName;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private float _animDuration = .5f;

        [SerializeField]
        private float _timeToHide = 2f;

        private void OnEnable() {
            if (string.IsNullOrWhiteSpace(_bossName)) {
                _canvasGroup.alpha = 0;
                gameObject.SetActive(false);
                Destroy(this);
                return;
            }

            SoundManager.instance.Play("Boss Name");

            Sequence sequence = DOTween.Sequence();
            sequence.Append(_canvasGroup.DOFade(1f, _animDuration));
            sequence.Append(_realmNameText.DOText(_bossName, _animDuration));
            sequence.SetEase(Ease.InExpo);
            sequence.OnComplete(HideText);
            sequence.Play();
        }

        private async void HideText() {
            await UniTask.Delay((int)_timeToHide * 1000);

            _canvasGroup.DOFade(0f, _animDuration).OnComplete(() => gameObject.SetActive(false));
        }
    }
}