using BerserkTools.Health.UI;
using TMPro;
using UnityEngine;

namespace UI {
    public class TextHealthBar : HealthBar {
        [SerializeField]
        private TextMeshProUGUI _text;

        protected override void Start() {
            base.Start();
            if (healthScript == null) {
                _disablePanelComponent.Hide();
                return;
            }
            healthScript.OnHealthChanged += HandleHealth;
            HandleHealth(healthScript.CurrentHealth, healthScript.MaxHealth);
            ChangePercentage(healthScript.CurrentHealth / (float)healthScript.MaxHealth);
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            if (healthScript == null) {
                return;
            }
            healthScript.OnHealthChanged -= HandleHealth;
        }

        public void HandleHealth(int current, int total) {
            _text.text = $"{current}/{total}";
        }
    }
}