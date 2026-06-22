using UnityEngine;
using BerserkPixel.Health;
using BerserkPixel.Utils;

namespace Weapons.DesignEx {
    /// <summary>
    /// A dart that deals 0 direct damage but embeds itself onto enemies yielding an explosion later.
    /// </summary>
    public class MarkDartBullet : BaseBullet {
        [Tooltip("The radius of the explosion upon detonation.")]
        [SerializeField] private float _explosionRadius = 2.5f;

        [Tooltip("The damage of the explosion when triggered.")]
        [SerializeField] private int _explosionDamage = 20;

        private bool _isEmbedded = false;

        private void OnTriggerEnter2D(Collider2D other) {
            if (_isEmbedded) return; // Already stuck

            // Note: Not executing the base CheckCollision so we don't apply flat damage on hit
            // In a complete implementation we might want to check the mask manually.

            if (_targetMask.LayerMatchesObject(other)) {
                _isEmbedded = true;
                _rb.linearVelocity = Vector2.zero; // Stop dart physics
                transform.SetParent(other.transform); // Attach
                SpawnCollisionParticles(transform.position); // Give setup feedback
            }
        }

        /// <summary>
        /// Called from MarkDetonatePistolWeapon or an external tracker to pop the damage.
        /// It causes an overlap circle area effect.
        /// </summary>
        public void ExecuteDetonation() {
            if (!_isEmbedded) return; // Only explode stuck darts

            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _explosionRadius, _targetMask);
            foreach (var hit in hitColliders) {
                if (hit.TryGetComponent<CharacterHealth>(out var health) && !health.IsDead) {
                    var detonateHit = new HitDataBuilder()
                        .WithWeapon(_weapon)
                        .WithDamage(_explosionDamage)
                        .WithDirection((hit.transform.position - transform.position).normalized)
                        .Build(transform, hit.transform);

                    health.PerformDamage(detonateHit);
                    Debug.Log($"Mark detonated for {_explosionDamage} on {hit.name}");
                }
            }
            
            // Effect
            PlayImpactSound(transform.position, "projectile_impact");
            SpawnCollisionParticles(transform.position);

            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _explosionRadius);
        }
    }
}
