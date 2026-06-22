using Base;
using BerserkPixel.Health;
using Modifiers.Powerups;
using UnityEngine;

namespace Weapons {
    /// <summary>
    /// A localized weapon that doesn't just deal direct damage, 
    /// but also applies a specialized status effect (PowerupConfig) to the hit victim.
    /// This could be a "Freeze Gun", "Poison Blade", or "Debuff Curse".
    /// </summary>
    [CreateAssetMenu(fileName = "StatusWeapon", menuName = "Aurora/Weapons/Status Weapon")]
    public class StatusWeapon : ShootingWeapon {
        [Tooltip("The status effect (PowerupConfig) to apply to the victim.")]
        [SerializeField] private PowerupConfig _statusEffect;

        [Tooltip("The probability of applying the effect on each hit (0.0 to 1.0).")]
        [SerializeField, Range(0f, 1f)] private float _applyChance = 1.0f;

        /// <summary>
        /// Subscribes to global damage events when the weapon is active.
        /// </summary>
        private void OnEnable() {
            CharacterHealth.OnAnyDamagePerformed += HandleStatusApplication;
        }

        /// <summary>
        /// Unsubscribes when the weapon is swapped or destroyed.
        /// </summary>
        private void OnDisable() {
            CharacterHealth.OnAnyDamagePerformed -= HandleStatusApplication;
        }

        private void OnDestroy() {
            CharacterHealth.OnAnyDamagePerformed -= HandleStatusApplication;
        }

        /// <summary>
        /// Hook to check if THIS specific weapon caused damage and, if so, 
        /// apply the assigned status effect to the victim.
        /// </summary>
        /// <param name="data">Hit specifics.</param>
        private void HandleStatusApplication(HitData data) {
            // Validating weapon instance and application chance
            if (data.weapon == this && Random.value <= _applyChance) {
                ApplyEffect(data.victim);
            }
        }

        /// <summary>
        /// Attempts to find the CharacterPowerup component on the victim 
        /// and inject the configured status effect.
        /// </summary>
        /// <param name="victim">Target transform hit by the weapon.</param>
        private void ApplyEffect(Transform victim) {
            if (victim == null || _statusEffect == null) return;

            // CharacterPowerup acts as the receiver for all status effects and boosters
            if (victim.TryGetComponent<CharacterPowerup>(out var characterPowerup)) {
                // We clone the config if we want independent timers per enemy, 
                // but usually SOs are fine if DoPowerup handles instantiation internally.
                characterPowerup.DoPowerup(_statusEffect, victim);
                
                DebugTools.DebugLog.Log($"Applied effect {_statusEffect.Name} to {victim.name}");
            }
        }

        /// <summary>
        /// Standard attack logic (inherited from ShootingWeapon).
        /// </summary>
        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            if (HasAmmo()) {
                animations?.Play(AttackAnimation);
                
                // Fire the projectile (using current animations transform as owner)
                ShootBullet(position, direction, animations?.Transform);
                
                StartCooldown();
            } else {
                Reload(animations);
            }
        }
    }
}
