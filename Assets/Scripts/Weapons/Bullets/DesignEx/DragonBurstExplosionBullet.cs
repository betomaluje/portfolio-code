using UnityEngine;
using BerserkPixel.Health;
using BerserkPixel.Utils;

namespace Weapons.DesignEx {
    /// <summary>
    /// The golden AOE explosion bullet spawned by the Dragon Burst Gauntlet at 3 hit stacks.
    /// On spawn, immediately pulses outwards and deals damage to all enemies in a radius.
    /// Inspired by Goku's Dragon Fist / MHA's Detroit Smash.
    /// </summary>
    public class DragonBurstExplosionBullet : BaseBullet {
        [Tooltip("Radius of the golden energy burst.")]
        [SerializeField] private float _explosionRadius = 3.5f;

        [Tooltip("Target layer for AOE explosion detection.")]
        [SerializeField] private LayerMask _aoeTargetMask;

        [Tooltip("Lifetime in seconds before self-destruction.")]
        [SerializeField] private float _lifetime = 0.3f;

        private float _timer;
        private bool _hasExploded = false;

        private void OnEnable() {
            _timer = 0f;
            _hasExploded = false;
        }

        private void Update() {
            _timer += Time.deltaTime;
            if (_timer >= _lifetime && !_hasExploded) {
                Destroy(gameObject);
            }
        }

        public override void Fire(Vector2 direction) {
            // This bullet is a stationary burst — no movement needed.
            // Keep it in place at the hit position.
            if (_rb != null) {
                _rb.linearVelocity = Vector2.zero;
                _rb.bodyType = RigidbodyType2D.Static;
            }

            Explode();
        }

        private void Explode() {
            if (_hasExploded) return;
            _hasExploded = true;

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _explosionRadius, _aoeTargetMask);
            foreach (var col in hits) {
                if (col.TryGetComponent<CharacterHealth>(out var health) && !health.IsDead) {
                    var dir = (col.transform.position - transform.position).normalized;
                    var hitData = new HitDataBuilder()
                        .WithWeapon(_weapon)
                        .WithDamage(GetDamage())
                        .WithDirection(dir)
                        .Build(transform, col.transform);

                    health.PerformDamage(hitData);
                }
            }

            SpawnCollisionParticles(transform.position);
            PlayImpactSound(transform.position, "explosion");
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            Gizmos.color = new Color(1f, 0.8f, 0f, 0.4f);
            Gizmos.DrawWireSphere(transform.position, _explosionRadius);
        }
#endif
    }
}
