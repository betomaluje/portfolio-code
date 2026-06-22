using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// A persistent organic swarmer entity spawned by the Living Hive Gun.
    /// Mastery: It responds to the player's pheromone (Charge/Aim).
    /// </summary>
    public class StingerBitComponent : BaseBullet {
        
        [Header("AI Behavior")]
        [SerializeField] private float _driftRadius = 3f;
        [SerializeField] private float _attackRadius = 8f;
        [Tooltip("Amount of seconds the stinger remains active before returning to the hive (destroying itself).")]
        [SerializeField] private float _lifetime = 10f;

        private bool _isConcentrating = false;
        private Vector2 _currentAimDir;
        private float _startTime;

        /// <summary>
        /// Initializer for speed and weapon-link.
        /// </summary>
        public void InitializeSwarm(float speed, LivingHiveGun weapon) {
            _speed = speed;
            _startTime = Time.time;
        }

        /// <summary>
        /// Set by the weapon based on the player's ICharge state.
        /// </summary>
        public void SetTargetState(bool isCinching, Vector2 direction) {
            _isConcentrating = isCinching;
            _currentAimDir = direction;
        }

        private void Update() {
            if (_owner == null || Time.time >= _startTime + _lifetime) {
                Destroy(gameObject);
                return;
            }

            Vector3 targetPos;
            if (_isConcentrating) {
                // MOVE TOWARDS A POINT in front of the player (the aim focus)
                targetPos = _owner.position + (Vector3)_currentAimDir * _attackRadius;
            } else {
                // DRIFT AROUND PLAYER in a lazy orbit or cloud
                float noiseX = Mathf.PerlinNoise(Time.time * 0.5f, 0f) - 0.5f;
                float noiseY = Mathf.PerlinNoise(0f, Time.time * 0.5f) - 0.5f;
                targetPos = _owner.position + new Vector3(noiseX, noiseY, 0) * _driftRadius;
            }

            // Smooth movement towards the determined point
            transform.position = Vector3.MoveTowards(transform.position, targetPos, _speed * Time.deltaTime);
            
            // Look in movement direction
            if (_rb != null && (_isConcentrating || Vector3.Distance(transform.position, targetPos) > 0.1f)) {
                Vector2 moveDir = (targetPos - transform.position).normalized;
                float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }

        /// <summary>
        /// Automatic collision detection for anything the swarmer touches during its flight.
        /// </summary>
        private void OnTriggerStay2D(Collider2D other) {
            // Apply damage if we overlap an enemy
            if (CheckCollision(other)) {
                // If it's concentrating, it resets its focus after a hit to not stick on one target
                if (_isConcentrating) {
                     // Minor bounce or recoil effect
                }
            }
        }
    }
}
