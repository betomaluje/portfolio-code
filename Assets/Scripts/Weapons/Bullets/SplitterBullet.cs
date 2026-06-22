using UnityEngine;

namespace Weapons {
    /// <summary>
    /// A projectile that travels a specific distance or hits a target, 
    /// then splits into multiple smaller "fragment" bullets.
    /// Fragmented bullets carry a portion of the original damage.
    /// </summary>
    public class SplitterBullet : BaseBullet {
        [Tooltip("The projectile to spawn when this one fragments.")]
        [SerializeField] private GameObject _fragmentPrefab;

        [Tooltip("Number of fragment bullets to spawn.")]
        [SerializeField] private int _numberOfFragments = 3;

        [Tooltip("Distance this projectile travels before automatically splitting.")]
        [SerializeField] private float _splitDistance = 6f;

        [Tooltip("The arc angle over which the fragments are spread.")]
        [SerializeField] private float _fragmentArcAngle = 120f;

        [Tooltip("Coefficient to multiply the original damage by for fragments.")]
        [SerializeField] private float _fragmentDamageMultiplier = 0.5f;

        private Vector2 _startPosition;
        private bool _hasFragmented = false;

        /// <summary>
        /// Captures the start position for distance tracking.
        /// </summary>
        private void Start() {
            _startPosition = transform.position;
        }

        /// <summary>
        /// Tracks distance and triggers fragmentation if threshold reached.
        /// </summary>
        private void Update() {
            if (!_hasFragmented && Vector2.Distance(_startPosition, transform.position) >= _splitDistance) {
                HandleFragmentation();
            }
        }

        /// <summary>
        /// Fragments immediately upon hitting a target.
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other) {
            if (_hasFragmented) return;

            if (CheckCollision(other)) {
                HandleFragmentation();
            }
        }

        /// <summary>
        /// Logic for spawning fragment projectiles in an arc.
        /// Fragments inherit the owner and a portion of the original damage.
        /// </summary>
        private void HandleFragmentation() {
            if (_hasFragmented || _fragmentPrefab == null) return;
            _hasFragmented = true;

            // Base direction is current velocity
            Vector2 currentDir = _rb.linearVelocity.normalized;
            if (currentDir == Vector2.zero) currentDir = transform.right;

            float startAngle = -_fragmentArcAngle / 2f;
            float angleStep = _fragmentArcAngle / (_numberOfFragments - 1 == 0 ? 1 : _numberOfFragments - 1);

            int fragmentDamage = Mathf.CeilToInt(GetDamage() * _fragmentDamageMultiplier);

            for (int i = 0; i < _numberOfFragments; i++) {
                float currentAngle = startAngle + (angleStep * i);
                Vector2 dir = RotateVector(currentDir, currentAngle);

                SpawnFragment(dir, fragmentDamage);
            }

            Destroy(gameObject);
        }

        /// <summary>
        /// Spawns an individual fragment and sets its weapon/owner parameters.
        /// </summary>
        private void SpawnFragment(Vector2 dir, int damage) {
            var fragmentInstance = Instantiate(_fragmentPrefab, transform.position, Quaternion.identity);
            if (fragmentInstance.TryGetComponent<IBullet>(out var ibullet)) {
                // Ensure the base weapon is passed even though this instance is destroyed
                ibullet.SetDamage(damage);
                ibullet.SetOwner(_owner);
                ibullet.Fire(dir);
            }
        }

        /// <summary>
        /// Utility to rotate a 2D vector by a specific degrees angle.
        /// </summary>
        private Vector2 RotateVector(Vector2 v, float degrees) {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
            return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
        }
    }
}
