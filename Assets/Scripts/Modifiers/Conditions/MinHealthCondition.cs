using BerserkPixel.Health;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modifiers.Conditions {
    [CreateAssetMenu(menuName = "Aurora/Modifiers/Conditions/Min Health")]
    [TypeInfoBox("Checks if the character's health percentage is at or below a certain threshold")]
    public class MinHealthCondition : BaseCondition {
        [SerializeField]
        [Range(0f, 1f)]
        private float _peakPercentage = .15f;

        private CharacterHealth _health;

        public override void Setup(Transform owner) {
            if (owner.TryGetComponent(out _health)) {

            }
        }

        public override bool Check(float deltaTime) => _health != null && _health.CurrentHealthPercentage <= _peakPercentage;

        public override void Cleanup() {
            _health = null;
        }
    }
}