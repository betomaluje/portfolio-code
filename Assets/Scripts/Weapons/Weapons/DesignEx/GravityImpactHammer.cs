using Base;
using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// A heavy melee weapon with massive knockback.
    /// Mastery: Smashing enemies into walls triggers "Collision Damage" which far exceeds base damage.
    /// </summary>
    [CreateAssetMenu(fileName = "GravityImpactHammer", menuName = "Aurora/Weapons/Expanded/Gravity Impact Hammer")]
    public class GravityImpactHammer : MeleeWeapon {
        
        [Header("Impact Mechanics")]
        [Tooltip("The base force multiplier applied to the target's position.")]
        [SerializeField] private float _impactForce = 50f;

        [Tooltip("Extra damage dealt when an enemy hits a wall due to this weapon's knockback.")]
        [SerializeField] private int _wallSlamDamage = 35;

        // Note: The actual "Wall Slam" logic usually requires the enemy to have a component 
        // that detects wall collisions during a "Knockback" state. 
        // For this implementation, we apply the initial heavy knockback.
        
        /// <summary>
        /// Executes the heavy swing.
        /// </summary>
        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            animations?.Play(AttackAnimation);
            
            // The actual damage and knockback are handled by the weapon's collider triggers,
            // which use GetKnockback() and GetDamage() from the Weapon class.
            
            StartCooldown();
        }

        /// <summary>
        /// Overrides the base knockback calculation to provide the "Gravity" feel.
        /// </summary>
        public override float GetKnockback() {
            return _impactForce;
        }

        /// <summary>
        /// Property for external systems (like an Impact Detector on the enemy) 
        /// to query how much damage a wall slam should do.
        /// </summary>
        public int WallSlamDamage => _wallSlamDamage;

        protected override void OnValidate() {
            base.OnValidate();
            if (string.IsNullOrEmpty(AttackAnimation)) {
                AttackAnimation = "Attack";
            }
        }
    }
}
