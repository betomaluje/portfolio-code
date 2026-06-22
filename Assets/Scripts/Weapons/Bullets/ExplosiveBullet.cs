using System.Collections.Generic;
using BerserkPixel.Health;
using BerserkPixel.Utils;
using Extensions;
using UnityEngine;

namespace Weapons {
    /// <summary>
    /// A projectile that triggers an area-of-effect (AOE) explosion upon hitting a target or obstacle.
    /// Deals damage to all enemies within the blast radius.
    /// </summary>
    public class ExplosiveBullet : BaseBullet {
        [Tooltip("The radius of the explosion.")]
        [SerializeField] private float _explosionRadius = 2.5f;

        [Tooltip("Damage multiplier for enemies in the explosion radius (relative to the base bullet damage).")]
        [SerializeField] private float _explosionDamageMultiplier = 1.0f;

        [Tooltip("Optional particles for the explosion itself.")]
        [SerializeField] private ParticleSystem _explosionParticlesPrefab;

        [Tooltip("The sound effect played when the bullet explodes.")]
        [SerializeField] private string _explosionSound = "projectile_impact";

        [Tooltip("Layer mask for environment/obstacles that trigger the explosion.")]
        [SerializeField] private LayerMask _worldMask;

        /// <summary>
        /// Handles collision and triggers explosion.
        /// We verify if the hit is a valid target or a wall before detonating.
        /// </summary>
        /// <param name="other">The collider hit.</param>
        private void OnTriggerEnter2D(Collider2D other) {
            // We use the TargetMask to see if it's an enemy, or WorldMask for obstacles
            if (_targetMask.LayerMatchesObject(other) || _worldMask.LayerMatchesObject(other)) {
                Explode();
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Finds all targets in radius and applies damage. Also spawns visual and sound effects.
        /// </summary>
        private void Explode() {
            // Find all valid targets
            // We use OverlapCircleAll because it might hit multiple characters
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _explosionRadius, _targetMask);
            
            int explosionDamage = Mathf.CeilToInt(GetDamage() * _explosionDamageMultiplier);
            
            foreach (var col in hitColliders) {
                if (col.TryGetComponent<CharacterHealth>(out var health) && !health.IsDead) {
                    ApplyExplosionDamage(col, explosionDamage);
                }
            }

            // Effects
            SpawnExplosionEffects();
        }

        /// <summary>
        /// Applies damage to a specific enemy found in the blast.
        /// </summary>
        /// <param name="col">Target collider.</param>
        /// <param name="damage">Calculated damage.</param>
        private void ApplyExplosionDamage(Collider2D col, int damage) {
            Vector2 direction = (col.transform.position - transform.position).normalized;
            var hitData = new HitDataBuilder()
                .WithDamage(damage)
                .WithDirection(direction)
                .Build(transform, col.transform);
            
            hitData.PerformDamage(col);
        }

        /// <summary>
        /// Spawns the visual particles and plays the explosion sound at the impact spot.
        /// </summary>
        private void SpawnExplosionEffects() {
            if (_explosionParticlesPrefab != null) {
                Instantiate(_explosionParticlesPrefab, transform.position, Quaternion.identity);
            }
            PlayImpactSound(transform.position, _explosionSound); // Reuses projectile impact or a dedicated sound
        }

        /// <summary>
        /// Draws the blast radius in the editor for easy configuration.
        /// </summary>
        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _explosionRadius);
        }
    }
}
