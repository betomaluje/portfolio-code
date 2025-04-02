using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace BerserkTools.Health.UI {
    public class ProgressbarBehaviour : MonoBehaviour {
        [SerializeField]
        protected Image[] foregroundImages;

        [SerializeField]
        [Min(0)]
        protected float updateSpeedSeconds = 0.2f;

        private void OnEnable() {
            ResetBar(true);
        }

        public virtual void ChangePercentage(float percentage, float duration = -1, Ease easeType = Ease.Linear) {
            if (duration == -1) {
                duration = updateSpeedSeconds;
            }

            foreach (Image image in foregroundImages) {
                image.DOFillAmount(percentage, duration).SetEase(easeType);
            }
        }

        public virtual void ResetBar(bool instant = false, float initialValue = 0) {
            if (instant) {
                foreach (Image image in foregroundImages) {
                    image.fillAmount = initialValue;
                }
            }
            else {
                foreach (Image image in foregroundImages) {
                    image.DOFillAmount(initialValue, updateSpeedSeconds);
                }
            }
        }
    }
}