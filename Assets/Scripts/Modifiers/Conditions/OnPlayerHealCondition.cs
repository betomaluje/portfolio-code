using BerserkPixel.Health;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modifiers.Conditions {
    [CreateAssetMenu(fileName = "Player Heal Condition", menuName = "Aurora/Modifiers/Conditions/On Heal")]
    [TypeInfoBox("Checks if this entity has healed by a certain amount")]
    public class OnPlayerHealCondition : BaseCondition {
        [SerializeField]
        [Min(0)]
        private int _minChange = 1;

        private CharacterHealth _health;
        private int _currentHealth;
        private bool _hasHealedAboveMin;

        public override void Setup(Transform owner) {
            if (owner.TryGetComponent(out _health)) {
                _currentHealth = _health.CurrentHealth;
                _health.OnHealthChanged -= OnHealthChanged;
                _health.OnHealthChanged += OnHealthChanged;
            }

            _hasHealedAboveMin = false;
        }

        public override void ResetCondition() {
            base.ResetCondition();
            _hasHealedAboveMin = false;
        }

        private void OnHealthChanged(int currentHealth, int maxHealth) {
            bool aboveMin = Mathf.Abs(_currentHealth - currentHealth) >= _minChange;
            bool hasIncreased = _currentHealth < currentHealth;
            if (aboveMin && hasIncreased) {
                _hasHealedAboveMin = true;
            }
            _currentHealth = currentHealth;
        }

        public override bool Check(float deltaTime) => _health != null && _hasHealedAboveMin;

        public override void Cleanup() {
            if (_health != null) {
                _health.OnHealthChanged -= OnHealthChanged;
            }
        }
    }
}