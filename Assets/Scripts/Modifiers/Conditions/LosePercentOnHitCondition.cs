using BerserkPixel.Health;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modifiers.Conditions
{
    [CreateAssetMenu(menuName = "Aurora/Modifiers/Conditions/Lose Percent On Hit")]
    [TypeInfoBox("Condition that triggers after losing more than X% health in one hit")]
    public class LosePercentOnHitCondition : BaseCondition
    {
        [SerializeField]
        [Range(0f, 1f)]
        private float _lostPercentage = .15f;

        private CharacterHealth _health;
        private int _amountToLose;
        private int _damageTaken;

        public override void Setup(Transform owner)
        {
            if (owner.TryGetComponent(out _health))
            {
                _amountToLose = (int)(_lostPercentage * _health.MaxHealth);

                _health.OnDamagePerformed -= HandleHurt;
                _health.OnDamagePerformed += HandleHurt;
            }
        }

        private void HandleHurt(HitData hitData)
        {
            _damageTaken = hitData.damage;
        }

        public override bool Check(float deltaTime) => _health != null && _damageTaken > 0 && _damageTaken >= _amountToLose;

        public override void Cleanup()
        {
            if (_health)
            {
                _health.OnDamagePerformed -= HandleHurt;
            }
        }
    }
}