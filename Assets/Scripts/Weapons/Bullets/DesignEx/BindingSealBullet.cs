using UnityEngine;
using BerserkPixel.Health;
using BerserkPixel.Utils;
using System.Collections.Generic;

namespace Weapons.DesignEx {
    /// <summary>
    /// A projectile that forms a binding field upon impact.
    /// It can trap multiple enemies in a magical seal.
    /// </summary>
    public class BindingSealBullet : BaseBullet {
        
        [Header("Seal VFX")]
        [SerializeField] private GameObject _sealFieldPrefab;
        
        private float _duration;
        private float _spreadRadius;
        private bool _isDeployed = false;

        public void InitializeSeal(float duration, float radius) {
            _duration = duration;
            _spreadRadius = radius;
        }

        public override void Fire(Vector2 direction) {
            if (_rb != null) {
                _rb.linearVelocity = direction.normalized * GetSpeed();
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (_isDeployed) return;

            // Hits an enemy or a wall, deploy the field
            if (CheckCollision(other) || _targetMask.LayerMatchesObject(other)) {
                DeploySeal(transform.position);
            }
        }

        private void DeploySeal(Vector3 position) {
            _isDeployed = true;
            _rb.linearVelocity = Vector2.zero;
            _rb.bodyType = RigidbodyType2D.Kinematic;
            
            // Visual seal expansion
            if (_sealFieldPrefab != null) {
                var field = Instantiate(_sealFieldPrefab, position, Quaternion.identity);
                field.transform.localScale = Vector3.one * (_spreadRadius * 2);
                Destroy(field, _duration);
            }

            // Bind current and future targets in radius
            Invoke(nameof(ApplyFinalPop), _duration);
            
            // Visual and audio feedback
            PlayImpactSound(position, "seal_binding");
            SpawnCollisionParticles(position);
        }

        private void Update() {
            if (!_isDeployed) return;

            // While active, it periodically scans and "slows" or "shackles" enemies
            // For now, let's just use it as a gathering field.
        }

        private void ApplyFinalPop() {
            // Re-scan and deal massive damage based on how many enemies were trapped
            Collider2D[] trapped = Physics2D.OverlapCircleAll(transform.position, _spreadRadius, _targetMask);
            
            int count = trapped.Length;
            int finalDamage = Mathf.CeilToInt(GetDamage() * (1f + (count * 0.2f))); // +20% dmg per enemy

            foreach (var col in trapped) {
                if (col.TryGetComponent<CharacterHealth>(out var health) && !health.IsDead) {
                    var popHit = new HitDataBuilder()
                        .WithWeapon(_weapon)
                        .WithDamage(finalDamage)
                        .WithDirection(Vector3.up)
                        .Build(transform, col.transform);
                        
                    health.PerformDamage(popHit);
                }
            }

            // Cleanup
            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _spreadRadius);
        }
    }
}
