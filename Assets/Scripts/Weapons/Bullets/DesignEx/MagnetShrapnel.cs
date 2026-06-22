using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// Metal shrapnel that lodges in objects and can be recalled magnetically.
    /// Mastery: Returning shards deal high damage to enemies on the return path.
    /// </summary>
    public class MagnetShrapnel : BaseBullet {
        
        [Tooltip("The speed at which the shard returns when magnetized.")]
        [SerializeField] private float _returnSpeedMult = 2.0f;

        private bool _isStuck = false;
        private bool _isReturning = false;

        public override void Fire(Vector2 direction) {
            _rb = GetComponent<Rigidbody2D>();
            if (_rb != null) {
                _rb.linearVelocity = direction.normalized * GetSpeed();
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            // If already stuck, do nothing unless returning
            if (_isStuck && !_isReturning) return;

            // Apply base damage logic
            if (CheckCollision(other)) {
                // If it hits a target (enemy) while returning, it deals extra damage or maintains momentum
                // If it hits a target while firing, it sticks to them!
                if (!_isReturning) {
                    Lodge(other);
                }
            }
            else {
                // If it hit a wall layer (assuming non-target mask), it still sticks
                Lodge(other);
            }
        }

        private void Lodge(Collider2D collider) {
            _isStuck = true;
            _rb.linearVelocity = Vector2.zero;
            _rb.bodyType = RigidbodyType2D.Kinematic;
            transform.SetParent(collider.transform);
        }

        /// <summary>
        /// Triggered by the MagneticCannon's ICharge field.
        /// </summary>
        public void ReturnToOwner() {
            if (_owner == null) return;
            
            _isStuck = false;
            _isReturning = true;
            transform.SetParent(null);
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _rb.gravityScale = 0f;
        }

        private void Update() {
            if (_isReturning) {
                if (_owner == null) {
                    Destroy(gameObject);
                    return;
                }

                // Homeward-bound at high speed
                Vector2 dir = (_owner.position - transform.position).normalized;
                _rb.linearVelocity = dir * GetSpeed() * _returnSpeedMult;

                // Snap distance
                if (Vector2.Distance(transform.position, _owner.position) < 0.5f) {
                    Destroy(gameObject);
                }
            }
        }
    }
}
