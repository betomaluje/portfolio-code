using BerserkPixel.Health;
using BerserkPixel.Utils;
using Extensions;
using Enemies;
using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// The primary chain projectile that roots enemies on impact.
    /// Mastery: Hitting already bound targets causes a massive explosion.
    /// </summary>
    public class CursedChainBullet : BaseBullet {
        [Tooltip("Radius of the chain explosion when hitting an already bound enemy.")]
        [SerializeField] private float _explosionRadius = 3.0f;
        
        private float _duration;
        private GameObject _groundChain;

        public void InitializeBinding(float duration, GameObject chainPrefab) {
            _duration = duration;
            _groundChain = chainPrefab;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (CheckCollision(other)) {
                // Check if target is already bound
                if (other.TryGetComponent<EnemyBindingState>(out var existingState)) {
                    // Refresh stun and detonate
                    existingState.RefreshAndExplode(GetDamage() * 2);
                } else {
                    // Initial Bind
                    var state = other.gameObject.AddComponent<EnemyBindingState>();
                    state.ApplyBind(_duration, _groundChain, _targetMask, _explosionRadius);
                }

                // Consume the projectile
                Destroy(gameObject);
                PlayImpactSound(other.transform.position, "chain_lock");
            }
            else {
                 // Hit wall?
                 if (!_targetMask.LayerMatchesObject(other)) {
                     Destroy(gameObject);
                 }
            }
        }
    }

    /// <summary>
    /// Internal component added to the victim to track and visualizes the binding.
    /// </summary>
    public class EnemyBindingState : MonoBehaviour {
        private GameObject _vfx;
        private float _endTime;
        private LayerMask _targetMask;
        private float _explosionRadius;

        private static readonly Collider2D[] _explosionResults = new Collider2D[20];

        public void ApplyBind(float duration, GameObject chainPrefab, LayerMask targetMask, float explosionRadius) {
            _endTime = Time.time + duration;
            _targetMask = targetMask;
            _explosionRadius = explosionRadius;
            
            // Visual chains erupting
            if (chainPrefab != null) {
                _vfx = Instantiate(chainPrefab, transform.position, Quaternion.identity, transform);
            }

            StunnEnemy();
        }

        private void StunnEnemy() {
            if (TryGetComponent<EnemyStateMachine>(out var stateMachine)) {
                stateMachine.Movement.Stop();
            }
        }

        public void RefreshAndExplode(int explosionDamage) {
            _endTime += 1.0f; // Minor extension
            
            // Create a small burst of damage (e.g., using overlap sphere)            
            // Play detonate FX
            int numColliders = Physics2D.OverlapCircleNonAlloc(transform.position, _explosionRadius, _explosionResults, _targetMask);
            for (int i = 0; i < numColliders; i++) {
                var col = _explosionResults[i];
                if (col != null) {
                    if (col.TryGetComponent<CharacterHealth>(out var health) && !health.IsDead) {
                        Vector2 direction = (col.transform.position - transform.position).normalized;
                        var hitData = new HitDataBuilder()
                            .WithDamage(explosionDamage)
                            .WithDirection(direction)
                            .Build(transform, col.transform);
                        
                        hitData.PerformDamage(col);
                    }
                    _explosionResults[i] = null; // Clear to avoid memory leaks/referencing destroyed objects
                }
            }
        }

        private void Update() {
            if (Time.time >= _endTime) {
                // Release
                if (_vfx != null) Destroy(_vfx);
                
                Destroy(this);
            }
        }
    }
}
