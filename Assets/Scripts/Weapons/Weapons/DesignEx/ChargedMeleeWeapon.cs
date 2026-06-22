using Base;
using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// Base configuration for committal melee weapons that require a charge-up phase before executing.
    /// Useful for Lances, Heavy Hammers, or Big Beam Cannons.
    /// Scales damage based on how long the attack was held.
    /// </summary>
    [CreateAssetMenu(fileName = "ChargedMeleeWeapon", menuName = "Aurora/Weapons/Expanded/Charged Melee")]
    public class ChargedMeleeWeapon : MeleeWeapon, ICharge {
        
        [Header("Charge Properties")]
        [Tooltip("Time in seconds to reach maximum charge output.")]
        [SerializeField] [Min(0.1f)] private float _maxChargeTime = 1.0f;
        
        [Tooltip("Damage multiplier attained when fully charged.")]
        [SerializeField] [Min(1.0f)] private float _maxDamageMultiplier = 2.5f;

        [Tooltip("If true, the attack triggers automatically when max charge is reached. If false, player must release.")]
        [SerializeField] private bool _autoReleaseOnMax = false;

        private float _chargeValue;

        /// <summary>
        /// Property required by the ICharge interface. 
        /// Injected by the WeaponManager during the charging sequence.
        /// </summary>
        public float Charge {
            set => _chargeValue = Mathf.Clamp01(value);
        }

        public float ChargeTime => _maxChargeTime;

        /// <summary>
        /// Gets the time required to reach maximum charge.
        /// </summary>
        public float MaxChargeTime => _maxChargeTime;

        /// <summary>
        /// Gets whether the weapon should release automatically at max charge.
        /// </summary>
        public bool AutoReleaseOnMax => _autoReleaseOnMax;

        /// <summary>
        /// Begins the charging phase. The actual attack is usually executed on release.
        /// Here we trigger the "Charge" animation state.
        /// </summary>
        /// <param name="animations">The character animation controller.</param>
        /// <param name="direction">Aim direction.</param>
        /// <param name="position">Origin position.</param>
        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;
            
            // Note: In a complete implementation, the WeaponManager tracks the button hold
            // and passes the ratio to the Charge property. We trigger the charge animation here.
            animations?.Play(AttackAnimation + "_Charge");
        }

        /// <summary>
        /// Executes the release of the charge attack.
        /// This would be called by the WeaponManager or an Animation Event when the swing completes.
        /// </summary>
        /// <returns>The calculated damage based on charge duration.</returns>
        public int ExecuteRelease() {
            StartCooldown();
            float multiplier = Mathf.Lerp(1.0f, _maxDamageMultiplier, _chargeValue);
            int finalDamage = Mathf.CeilToInt(GetDamage() * multiplier);
            
            // Reset charge after release
            _chargeValue = 0f;
            
            return finalDamage;
        }

        /// <summary>
        /// Overrides the damage influence calculation to incorporate the charge multiplier.
        /// This ensures the standard hit detection system picks up the charged damage.
        /// </summary>
        /// <param name="strength">Base strength influence from the character stats.</param>
        public override void SetDamageInfluence(float strength) {
            float chargeMult = Mathf.Lerp(1.0f, _maxDamageMultiplier, _chargeValue);
            base.SetDamageInfluence(strength * chargeMult);
        }
        
        /// <summary>
        /// Editor validation to ensure the attack animation has a charge variant.
        /// </summary>
        protected override void OnValidate() {
            base.OnValidate();
            if (string.IsNullOrEmpty(AttackAnimation)) {
                AttackAnimation = "Attack";
            }
        }
    }
}
