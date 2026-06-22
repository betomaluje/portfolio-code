using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// Bounces off walls. Under its final bounce, it initiates a homing trajectory back to the owner.
    /// If it touches the owner, it resets their cooldown.
    /// </summary>
    public class RicochetChakramBullet : BaseBullet {
        [Tooltip("How many bounces before it decides to return to the player.")]
        [SerializeField] [Min(1)] private int _maxBounces = 3;

        [Tooltip("The speed multiplier when homing back to the owner.")]
        [SerializeField] private float _returnSpeedMultiplier = 1.5f;

        private int _currentBounces = 0;
        private bool _isReturning = false;
        private RicochetChakramWeapon _chakramWeapon;

        public void InitializeChakram(RicochetChakramWeapon weaponRef) {
            _chakramWeapon = weaponRef;
        }

        // We use Update to handle the returning state since Physics reflection ends after we hit max bounces.
        private void Update() {
            if (_isReturning && _owner != null) {
                // Home towards the owner
                Vector2 returnDir = (_owner.position - transform.position).normalized;
                _rb.linearVelocity = returnDir * (_speed * _returnSpeedMultiplier);

                // Rotate to face owner
                float angle = Mathf.Atan2(returnDir.y, returnDir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            // If it hits the player while returning
            if (_isReturning && collision.transform == _owner) {
                _chakramWeapon?.CatchChakram(); // Mastery Hook: Reset cooldown!
                PlayImpactSound(transform.position, "catch_sound");
                Destroy(gameObject);
                return;
            }

            // Deal standard damage checking
            bool hitExpectedTarget = CheckCollision(collision.collider);

            if (!_isReturning) {
                _currentBounces++;
                
                if (_currentBounces >= _maxBounces) {
                    _isReturning = true;
                    // Physics layers should ideally be adjusted so returning bullets don't hit enemies again,
                    // but depending on your layer matrix, this might piece through them on the way back!
                    return; 
                }

                // If still bouncing, reflect physically
                if (collision.contacts.Length > 0) {
                    Vector2 normal = collision.contacts[0].normal;
                    Vector2 reflected = Vector2.Reflect(_rb.linearVelocity.normalized, normal);
                    _rb.linearVelocity = reflected * _speed;
                    
                    float angle = Mathf.Atan2(reflected.y, reflected.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                }
            }
        }
    }
}
