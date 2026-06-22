using Base;
using UnityEngine;
using BerserkPixel.Health;

namespace Weapons.DesignEx {
    /// <summary>
    /// A spinning flame dance technique inspired by Tanjiro's Hinokami Kagura (Demon Slayer).
    /// Performs a radial attack that first magnetizes all nearby enemies inward before striking,
    /// ensuring the full ring of damage always lands. ICharge scales the pull radius and damage.
    /// </summary>
    [CreateAssetMenu(fileName = "HinokamiWhistlingFlame", menuName = "Aurora/Weapons/Expanded/Hinokami Whistling Flame")]
    public class HinokamiWhistlingFlameWeapon : MeleeWeapon, ICharge {

        [Header("Flame Dance Properties")]
        [Tooltip("Base radius of the spinning attack.")]
        [SerializeField] private float _baseRadius = 2.5f;

        [Tooltip("At full charge, the radius scales by this multiplier.")]
        [SerializeField] private float _maxRadiusMultiplier = 2.2f;

        [Tooltip("Force applied to pull enemies inward before the hit.")]
        [SerializeField] private float _pullForce = 18f;

        [Tooltip("Layer mask for enemies to pull and damage.")]
        [SerializeField] private LayerMask _targetMask;

        [Tooltip("Time to reach maximum charge.")]
        [SerializeField] private float _chargeTime = 1.5f;

        [Tooltip("Damage multiplier at full charge.")]
        [SerializeField] private float _maxDamageMult = 2.5f;

        private float _chargeValue;

        public float Charge {
            set => _chargeValue = Mathf.Clamp01(value);
        }

        public float ChargeTime => _chargeTime;

        // The attack collider is centered on the player — no directional offset needed.
        public override bool ShouldMoveAttackCollider() => false;
        public override Vector2 AttackSize => Vector2.one * (CurrentRadius * 2f);
        public override Vector2 AttackOffset => Vector2.zero;

        private float CurrentRadius => Mathf.Lerp(_baseRadius, _baseRadius * _maxRadiusMultiplier, _chargeValue);

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            // 1. Pull all enemies inward — magnetic effect before the spin lands
            PullEnemiesInward(position);

            // 2. Trigger the spin animation (actual damage via AttackCollider + AnimationTrigger)
            animations?.Play(AttackAnimation);

            _chargeValue = 0f;
            StartCooldown();
        }

        private void PullEnemiesInward(Vector3 center) {
            Collider2D[] nearby = Physics2D.OverlapCircleAll(center, CurrentRadius, _targetMask);
            foreach (var col in nearby) {
                if (col.TryGetComponent<Rigidbody2D>(out var rb)) {
                    Vector2 toward = ((Vector2)center - (Vector2)col.transform.position).normalized;
                    rb.AddForce(toward * _pullForce, ForceMode2D.Impulse);
                }
            }
        }

        public override int GetDamage() {
            return Mathf.CeilToInt(base.GetDamage() * Mathf.Lerp(1f, _maxDamageMult, _chargeValue));
        }
    }
}
