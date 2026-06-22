using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// A bullet script that tethers an enemy hit by attaching to their transform
    /// and optionally dragging them towards the owner.
    /// </summary>
    public class TetherHarpoonBullet : BaseBullet {
        
        [Tooltip("The speed at which the tethered enemy is pulled.")]
        [SerializeField] private float _pullStrength = 15f;

        private Rigidbody2D _tetheredRb;
        private bool _isPulling = false;
        
        // Property for the Weapon to know if we successfully hooked someone
        public bool IsAttached { get; private set; } = false;

        private void OnTriggerEnter2D(Collider2D other) {
            if (IsAttached || _isPulling) return; // Prevent double grabs

            if (CheckCollision(other)) {
                SpawnCollisionParticles(other.transform.position);
                PlayImpactSound(other.transform.position);
                
                // If it survives hit data, start pulling it
                if (other.TryGetComponent<Rigidbody2D>(out _tetheredRb)) {
                    IsAttached = true;
                    _rb.linearVelocity = Vector2.zero; // Stop bullet movement
                    transform.SetParent(other.transform); // Attach to enemy visually
                } else {
                    Destroy(gameObject); // Enemy can't be pulled
                }
            }
        }

        /// <summary>
        /// Called by the Weapon script when the player recasts the ability.
        /// </summary>
        public void ExecutePull() {
            if (IsAttached) {
                _isPulling = true;
                // Play a yank sound/FX here
            }
        }

        private void Update() {
            // Drag enemy backwards using physical force
            if (_isPulling && _tetheredRb != null && _owner != null) {
                Vector2 pullDirection = (_owner.position - _tetheredRb.transform.position).normalized;
                _tetheredRb.AddForce(pullDirection * _pullStrength * Time.deltaTime, ForceMode2D.Impulse);
                
                // Snap tether if close enough
                if (Vector2.Distance(_owner.position, _tetheredRb.transform.position) < 1.5f) {
                    Destroy(gameObject);
                }
            }
            else if (IsAttached && _tetheredRb == null) {
                // Enemy died before we pulled them or while pulling
                Destroy(gameObject); 
            }
        }
    }
}
