using UnityEngine;
using BerserkPixel.Utils;

namespace Weapons.DesignEx {
    /// <summary>
    /// A massive, slow-moving dark crescent wave released on a full Getsuga charge.
    /// Pierces through a large number of enemies and deals bonus damage over its travel.
    /// Inspired by Ichigo's Getsuga Tenshō (Bleach).
    /// </summary>
    public class GetsugaHeavyWaveBullet : BaseBullet {
        [Tooltip("How many enemies this heavy wave can pierce before vanishing.")]
        [SerializeField] private int _maxPierce = 12;

        private int _currentPierces = 0;

        public override void Fire(Vector2 direction) {
            _currentPierces = 0;

            if (_rb != null) {
                _rb.linearVelocity = direction.normalized * GetSpeed();

                // Rotate the crescent sprite to face travel direction
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (CheckCollision(other)) {
                _currentPierces++;
                SpawnCollisionParticles(other.transform.position);

                if (_currentPierces >= _maxPierce) {
                    Destroy(gameObject);
                }
            } else {
                // Hit a wall — the wave collapses
                if (!_targetMask.LayerMatchesObject(other)) {
                    Destroy(gameObject);
                }
            }
        }
    }
}
