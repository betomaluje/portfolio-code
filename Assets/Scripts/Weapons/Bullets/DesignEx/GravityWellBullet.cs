using UnityEngine;
using BerserkPixel.Health;
using System.Collections.Generic;

namespace Weapons.DesignEx {
    /// <summary>
    /// A slow black hole entity that pulls in enemies and objects.
    /// Mechanics: Periodic pulling force towards center. High final damage on collapse.
    /// </summary>
    public class GravityWellBullet : BaseBullet {
        
        [Header("Singularity FX")]
        [SerializeField] private GameObject _vfxField;
        
        private float _pullForce;
        private float _radius;
        private float _lifespan;
        private float _startTime;
        private bool _isActive = true;

        public void InitializeSingularity(float force, float radius, float lifespan) {
            _pullForce = force;
            _radius = radius;
            _lifespan = lifespan;
            _startTime = Time.time;
        }

        public override void Fire(Vector2 direction) {
            if (_rb != null) {
                // Slower movement, it occupies space!
                _rb.linearVelocity = direction.normalized * GetSpeed() * 0.4f;
            }
        }

        private void FixedUpdate() {
            if (!_isActive) return;

            if (Time.time >= _startTime + _lifespan) {
                Collapse();
                return;
            }

            // Pull in enemies
            Collider2D[] trapped = Physics2D.OverlapCircleAll(transform.position, _radius, _targetMask);
            
            foreach (var col in trapped) {
                // Don't pull ourselves!
                if (col.gameObject == gameObject) continue;

                if (col.TryGetComponent<Rigidbody2D>(out var targetRb)) {
                    // Check if it's an enemy or something we should pull
                    Vector2 pullDir = (transform.position - col.transform.position).normalized;
                    float dist = Vector2.Distance(transform.position, col.transform.position);
                    
                    // Stronger pull the closer it is
                    float forceStrength = Mathf.Lerp(_pullForce, _pullForce * 0.2f, dist / _radius);
                    targetRb.AddForce(pullDir * forceStrength, ForceMode2D.Force);
                }
            }
        }

        private void Collapse() {
            _isActive = false;
            
            // Final burst damage
            Collider2D[] results = Physics2D.OverlapCircleAll(transform.position, _radius * 0.8f, _targetMask);
            foreach (var col in results) {
                if (col.TryGetComponent<CharacterHealth>(out var health) && !health.IsDead) {
                    var collapseHit = new HitDataBuilder()
                        .WithWeapon(_weapon)
                        .WithDamage(Mathf.CeilToInt(GetDamage() * 2.0f)) // High damage burst
                        .WithDirection(Vector3.up)
                        .Build(_owner, col.transform);
                        
                    health.PerformDamage(collapseHit);
                }
            }

            // Visual feedback
            PlayImpactSound(transform.position, "singularity_collapse");
            SpawnCollisionParticles(transform.position);
            
            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}
