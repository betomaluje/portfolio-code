using BerserkPixel.Health;
using Player;
using Player.Components;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modifiers.Conditions {
    [CreateAssetMenu(fileName = "Hit Condition", menuName = "Aurora/Modifiers/Conditions/Min Hit Combo")]
    [TypeInfoBox("Checks if the player has successfully hit an enemy a certain number of times in a row")]
    public class HitCounterCondition : BaseCondition {
        [SerializeField]
        [Min(0)]
        private int _minHits = 3;

        [BoxGroup("Take Damage")]
        [Tooltip("If true, damage is ignored when checking the hit counter.")]
        [SerializeField]
        private bool _canReceiveDamage = true;

        [BoxGroup("Take Damage")]
        [Tooltip("Time gap (in seconds) to check if the player receives a hit within this time frame to reset the combo.")]
        [SerializeField]
        [HideIf("_canReceiveDamage")]
        [Min(0f)]
        private float _damageResetTimeGap = 1f;

        private HitComboCounter _comboCounter;

        private CharacterHealth _health;
        private int _currentHealth;
        private int _currentHits;
        private float _lastDamageTime = -Mathf.Infinity;  // Tracks when the player last received damage

        public override void Setup(Transform owner) {
            if (owner.TryGetComponent<PlayerStateMachine>(out var stateMachine)) {
                _comboCounter = stateMachine.HitComboCounter;
                _comboCounter.OnHitAdded -= RegisterHit;
                _comboCounter.OnHitAdded += RegisterHit;
            }

            if (owner.TryGetComponent(out _health)) {
                _currentHealth = _health.CurrentHealth;
                _health.OnHealthChanged -= OnHealthChanged;
                _health.OnHealthChanged += OnHealthChanged;
            }

            _currentHits = 0;
        }

        public override void ResetCondition() {
            base.ResetCondition();
            _currentHits = 0;
        }

        private void OnHealthChanged(int newCurrentHealth, int maxHealth) {
            // we need to track a hit combo and ensure that it resets if the player takes damage.
            var tookDamage = _currentHealth > newCurrentHealth;

            if (tookDamage) {
                TryResetComboOnDamage();
            }

            _currentHealth = newCurrentHealth;
        }

        private void TryResetComboOnDamage() {
            // If damage should be ignored, do not reset the combo
            if (_canReceiveDamage) {
                return;
            }

            float currentTime = Time.time;

            // Reset the combo if damage occurred within the reset time gap
            if (currentTime - _lastDamageTime <= _damageResetTimeGap) {
                // DebugTools.DebugLog.Log("Combo reset due to taking damage within the time gap.");
                _currentHits = 0;
            }

            // Update last damage time
            _lastDamageTime = currentTime;
        }

        private void RegisterHit(int hitCount) {
            _currentHits = hitCount;
        }

        public override bool Check(float deltaTime) => _currentHits >= _minHits;

        public override void Cleanup() {
            if (_health != null) {
                _health.OnHealthChanged -= OnHealthChanged;
            }
            if (_comboCounter != null) {
                _comboCounter.OnHitAdded -= RegisterHit;
            }
        }
    }
}