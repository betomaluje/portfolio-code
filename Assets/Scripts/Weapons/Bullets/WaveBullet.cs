using UnityEngine;

namespace Weapons {
    /// <summary>
    /// A projectile that travels in a wave-like (sinusoidal) pattern.
    /// It covers a wider area than a straight bullet and can sometimes "avoid" 
    /// narrow cover while traveling.
    /// </summary>
    public class WaveBullet : BaseBullet {
        [Tooltip("The frequency of the wave oscillation.")]
        [SerializeField] private float _waveFrequency = 10f;

        [Tooltip("The amplitude (height) of the wave oscillation.")]
        [SerializeField] private float _waveAmplitude = 1.5f;

        private float _timeCounter;
        private Vector2 _originalDirection;
        private Vector2 _perpendicularDirection;

        /// <summary>
        /// Initializes the movement directions once the bullet is fired.
        /// </summary>
        /// <param name="direction">The primary forward direction.</param>
        public override void Fire(Vector2 direction) {
            base.Fire(direction);
            _originalDirection = direction.normalized;
            _perpendicularDirection = new Vector2(-_originalDirection.y, _originalDirection.x);
            _timeCounter = 0;
            
            // We set velocity to zero because we handle repositioning in Update for the wave effect
            _rb.linearVelocity = Vector2.zero;
        }

        /// <summary>
        /// Updates the bullet position based on a linear forward movement 
        /// plus an added sine wave offset alongside the perpendicular vector.
        /// </summary>
        private void Update() {
            _timeCounter += Time.deltaTime;

            // Move linear forward
            Vector2 forwardStep = _originalDirection * _speed * Time.deltaTime;
            
            // Calculate sine offset
            float offsetAmount = Mathf.Sin(_timeCounter * _waveFrequency) * _waveAmplitude;
            Vector2 waveStep = _perpendicularDirection * offsetAmount;

            // Update position
            transform.position += (Vector3)(forwardStep + waveStep);

            // Update rotation to face the immediate movement direction
            Vector2 movementVector = forwardStep + waveStep;
            float angle = Mathf.Atan2(movementVector.y, movementVector.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        /// <summary>
        /// Custom hit detection to avoid the Sine wave clipping into walls 
        /// and still correctly damaging enemies.
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other) {
            if (CheckCollision(other)) {
                SpawnCollisionParticles(other.transform.position);
                PlayImpactSound(other.transform.position);
                Destroy(gameObject);
            }
        }
    }
}
