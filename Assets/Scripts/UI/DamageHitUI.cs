using BerserkPixel.Health;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI {
    [RequireComponent(typeof(CanvasGroup))]
    public class DamageHitUI : MonoBehaviour {
        [SerializeField]
        private TextMeshProUGUI _damageText;

        [SerializeField]
        private float _timeToHide = 1f;

        private CanvasGroup _canvasGroup;
        
        private void Awake() {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void SetDamageText(HitData hitData) {
            _damageText.text = hitData.isCritical ? $"{hitData.damage}!!" : $"{hitData.damage}";

            _canvasGroup.alpha = 1f;
            _canvasGroup.DOFade(0f, _timeToHide).OnComplete(() => {
                Destroy(gameObject);
            });
        }
    }
}