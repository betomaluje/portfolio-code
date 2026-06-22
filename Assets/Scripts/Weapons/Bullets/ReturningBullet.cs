using UnityEngine;
using BerserkPixel.Utils;
using DG.Tweening;

namespace Weapons {
    /// <summary>
    /// A projectile that travels a specific distance and then returns to the shooter.
    /// It can deal damage both while traveling away and while returning.
    /// </summary>
    public class ReturningBullet : BaseBullet {
        [Tooltip("Maximum distance to travel before returning.")]
        [SerializeField] private float _maxDistance = 10f;

        [Tooltip("How fast the bullet returns to the owner.")]
        [SerializeField] private float _returnSpeed = 15f;
        
        [Tooltip("Cooldown for damage so it doesn't hit the same target rapidly.")]
        [SerializeField] private float _damageCooldown = 0.2f;

        private Vector2 _startPosition;
        private bool _isReturning = false;
        private float _lastDamageTime;

        /// <summary>
        /// Records the starting position of the projectile.
        /// </summary>
        private void Start() {
            _startPosition = transform.position;
        }

        /// <summary>
        /// Manages the bullet's movement toward its destination and back to the owner.
        /// </summary>
        private void Update() {
            if (!_isReturning) {
                // Check if max distance is reached
                if (Vector2.Distance(_startPosition, transform.position) >= _maxDistance) {
                    StartReturn();
                }
            } else {
                HandleReturnMovement();
            }
        }

        /// <summary>
        /// Shifts the bullet state to returning.
        /// </summary>
        private void StartReturn() {
            _isReturning = true;
            _rb.linearVelocity = Vector2.zero; // Stop physics movement
        }

        /// <summary>
        /// Moves the bullet towards the owner. If owner is lost, destroy the bullet.
        /// </summary>
        private void HandleReturnMovement() {
            if (_owner == null) {
                Destroy(gameObject);
                return;
            }

            Vector2 direction = ((Vector2)_owner.position - (Vector2)transform.position).normalized;
            transform.position += (Vector3)direction * _returnSpeed * Time.deltaTime;

            // Rotate bullet to face the owner while returning
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // If it reaches the owner, destroy it
            if (Vector2.Distance(transform.position, _owner.position) < 0.5f) {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Logic for when the bullet hits a target. Allows multiple hits with a small cooldown.
        /// </summary>
        /// <param name="other">The collider hit.</param>
        private void OnTriggerEnter2D(Collider2D other) {
            if (Time.time < _lastDamageTime + _damageCooldown) return;

            if (CheckCollision(other)) {
                _lastDamageTime = Time.time;
                SpawnCollisionParticles(other.transform.position);
                PlayImpactSound(other.transform.position);
                
                // If it hits an obstacle while going out, it might return early
                if (!_isReturning && !_targetMask.LayerMatchesObject(other)) {
                     StartReturn();
                }
            }
        }
    }
}
