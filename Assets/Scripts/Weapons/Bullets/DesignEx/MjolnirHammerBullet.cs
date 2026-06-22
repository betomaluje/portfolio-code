using UnityEngine;
using BerserkPixel.Health;
using System.Collections.Generic;

namespace Weapons.DesignEx {
    /// <summary>
    /// A hammer that travels and returns to the owner.
    /// Handles the lightning chain logic while in its 'Return' state.
    /// </summary>
    public class MjolnirHammerBullet : BaseBullet {
        
        [Header("Lightning VFX")]
        [SerializeField] private LineRenderer _lightningLine;
        
        private float _chainDamageMultiplier;
        private float _maxDistance;
        private Vector3 _startPosition;
        private bool _isReturning = false;
        private float _lastChainTickTime = 0f;
        private float _chainTickRate = 0.2f;

        public void InitializeHammer(float damageMult, float maxDist) {
            _chainDamageMultiplier = damageMult;
            _maxDistance = maxDist;
            _startPosition = transform.position;
        }

        public override void Fire(Vector2 direction) {
            if (_rb != null) {
                _rb.linearVelocity = direction.normalized * GetSpeed();
            }
        }

        private void Update() {
            if (_owner == null) {
                Destroy(gameObject);
                return;
            }

            float distFromStart = Vector3.Distance(_startPosition, transform.position);

            if (!_isReturning) {
                if (distFromStart >= _maxDistance) {
                    InitiateReturn();
                }
            } else {
                // Moving back towards owner
                Vector2 dirToOwner = (_owner.position - transform.position).normalized;
                _rb.linearVelocity = dirToOwner * GetSpeed() * 1.5f;

                // Update Visual Chain
                if (_lightningLine != null) {
                    _lightningLine.enabled = true;
                    _lightningLine.SetPosition(0, transform.position);
                    _lightningLine.SetPosition(1, _owner.position);
                }

                // Periodic damage along the chain path
                ApplyChainPathDamage();

                if (Vector2.Distance(transform.position, _owner.position) < 0.5f) {
                    Destroy(gameObject);
                }
            }
        }

        private void InitiateReturn() {
            _isReturning = true;
            _rb.linearVelocity = Vector2.zero;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            // Impact with an enemy while flying out or back
            if (CheckCollision(other)) {
                if (!_isReturning) {
                    InitiateReturn(); // Hit an enemy, bounce back!
                }
            } else if (((1 << other.gameObject.layer) & LayerMask.GetMask("Walls")) != 0) {
                InitiateReturn(); // Hit a wall, bounce back!
            }
        }

        private void ApplyChainPathDamage() {
            if (Time.time < _lastChainTickTime + _chainTickRate) return;
            _lastChainTickTime = Time.time;

            // Simple raycast or box check along the chain
            Vector2 dir = (_owner.position - transform.position).normalized;
            float dist = Vector2.Distance(transform.position, _owner.position);

            // Using BoxCast for the chain width
            RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, new Vector2(0.5f, 0.5f), 0f, dir, dist, _targetMask);
            
            foreach (var hit in hits) {
                if (hit.collider.TryGetComponent<CharacterHealth>(out var health) && !health.IsDead) {
                    var chainHit = new HitDataBuilder()
                        .WithWeapon(_weapon)
                        .WithDamage(Mathf.CeilToInt(GetDamage() * _chainDamageMultiplier))
                        .WithDirection(dir)
                        .Build(_owner, hit.collider.transform);
                        
                    health.PerformDamage(chainHit);
                }
            }
            
            PlayImpactSound(transform.position, "lightning_chain_snap");
        }
    }
}
