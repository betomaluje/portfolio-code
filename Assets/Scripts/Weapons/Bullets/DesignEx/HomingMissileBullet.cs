using UnityEngine;
using BerserkPixel.Health;
using System.Collections.Generic;

namespace Weapons.DesignEx {
    /// <summary>
    /// A small homing missile that identifies and tracks enemies within its search radius.
    /// Mechanics: High speed with steering. Explodes on contact with target or wall.
    /// </summary>
    public class HomingMissileBullet : BaseBullet {
        
        [Header("Missile Homing")]
        [Tooltip("The angle in degrees per second the missile can turn towards a target.")]
        [SerializeField] private float _turnSpeed = 180.0f;
        
        [Tooltip("Search radius for potential targets.")]
        [SerializeField] private float _searchRadius = 15.0f;

        private Transform _target;
        private bool _isFired = false;
        private Vector2 _currentDir;
        private float _startTime;

        public override void Fire(Vector2 direction) {
            _currentDir = direction.normalized;
            _isFired = true;
            _startTime = Time.time;
            
            if (_rb != null) {
                _rb.linearVelocity = _currentDir * GetSpeed();
            }
        }

        private void FixedUpdate() {
            if (!_isFired) return;

            // Search for target if we don't have one
            if (_target == null || !_target.gameObject.activeInHierarchy) {
                SearchForTarget();
            }

            if (_target != null) {
                // Steering logic
                Vector2 targetDir = (_target.position - transform.position).normalized;
                
                // Gradually rotate current direction towards target direction
                _currentDir = Vector3.RotateTowards(_currentDir, targetDir, _turnSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime, 0.0f);
            }

            // Always update velocity based on direction and speed
            if (_rb != null) {
                _rb.linearVelocity = _currentDir * GetSpeed();
                
                // Rotate visual towards movement direction
                float angle = Mathf.Atan2(_currentDir.y, _currentDir.x) * Mathf.Rad2Deg;
                _rb.rotation = angle - 90f; // Adjusted for upright sprites
            }

            // Max lifespan safety
            if (Time.time >= _startTime + 10f) {
                Destroy(gameObject);
            }
        }

        private void SearchForTarget() {
            Collider2D[] results = Physics2D.OverlapCircleAll(transform.position, _searchRadius, _targetMask);
            
            float closestDist = float.MaxValue;
            Transform closest = null;

            foreach (var col in results) {
                float dist = Vector2.Distance(transform.position, col.transform.position);
                if (dist < closestDist) {
                    closestDist = dist;
                    closest = col.transform;
                }
            }
            
            _target = closest;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            // Check for collision with an enemy or wall
            if (CheckCollision(other)) {
                // Hit enemy - handled by BaseBullet.CheckCollision which calls health system
                // We add an explosion on top!
                TriggerExplosion(transform.position);
                Destroy(gameObject);
            } else if (((1 << other.gameObject.layer) & LayerMask.GetMask("Walls")) != 0) {
                TriggerExplosion(transform.position);
                Destroy(gameObject);
            }
        }

        private void TriggerExplosion(Vector2 position) {
            // Add a small AOE effect around the missile hit
            Collider2D[] splatter = Physics2D.OverlapCircleAll(position, 1.5f, _targetMask);
            foreach (var col in splatter) {
                if (col.TryGetComponent<CharacterHealth>(out var health) && !health.IsDead) {
                    var splashHit = new HitDataBuilder()
                        .WithWeapon(_weapon)
                        .WithDamage(Mathf.CeilToInt(GetDamage() * 0.5f))
                        .WithDirection(Vector3.up)
                        .Build(_owner, col.transform);
                        
                    health.PerformDamage(splashHit);
                }
            }

            PlayImpactSound(position, "missile_impact_detonate");
            SpawnCollisionParticles(position);
        }
    }
}
