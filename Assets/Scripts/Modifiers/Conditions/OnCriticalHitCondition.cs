using BerserkPixel.Health;
using Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modifiers.Conditions {
    [CreateAssetMenu(menuName = "Aurora/Modifiers/Conditions/On Critical Hit")]
    [TypeInfoBox("Condition that triggers when the player's attack lands a critical hit")]
    public class OnCriticalHitCondition : BaseCondition {
        private PlayerStateMachine _machine;
        private bool _landedCriticalHit;

        public override void Setup(Transform owner) {
            if (owner.TryGetComponent(out _machine)) {
                _machine.OnHit += HandleCriticalHit;
            }
            _landedCriticalHit = false;
        }

        public override void ResetCondition() {
            base.ResetCondition();
            _landedCriticalHit = false;
        }

        private void HandleCriticalHit(HitData hitData) {
            _landedCriticalHit = hitData.isCritical;
        }

        public override bool Check(float deltaTime) => _machine != null && _landedCriticalHit;

        public override void Cleanup() {
            if (_machine != null) {
                _machine.OnHit -= HandleCriticalHit;
            }
            _landedCriticalHit = false;
        }
    }
}