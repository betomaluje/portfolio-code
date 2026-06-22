using UnityEngine;
using BerserkPixel.Utils;

namespace Weapons.DesignEx {
    /// <summary>
    /// A piercing beam of spirit energy fired by the SpiritBladeWeapon on a charged release.
    /// Passes through all enemies it touches, destroying itself on wall contact.
    /// Its scale is set externally to reflect the charge level.
    /// Inspired by Goku Black's Azure Slicer / Vegito's Spirit Sword (Dragon Ball Super).
    /// </summary>
    public class SpiritBladeBullet : BaseBullet {
        [Tooltip("Maximum number of enemies this beam can pierce through.")]
        [SerializeField] private int _maxPierce = 8;

        private int _currentPierces = 0;

        public override void Fire(Vector2 direction) {
            _currentPierces = 0;

            if (_rb != null) {
                _rb.linearVelocity = direction.normalized * GetSpeed();

                // Rotate to face the travel direction
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
                // Hit a wall or non-target — dissipate
                if (!_targetMask.LayerMatchesObject(other)) {
                    Destroy(gameObject);
                }
            }
        }
    }
}
