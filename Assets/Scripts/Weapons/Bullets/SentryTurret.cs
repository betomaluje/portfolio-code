using UnityEngine;
using System.Linq;
using BerserkPixel.Health;

namespace Weapons {
    /// <summary>
    /// A spawned object that stays in position and automatically fires 
    /// at nearby enemies until its lifetime expires.
    /// </summary>
    public class SentryTurret : BaseSpawnWeapon {
        [Tooltip("The projectile the sentry fires.")]
        [SerializeField] private GameObject _bulletPrefab;

        [Tooltip("How often the sentry fires.")]
        [SerializeField] private float _fireRate = 1.0f;

        [Tooltip("Detection range of the sentry.")]
        [SerializeField] private float _range = 8f;

        [Tooltip("Lifetime of the sentry before it self-destructs.")]
        [SerializeField] private float _lifetime = 10f;

        private float _nextFireTime;
        private readonly Collider2D[] _targetResults = new Collider2D[10];
        
        // Sentry doesn't move, so we return a null or empty movement config
        protected override MovementConfig MovementConfig => null;

        /// <summary>
        /// Starts the sentry's lifetime countdown.
        /// </summary>
        private void Start() {
            Destroy(gameObject, _lifetime);
        }

        /// <summary>
        /// Scans for enemies and fires if one is in range and the weapon is ready.
        /// </summary>
        private void Update() {
            if (Time.time >= _nextFireTime) {
                Transform target = FindClosestEnemy();
                if (target != null) {
                    FireAt(target);
                    _nextFireTime = Time.time + _fireRate;
                }
                else {
                    // Optimized idle check: only scan ~10 times a second instead of every frame
                    // This saves performance while keeping the turret responsive.
                    _nextFireTime = Time.time + 0.1f;
                }
            }
        }

        /// <summary>
        /// Searches for the nearest valid enemy within detection range.
        /// </summary>
        /// <returns>The closest enemy Transform or null.</returns>
        private Transform FindClosestEnemy() {
            ContactFilter2D filter = new ContactFilter2D { layerMask = _targetMask, useTriggers = true };
            int count = Physics2D.OverlapCircle(transform.position, _range, filter, _targetResults);
            if (count == 0) return null;
            Transform closest = null;
            float minDistance = float.MaxValue;
            for (int i = 0; i < count; i++) {
                var collider = _targetResults[i];
                if (collider.TryGetComponent<CharacterHealth>(out var health) && !health.IsDead) {
                    float distance = Vector2.Distance(transform.position, collider.transform.position);
                    if (distance < minDistance) {
                        minDistance = distance;
                        closest = collider.transform;
                    }
                }
            }
            
            return closest;
        }

        /// <summary>
        /// Spawns a bullet traveling toward the target.
        /// </summary>
        /// <param name="target">Target enemy transform.</param>
        private void FireAt(Transform target) {
            if (_bulletPrefab == null) return;

            Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
            var bulletObj = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
            
            if (bulletObj.TryGetComponent<IBullet>(out var ibullet)) {
                ibullet.SetWeapon(_weapon);
                ibullet.SetDamage(_damage);
                ibullet.SetOwner(transform); // Sentry is the owner of its own bullets
                ibullet.Fire(direction);
            }
        }

        /// <summary>
        /// Required by BaseSpawnWeapon, though the turret uses its own Update logic.
        /// </summary>
        public override void Shoot() {
            // Sentry logic is handled in Update
        }

        /// <summary>
        /// Visualizes the sentry's activation range in the editor.
        /// </summary>
        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _range);
        }
    }
}
