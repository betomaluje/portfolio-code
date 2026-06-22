using System.Collections.Generic;
using System.Linq;
using BerserkPixel.Health;
using Extensions;
using UnityEngine;

namespace Weapons {
    /// <summary>
    /// A projectile that jumps between multiple targets upon impact.
    /// Each jump can potentially deal reduced damage.
    /// </summary>
    public class ChainBullet : BaseBullet {
        [Tooltip("How many times the bullet can jump between targets.")]
        [SerializeField] private int _maxJumps = 3;

        [Tooltip("The radius to search for the next target.")]
        [SerializeField] private float _jumpRadius = 5f;

        [Tooltip("Damage multiplier per jump (e.g., 0.8 means 80% damage of the previous hit).")]
        [SerializeField] private float _damageMultiplierPerJump = 0.8f;

        [Tooltip("The visual delay before jumping to the next target.")]
        [SerializeField] private float _jumpDelay = 0.1f;

        private int _currentJumps = 0;
        private readonly List<Collider2D> _hitTargets = new();
        private float _currentDamage;

        /// <summary>
        /// Initializes the bullet and its damage.
        /// </summary>
        private void Start() {
            // We initialize with the weapon's default damage
            _currentDamage = _rb != null ? GetDamage() : 0;
        }

        /// <summary>
        /// Logic for when the bullet hits a target. 
        /// Overrides the default behavior to handle the chain jumping.
        /// </summary>
        /// <param name="other">The collider hit.</param>
        private void OnTriggerEnter2D(Collider2D other) {
            if (_hitTargets.Contains(other)) return;

            if (CheckCollision(other)) {
                _hitTargets.Add(other);
                HandleChainJump(other.transform.position);
            }
        }

        /// <summary>
        /// Decides whether to jump to a new target or destroy the projectile.
        /// </summary>
        /// <param name="lastHitPosition">The position of the last enemy hit.</param>
        private void HandleChainJump(Vector2 lastHitPosition) {
            if (_currentJumps >= _maxJumps) {
                DestroyBullet();
                return;
            }

            _currentJumps++;
            _currentDamage *= _damageMultiplierPerJump;
            SetDamage(Mathf.CeilToInt(_currentDamage));

            Collider2D nextTarget = FindNextTarget(lastHitPosition);
            if (nextTarget != null) {
                StartCoroutine(JumpToTarget(lastHitPosition, nextTarget));
            } else {
                DestroyBullet();
            }
        }

        private System.Collections.IEnumerator JumpToTarget(Vector2 lastHitPosition, Collider2D nextTarget) {
            if (_jumpDelay > 0) {
                yield return new WaitForSeconds(_jumpDelay);
            }

            if (nextTarget == null) {
                DestroyBullet();
                yield break;
            }

            // We use a small delay or immediate movement depending on desired feel
            // For now, we move the bullet to the last hit position and fire it towards the next
            transform.position = lastHitPosition;
            Vector2 direction = ((Vector2)nextTarget.transform.position - lastHitPosition).normalized;

            // Reset velocity before firing again
            if (_rb != null) {
                _rb.linearVelocity = Vector2.zero;
            }
            Fire(direction);
        }

        /// <summary>
        /// Searches for the closest valid target within the jump radius that hasn't been hit yet.
        /// </summary>
        /// <param name="position">The center of the search radius.</param>
        /// <returns>The closest valid Collider2D, or null if none found.</returns>
        private Collider2D FindNextTarget(Vector2 position) {
            Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(position, _jumpRadius, _targetMask);
            
            return potentialTargets
                .Where(c => !_hitTargets.Contains(c) && c.TryGetComponent<CharacterHealth>(out var health) && !health.IsDead)
                .OrderBy(c => Vector2.Distance(position, c.transform.position))
                .FirstOrDefault();
        }

        /// <summary>
        /// Cleans up the bullet with effects.
        /// </summary>
        private void DestroyBullet() {
            SpawnCollisionParticles(transform.position);
            PlayImpactSound(transform.position);
            Destroy(gameObject);
        }

        /// <summary>
        /// Visualizes the jump radius in the editor.
        /// </summary>
        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _jumpRadius);
        }
    }
}
