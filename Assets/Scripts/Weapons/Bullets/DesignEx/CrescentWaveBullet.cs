using UnityEngine;
using BerserkPixel.Utils;

namespace Weapons.DesignEx {
    /// <summary>
    /// A wide, crescent-shaped projectile launched on blade swings.
    /// Mastery: Its wide hitbox allows it to hit entire crowds in a single line.
    /// </summary>
    public class CrescentWaveBullet : BaseBullet {
        
        [Tooltip("How many enemies this wave can pass through before vanishing.")]
        [SerializeField] private int _pierceCount = 3;

        private int _currentPierces = 0;

        public override void Fire(Vector2 direction) {
            if (_rb != null) {
                _rb.linearVelocity = direction.normalized * GetSpeed();
                
                // Rotates the wide sprite to face the travel direction
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            // Apply standard damage logic
            if (CheckCollision(other)) {
                _currentPierces++;
                
                // Visual impact effect
                SpawnCollisionParticles(other.transform.position);

                if (_currentPierces > _pierceCount) {
                    Destroy(gameObject);
                }
            }
            else {
                // If it hits a Wall, dissipate
                if (!_targetMask.LayerMatchesObject(other)) {
                     // Collided with terrain or other non-target object
                     Destroy(gameObject);
                }
            }
        }
    }
}
