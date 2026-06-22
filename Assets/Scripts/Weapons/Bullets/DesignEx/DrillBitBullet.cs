using BerserkPixel.Utils;
using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// Specialized projectile for the Railgun that doesn't self-destruct on hit.
    /// Instead, it consumes momentum to pierce and then speeds back up.
    /// </summary>
    public class DrillBitBullet : BaseBullet {
        [Tooltip("Max targets to pierce before destroying itself.")]
        [SerializeField] private int _maxPierces = 5;

        private float _speedMultiplier = 1.25f;
        private int _currentPierces = 0;

        /// <summary>
        /// Initializer for Railgun specific data.
        /// </summary>
        public void InitializeDrill(float multiplier) {
            _speedMultiplier = multiplier;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            // Apply damage via the base system
            if (CheckCollision(other)) {
                _currentPierces++;
                
                // Acceleration effect
                float newSpeed = GetSpeed() * _speedMultiplier;
                SetSpeed(newSpeed);

                // Update physical velocity immediately
                if (_rb != null) {
                    _rb.linearVelocity = _rb.linearVelocity.normalized * newSpeed;
                }

                SpawnCollisionParticles(transform.position);
                PlayImpactSound(transform.position, "drill_hit"); // Custom sound if possible

                if (_currentPierces >= _maxPierces) {
                    Destroy(gameObject);
                }
            }
            else {
                // If we hit a Wall (Static terrain), the drill usually stops.
                // Assuming TargetMask is for Enemies, we'd need another check for Wall layers.
                // We'll trust the physics matrix to stop us if they overlap correctly.
                if (!_targetMask.LayerMatchesObject(other)) {
                     // Hit something that isn't a target (e.g., Wall)
                     Destroy(gameObject);
                }
            }
        }
    }
}
