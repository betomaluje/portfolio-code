using UnityEngine;

namespace Weapons {
    /// <summary>
    /// A projectile that orbits around its owner rather than traveling in a straight line.
    /// Hits enemies that come in close range of the character.
    /// </summary>
    public class OrbitBullet : BaseBullet {
        [Tooltip("How fast the bullet orbits the owner.")]
        [SerializeField] private float _orbitSpeed = 180f; // degrees per second

        [Tooltip("Distance from the owner during orbit.")]
        [SerializeField] private float _orbitRadius = 2f;

        [Tooltip("Lifetime of the orbit bullet.")]
        [SerializeField] private float _lifetime = 5f;

        private float _currentAngle;

        /// <summary>
        /// Registers current time for self-destruction after lifetime.
        /// </summary>
        private void Start() {
            Destroy(gameObject, _lifetime);
            
            // Randomized start angle
            _currentAngle = Random.Range(0, 360f);
        }

        /// <summary>
        /// Constrains the bullet's movement to a circular path around the owner.
        /// If owner is lost, destroy the bullet.
        /// </summary>
        private void Update() {
            if (_owner == null) {
                Destroy(gameObject);
                return;
            }

            // Update orbit angle
            _currentAngle += _orbitSpeed * Time.deltaTime;
            
            // Calculate new position relative to owner
            float angleInRad = _currentAngle * Mathf.Deg2Rad;
            Vector2 offset = new Vector2(Mathf.Cos(angleInRad), Mathf.Sin(angleInRad)) * _orbitRadius;
            
            transform.position = (Vector2)_owner.position + offset;

            // Rotate bullet to look in its current movement direction
            // Perpendicular to the offset vector
            Vector2 movementDir = new Vector2(-Mathf.Sin(angleInRad), Mathf.Cos(angleInRad));
            float rotAngle = Mathf.Atan2(movementDir.y, movementDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(rotAngle, Vector3.forward);
        }

        /// <summary>
        /// Standard hit detection. Multiple hits might be allowed if you use a damage cooldown.
        /// For this simple bullet, it handles damage and destroys upon collision with enemy.
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other) {
            if (CheckCollision(other)) {
                SpawnCollisionParticles(other.transform.position);
                PlayImpactSound(other.transform.position);
                Destroy(gameObject);
            }
        }
    }
}
