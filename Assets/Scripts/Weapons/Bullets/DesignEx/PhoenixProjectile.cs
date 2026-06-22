using UnityEngine;
using BerserkPixel.Health;
using System.Collections.Generic;

namespace Weapons.DesignEx {
    /// <summary>
    /// A giant fiery phoenix that travels and returns to the owner.
    /// Hits all enemies along its path. Returns health to the owner on touch.
    /// </summary>
    public class PhoenixProjectile : BaseBullet {
        
        [Header("Flame VFX")]
        [SerializeField] private GameObject _fireTrail;
        
        private int _healingAmount;
        private bool _isReturning = false;
        private float _maxDistance = 12f;
        private Vector3 _startPosition;
        private HashSet<CharacterHealth> _alreadyHit = new();

        public void InitializePhoenix(int healing) {
            _healingAmount = healing;
            _startPosition = transform.position;
        }

        public override void Fire(Vector2 direction) {
            if (_rb != null) {
                // High speed, pierces through
                _rb.linearVelocity = direction.normalized * GetSpeed();
            }
        }

        private void Update() {
            if (_owner == null) {
                Destroy(gameObject);
                return;
            }

            if (!_isReturning) {
                if (Vector3.Distance(_startPosition, transform.position) >= _maxDistance) {
                    InitiateReturn();
                }
            } else {
                // Flying back to owner
                Vector2 dirToOwner = (_owner.position - transform.position).normalized;
                _rb.linearVelocity = dirToOwner * GetSpeed() * 1.5f;

                if (Vector2.Distance(transform.position, _owner.position) < 0.6f) {
                    // Trigger return effect!
                    if (_weapon is PhoenixStaffWeapon staff) {
                        staff.HandleReturnHealing(_owner);
                    }
                    Destroy(gameObject);
                }
            }
        }

        private void InitiateReturn() {
            _isReturning = true;
            _alreadyHit.Clear(); // Can hit enemies again on the way back!
            
            // Should play a visual transformation sound
            PlayImpactSound(transform.position, "phoenix_rebirth_snap");
        }

        private void OnTriggerEnter2D(Collider2D other) {
            // Hit an enemy - don't destroy, just pass through!
            if (other.TryGetComponent<CharacterHealth>(out var health) && !health.IsDead) {
                if (_alreadyHit.Contains(health)) return;
                
                _alreadyHit.Add(health);
                
                var phoenixHit = new HitDataBuilder()
                    .WithWeapon(_weapon)
                    .WithDamage(GetDamage())
                    .WithDirection(_rb.linearVelocity.normalized)
                    .Build(_owner, other.transform);
                    
                health.PerformDamage(phoenixHit);
                
                PlayImpactSound(other.transform.position, "fire_burn_impact");
                SpawnCollisionParticles(other.transform.position);
            } else if (((1 << other.gameObject.layer) & LayerMask.GetMask("Walls")) != 0) {
                InitiateReturn();
            }
        }
    }
}
