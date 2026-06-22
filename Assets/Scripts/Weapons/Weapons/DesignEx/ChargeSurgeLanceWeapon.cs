using Base;
using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// A melee weapon utilizing ICharge. Unlike standard ChargedWeapon (which shoots big bullets),
    /// this weapon scales a physical lunge and melee cleave based on charge time.
    /// </summary>
    [CreateAssetMenu(fileName = "ChargeSurgeLance", menuName = "Aurora/Weapons/Expanded/Charge Surge-Lance")]
    public class ChargeSurgeLanceWeapon : MeleeWeapon, ICharge {
        
        [Header("Surge Properties")]
        [Tooltip("Max force applied to launch the player forward on release.")]
        [SerializeField] private float _maxLungeForce = 40f;
        
        [Tooltip("Damage multiplier attained when fully charged.")]
        [SerializeField] private float _maxDamageMultiplier = 3.0f;

        [Tooltip("Increases knockback based on the charge ratio.")]
        [SerializeField] private float _maxKnockbackMultiplier = 2.0f;

        [Tooltip("Time in seconds to reach maximum charge output.")]
        [SerializeField] private float _chargeTime = 2.0f;

        private float _chargeValue;

        /// <summary>
        /// Injected by the WeaponManager while player holds the attack button.
        /// </summary>
        public float Charge {
            set => _chargeValue = Mathf.Clamp01(value);
        }

        public float ChargeTime => _chargeTime;

        /// <summary>
        /// Triggers right as the player releases the charge. 
        /// In this weapon's design, we dynamically lunge based on the built-up ratio.
        /// </summary>
        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            animations?.Play(AttackAnimation);
            
            // Execute the lunge 
            Transform owner = animations?.Transform.root;
            if (owner != null && owner.TryGetComponent<Rigidbody2D>(out var rb)) {
                // Minimum lunge is 10% of max, scales linearly with charge
                float currentLunge = Mathf.Lerp(_maxLungeForce * 0.1f, _maxLungeForce, _chargeValue);
                rb.AddForce(direction.normalized * currentLunge, ForceMode2D.Impulse);
            }

            StartCooldown();
        }

        public override float GetKnockback() {
            // Scales knockback linearly with the chargeBuilt ratio
            float knockMult = Mathf.Lerp(1.0f, _maxKnockbackMultiplier, _chargeValue);
            return base.GetKnockback() * knockMult;
        }

        /// <summary>
        /// Exposes the scaled damage based on our charge ratio (called from your animation hit collider).
        /// </summary>
        public override void SetDamageInfluence(float strength) {
            // Overrides basic strength to also include our charge multiplier
            float chargeMult = Mathf.Lerp(1.0f, _maxDamageMultiplier, _chargeValue);
            base.SetDamageInfluence(strength * chargeMult);
        }

        // Note: Knockback would typically be applied via your HitDataBuilder, 
        // which you can scale using a similar property override.
        // Once the attack resolves, ensure WeaponManager or an Event resets _chargeValue = 0f.
    }
}
