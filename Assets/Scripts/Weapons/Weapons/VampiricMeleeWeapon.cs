using Base;
using BerserkPixel.Health;
using UnityEngine;

namespace Weapons {
    /// <summary>
    /// A melee weapon that heals the user for a percentage of the damage dealt.
    /// It hooks into the global damage events to detect when it has successfully landed a hit.
    /// </summary>
    [CreateAssetMenu(fileName = "VampiricMeleeWeapon", menuName = "Aurora/Weapons/Vampiric Melee Weapon")]
    public class VampiricMeleeWeapon : MeleeWeapon {
        [Tooltip("Percentage of damage dealt that is converted to health (0.1 = 10%).")]
        [SerializeField] private float _vampirismRatio = 0.15f;

        /// <summary>
        /// Subscribes to the global damage event when the weapon is active.
        /// </summary>
        private void OnEnable() {
            CharacterHealth.OnAnyDamagePerformed += HandleAnyDamagePerformed;
        }

        /// <summary>
        /// Unsubscribes to prevent memory leaks or calling logic on destroyed objects.
        /// </summary>
        private void OnDisable() {
            CharacterHealth.OnAnyDamagePerformed -= HandleAnyDamagePerformed;
        }

        private void OnDestroy() {
            CharacterHealth.OnAnyDamagePerformed -= HandleAnyDamagePerformed;
        }

        /// <summary>
        /// Global callback that checks if THIS weapon was the cause of damage.
        /// If so, it calculates the lifesteal and heals the attacker.
        /// </summary>
        /// <param name="data">The hit data containing weapon and attacker references.</param>
        private void HandleAnyDamagePerformed(HitData data) {
            // Check if THIS specific weapon instance caused the damage
            if (data.weapon == this && data.attacker != null) {
                int healAmount = Mathf.CeilToInt(data.damage * _vampirismRatio);
                
                if (healAmount > 0) {
                    ApplyHeal(data.attacker, healAmount);
                }
            }
        }

        /// <summary>
        /// Heals the attacking character's health component.
        /// </summary>
        /// <param name="attacker">The transform of the character who dealt damage.</param>
        /// <param name="amount">The amount of health to restore.</param>
        private void ApplyHeal(Transform attacker, int amount) {
            if (attacker.TryGetComponent<CharacterHealth>(out var health)) {
                health.GiveHealth(amount);
                
                // Optional: Play a small heal effect or sound here if desired.
                DebugTools.DebugLog.Log($"Vampiric Weapon healed {attacker.name} for {amount} HP");
            }
        }

        /// <summary>
        /// Standard attack implementation from MeleeWeapon.
        /// </summary>
        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            animations?.Play(AttackAnimation);
            StartCooldown();
        }
    }
}
