using UnityEngine;
using BerserkPixel.Health;

namespace Weapons.DesignEx {
    /// <summary>
    /// A slow sonic wave that reflects off static obstacles.
    /// Triggers area-of-effect ripple when bouncing to reward geometry-based positioning.
    /// </summary>
    public class SonicEchoBullet : BaseBullet {
        
        [Header("Resonance Logic")]
        [SerializeField] private GameObject _rippleVFXPrefab;
        [SerializeField] private float _rippleDamageMult = 0.5f;

        private float _rippleRadius = 2.5f;
        private int _maxEchoes = 2;
        private int _currentEchoes = 0;
        private Vector2 _velocity;

        /// <summary>
        /// Initializer for resonance specific configuration.
        /// </summary>
        public void InitializeResonance(float radius, int echoes) {
            _rippleRadius = radius;
            _maxEchoes = echoes;
        }

        public override void Fire(Vector2 direction) {
            _velocity = direction.normalized * GetSpeed();
            if (_rb != null) {
                _rb.linearVelocity = _velocity;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            // Check for Wall / Static terrain
            // Using layer logic or simple tag check if layer mask isn't available
            // If it's a world wall: 
            if (_currentEchoes < _maxEchoes) {
                _currentEchoes++;
                
                // Reflection over surface normal
                Vector2 normal = collision.contacts[0].normal;
                _velocity = Vector2.Reflect(_velocity, normal);
                _rb.linearVelocity = _velocity;
                
                // Trigger Sound Ripple (AoE damage at bounce point)
                TriggerResonanceRipple(collision.contacts[0].point);
                
                // Rotate to match new velocity
                float angle = Mathf.Atan2(_velocity.y, _velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            else {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            // Apply base wave damage on pass-through
            if (CheckCollision(other)) {
                // Wave is piercing, it doesn't destroy on hit unless it's a wall (Physics Matrix)
                // Spawn minor hit effect
                SpawnCollisionParticles(other.transform.position);
            }
        }

        private void TriggerResonanceRipple(Vector2 position) {
            // Optional VFX instantiation
            if (_rippleVFXPrefab != null) {
                Instantiate(_rippleVFXPrefab, position, Quaternion.identity);
            }

            // Perform Area Damage
            var hits = Physics2D.OverlapCircleAll(position, _rippleRadius, _targetMask);
            foreach (var hit in hits) {
                // Create a minor HitData for the ripple and perform damage
                // This simulates the 'echo' effect
                // (Note: in a production setup, reuse HitDataBuilder here)
                
                // Apply ripple damage using the system's HitData pattern
                if (hit.TryGetComponent<CharacterHealth>(out var health) && !health.IsDead) {
                    var rippleHit = new HitDataBuilder()
                        .WithWeapon(_weapon)
                        .WithDamage(Mathf.CeilToInt(GetDamage() * _rippleDamageMult))
                        .WithDirection((hit.transform.position - transform.position).normalized)
                        .Build(transform, hit.transform);
                        
                    health.PerformDamage(rippleHit);
                }
            }
            
            PlayImpactSound(position, "resonance_ripple");
        }
    }
}
