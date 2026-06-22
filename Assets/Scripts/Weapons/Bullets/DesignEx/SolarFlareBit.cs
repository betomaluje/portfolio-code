using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// Specialized drone acting as a projectile.
    /// Orbits the owner and zaps targets nearby. 
    /// Mastery: Its orbit radius is controlled via player input (Charge value). 
    /// Its fire-rate increases as it is cinched closer to the player.
    /// </summary>
    public class SolarFlareBit : BaseBullet {
        
        [Header("Orbit Management")]
        [SerializeField] private float _rotationSpeed = 180f;

        [Header("Weaponry")]
        [Tooltip("Prefab for the laser projectile fired by the drone.")]
        [SerializeField] private GameObject _laserProjectile;
        [SerializeField] private float _baseFireRate = 1.0f;
        [SerializeField] private float _maxFireRateMult = 3.0f;

        private float _minRadius, _maxRadius;
        private float _currentCinch = 0f;
        private float _nextBitShotTime = 0f;
        private float _angle = 0f;

        /// <summary>
        /// Initializer for radius boundaries.
        /// </summary>
        public void InitializeOrbit(float min, float max) {
            _minRadius = min;
            _maxRadius = max;
        }

        /// <summary>
        /// Adjusts the orbit distance based on player weapon held charge.
        /// 0f = Max distance, 1f = Min distance (Cinch)
        /// </summary>
        public void SetCinchRatio(float ratio) {
            _currentCinch = Mathf.Clamp01(ratio);
        }

        private void Update() {
            if (_owner == null) {
                Destroy(gameObject);
                return;
            }

            // Calculation of current orbit radius
            float currentRadius = Mathf.Lerp(_maxRadius, _minRadius, _currentCinch);

            // Orbit positioning logic
            _angle += _rotationSpeed * Time.deltaTime;
            float rad = _angle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * currentRadius;
            
            transform.position = _owner.position + offset;
            
            // Firing logic
            ProcessAutomaticWeaponry();
        }

        /// <summary>
        /// Automatically targets and zaps nearby enemies based on current cinch state.
        /// Higher cinch = faster fire rate. 
        /// </summary>
        private void ProcessAutomaticWeaponry() {
            if (Time.time < _nextBitShotTime) return;

            // Search for target in range (or the drone itself can have a trigger)
            // For simplicity, we fire in the current orbit tangent or toward the nearest enemy.
            var target = FindNearestTargetInReach();
            if (target != null && _laserProjectile != null) {
                Vector2 dir = (target.position - transform.position).normalized;
                var laserObj = Instantiate(_laserProjectile, transform.position, Quaternion.identity);
                if (laserObj.TryGetComponent<IBullet>(out var bullet)) {
                     // Drone uses its own damage from its config bit or scales with own stats
                     bullet.SetDamage(GetDamage());
                     bullet.SetOwner(_owner);
                     bullet.Fire(dir);
                }

                // Fire Rate scales with Cinch (higher cinch = faster)
                float currentInterval = _baseFireRate / Mathf.Lerp(1.0f, _maxFireRateMult, _currentCinch);
                _nextBitShotTime = Time.time + currentInterval;
                
                PlayImpactSound(transform.position, "bit_fire");
            }
        }

        private Transform FindNearestTargetInReach() {
             // In an actual game, use Physics2D.OverlapCircleAll with _targetMask
             var hits = Physics2D.OverlapCircleAll(transform.position, 5f, _targetMask);
             Transform best = null;
             float bestDist = float.MaxValue;
             foreach (var hit in hits) {
                 float d = Vector2.Distance(transform.position, hit.transform.position);
                 if (d < bestDist) {
                     bestDist = d;
                     best = hit.transform;
                 }
             }
             return best;
        }
    }
}
