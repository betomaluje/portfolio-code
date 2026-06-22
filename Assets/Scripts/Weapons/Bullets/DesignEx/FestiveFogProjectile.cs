using UnityEngine;
using BerserkPixel.Health;
using System.Collections.Generic;

namespace Weapons.DesignEx {
    /// <summary>
    /// A lobbed bottle or orb that breaks and releases a toxin fog cloud.
    /// Mechanics: Cloud creates tick-damage and applies a slow status.
    /// </summary>
    public class FestiveFogProjectile : BaseBullet {
        
        [Header("Cloud Prefab")]
        [SerializeField] private GameObject _cloudVFXPrefab;
        
        private float _radius;
        private float _duration;
        private float _tickRate;
        private bool _isDeployed = false;

        public void InitializeFog(float radius, float duration, float tickRate) {
            _radius = radius;
            _duration = duration;
            _tickRate = tickRate;
        }

        public override void Fire(Vector2 direction) {
            if (_rb != null) {
                // High arc lob
                _rb.linearVelocity = (direction.normalized * GetSpeed()) + Vector2.up * (GetSpeed() * 0.5f);
                _rb.gravityScale = 2.0f;
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (_isDeployed) return;

            // Hits anything, break and release fog
            DeployFogPortal();
        }

        private void DeployFogPortal() {
            _isDeployed = true;
            _rb.linearVelocity = Vector2.zero;
            _rb.bodyType = RigidbodyType2D.Kinematic;

            if (_cloudVFXPrefab != null) {
                var cloud = Instantiate(_cloudVFXPrefab, transform.position, Quaternion.identity);
                cloud.transform.localScale = Vector3.one * (_radius * 2);
                Destroy(cloud, _duration);
            }

            // Start tick damage cycle
            InvokeRepeating(nameof(TickDamage), 0f, _tickRate);
            
            // Auto cleanup after duration
            Destroy(gameObject, _duration);
            
            PlayImpactSound(transform.position, "glass_shatter_wine");
        }

        private void TickDamage() {
            Collider2D[] results = Physics2D.OverlapCircleAll(transform.position, _radius, _targetMask);
            
            foreach (var col in results) {
                if (col.TryGetComponent<CharacterHealth>(out var health) && !health.IsDead) {
                    var tickHit = new HitDataBuilder()
                        .WithWeapon(_weapon)
                        .WithDamage(Mathf.CeilToInt(GetDamage() * 0.25f)) // Tick is a fraction of base weapon dmg
                        .WithDirection(Vector3.down)
                        .Build(_owner, col.transform);
                    
                    health.PerformDamage(tickHit);
                }
            }
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}
