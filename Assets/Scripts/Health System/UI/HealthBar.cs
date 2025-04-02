using BerserkPixel.Health;
using DG.Tweening;
using UI;
using UnityEngine;

namespace BerserkTools.Health.UI {
    public class HealthBar : ProgressbarBehaviour {
        [SerializeField]
        protected CharacterHealth healthScript;

        protected DisablePanelComponent _disablePanelComponent;

        private void Awake() {
            _disablePanelComponent = new DisablePanelComponent(GetComponent<RectTransform>());
        }

        public void SetCharacterHealth(CharacterHealth characterHealth) {
            healthScript = characterHealth;
            float percentage = healthScript.CurrentHealth / (float)healthScript.MaxHealth;
            ChangePercentage(percentage);
        }

        protected virtual void Start() {
            if (healthScript == null) {
                _disablePanelComponent.Hide();
                return;
            }

            healthScript.OnPercentageChanged += HandleHealth;
        }

        protected virtual void OnDestroy() {
            if (healthScript == null) {
                return;
            }

            healthScript.OnPercentageChanged -= HandleHealth;
        }

        private void HandleHealth(float healthPercentage) {
            if (healthPercentage <= 0) {
                Destroy(gameObject);
                return;
            }

            ChangePercentage(healthPercentage);
        }

        public override void ChangePercentage(float healthPercentage, float duration = -1, Ease easeType = Ease.Linear) {
            if (healthPercentage <= 0) {
                gameObject.SetActive(false);
            }

            base.ChangePercentage(healthPercentage);
        }
    }
}