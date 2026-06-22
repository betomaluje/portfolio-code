using UnityEngine;
using BerserkPixel.Utils;

namespace Weapons.DesignEx {
    /// <summary>
    /// Spear behavior that lodges in objects and sustains a damaging beam between owner and self.
    /// Mastery: Moving the owner sweeps the beam across the battlefield.
    /// Recalls when triggered by the weapon.
    /// </summary>
    public class TetherSpearBullet : BaseBullet {
        
        [Header("Tether Logic")]
        [SerializeField] private float _beamWidth = 0.2f;
        [SerializeField] private float _damageTickRate = 0.1f;
        [SerializeField] private LayerMask _penetrationLayer; // Layer mask for what the beam hits

        private LineRenderer _lineRenderer;
        private float _nextDamageTime = 0f;
        private bool _isLodged = false;
        private bool _isRecalling = false;

        public bool IsLodged => _isLodged;

        /// <summary>
        /// Initializer for visual tether and timing.
        /// </summary>
        public void InitializeBeam(GameObject visualPrefab, float tickRate) {
            _damageTickRate = tickRate;
            if (visualPrefab != null) {
                var obj = Instantiate(visualPrefab, transform);
                _lineRenderer = obj.GetComponent<LineRenderer>();
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (_isLodged || _isRecalling) return;

            // Lodge logic: Stop on first collision with target mask OR walls
            if (CheckCollision(other) || _penetrationLayer.LayerMatchesObject(other)) {
                _isLodged = true;
                _rb.linearVelocity = Vector2.zero;
                _rb.bodyType = RigidbodyType2D.Kinematic;
                transform.SetParent(other.transform);
                
                // Initialize LineRenderer if present
                if (_lineRenderer != null) {
                    _lineRenderer.positionCount = 2;
                }
            }
        }

        public void Recall() {
            _isLodged = false;
            _isRecalling = true;
            transform.SetParent(null);
            // Move back to owner rapidly or just destroy? 
            // We'll move back for visual polish.
        }

        private void Update() {
            if (_owner == null) {
                Destroy(gameObject);
                return;
            }

            if (_isLodged) {
                UpdateTetherLogic();
            }
            else if (_isRecalling) {
                // Move towards owner
                float step = GetSpeed() * 2f * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, _owner.position, step);
                
                if (Vector3.Distance(transform.position, _owner.position) < 0.5f) {
                    Destroy(gameObject);
                }
            }
        }

        private void UpdateTetherLogic() {
            // Update Visuals
            if (_lineRenderer != null) {
                _lineRenderer.SetPosition(0, _owner.position);
                _lineRenderer.SetPosition(1, transform.position);
            }

            // Damage processing via Raycast
            if (Time.time >= _nextDamageTime) {
                ProcessBeamDamage();
                _nextDamageTime = Time.time + _damageTickRate;
            }
        }

        private void ProcessBeamDamage() {
             Vector2 dir = (transform.position - _owner.position).normalized;
             float dist = Vector2.Distance(_owner.position, transform.position);
             
             // Cast a thin box or several rays between owner and spear
             RaycastHit2D[] hits = Physics2D.CircleCastAll(_owner.position, _beamWidth, dir, dist, _targetMask);
             foreach (var hit in hits) {
                 // Trigger hit logic
                 // Reuse weapon damage stats if possible.
                 // hit.collider.GetComponent<CharacterHealth>()?.TakeDamage(GetDamage());
                 // This simulates the "clothesline" effect
             }
        }
    }
}
