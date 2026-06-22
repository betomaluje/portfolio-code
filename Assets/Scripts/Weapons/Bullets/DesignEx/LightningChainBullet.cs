using UnityEngine;
using BerserkPixel.Health;
using BerserkPixel.Utils;

namespace Weapons.DesignEx {
    /// <summary>
    /// A short-range lightning arc that spawns on the first electrocuted target and
    /// chains to up to N adjacent enemies within a radius.
    /// Inspired by Kakashi's Chidori / Sasuke's Raikiri (Naruto).
    /// </summary>
    public class LightningChainBullet : BaseBullet {
        [Tooltip("Maximum number of additional targets to chain to after the first.")]
        [SerializeField] private int _maxChainJumps = 3;

        [Tooltip("Radius to search for the next chain target.")]
        [SerializeField] private float _chainRadius = 3f;

        [Tooltip("Damage falloff per chain jump (0.75 = 25% less per jump).")]
        [SerializeField] private float _chainDamageFalloff = 0.75f;

        [Tooltip("Target layer for chain searching.")]
        [SerializeField] private LayerMask _chainTargetMask;

        private int _chainsLeft;

        private void OnEnable() {
            _chainsLeft = _maxChainJumps;
        }

        public override void Fire(Vector2 direction) {
            // The Raikiri chain bullet is spawned at the initial hit target.
            // It doesn't travel — it triggers the chain cascade immediately.
            if (_rb != null) {
                _rb.linearVelocity = Vector2.zero;
                _rb.bodyType = RigidbodyType2D.Static;
            }

            ChainFrom(transform.position, GetDamage(), null);

            // Self-destruct after chain is done
            Destroy(gameObject);
        }

        /// <summary>
        /// Recursively chains lightning to nearby targets.
        /// </summary>
        private void ChainFrom(Vector2 origin, int damage, Transform previousTarget) {
            if (_chainsLeft <= 0) return;

            Collider2D[] nearby = Physics2D.OverlapCircleAll(origin, _chainRadius, _chainTargetMask);

            foreach (var col in nearby) {
                // Skip the target we just jumped from
                if (previousTarget != null && col.transform == previousTarget) continue;

                if (col.TryGetComponent<CharacterHealth>(out var health) && !health.IsDead) {
                    var dir = (col.transform.position - (Vector3)origin).normalized;
                    var hitData = new HitDataBuilder()
                        .WithWeapon(_weapon)
                        .WithDamage(damage)
                        .WithDirection(dir)
                        .Build(transform, col.transform);

                    health.PerformDamage(hitData);
                    SpawnCollisionParticles(col.transform.position);

                    _chainsLeft--;

                    // Chain recursively from the new target
                    ChainFrom(col.transform.position, Mathf.CeilToInt(damage * _chainDamageFalloff), col.transform);

                    // Only chain to one target per jump for the Raikiri feel
                    break;
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            Gizmos.color = new Color(0.4f, 0.8f, 1f, 0.35f);
            Gizmos.DrawWireSphere(transform.position, _chainRadius);
        }
#endif
    }
}
