using BerserkPixel.Health;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modifiers.Conditions {
    [CreateAssetMenu(menuName = "Aurora/Modifiers/Conditions/On Receive Damage")]
    [TypeInfoBox("Condition that triggers when the player has been damaged at least X amount of times in Y seconds")]
    public class ReceiveDamageCondition : BaseCondition {
        [SerializeField]
        private int _minAmountOfHits = 1;

        [SerializeField]
        private float _damageTimerThreshold = 2f;

        private CharacterHealth _health;

        private float _damageTimerAccumulated;
        private int _damageCount;

        public override void Setup(Transform owner) {
            if (owner.TryGetComponent(out _health)) {
                _health.OnDamagePerformed -= HandleHurt;
                _health.OnDamagePerformed += HandleHurt;
            }
            _damageTimerAccumulated = 0f;
            _damageCount = 0;
        }

        private void HandleHurt(HitData hitData) {
            _damageTimerAccumulated = 0f; // Reset the accumulator on every hurt event            
            _damageCount++;
        }

        public override void ResetCondition() {
            base.ResetCondition();
            _damageTimerAccumulated = 0f; // Reset the accumulator 
            _damageCount = 0;
        }

        public override bool Check(float deltaTime) {
            // Accumulate damage time over frames
            _damageTimerAccumulated += deltaTime;

            return _health != null && _damageTimerAccumulated <= _damageTimerThreshold && _damageCount >= _minAmountOfHits;
        }

        public override void Cleanup() {
            if (_health) {
                _health.OnDamagePerformed -= HandleHurt;
            }
        }
    }
}