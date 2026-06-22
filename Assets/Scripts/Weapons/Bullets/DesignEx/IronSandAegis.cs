using BerserkPixel.Utils;
using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// Defensive/Offensive sand drone that orbits the owner.
    /// Mastery: Cinching (ICharge) solidifies the cloud into a high-damage spear.
    /// </summary>
    public class IronSandAegis : BaseBullet {
        
        [Header("Defense Properties")]
        [SerializeField] private float _rotationSpeed = 120f;
        [SerializeField] private float _baseRadius = 3.5f;

        [Header("Projectile Conversion")]
        [Tooltip("The high-damage spear spawned when solidified.")]
        [SerializeField] private GameObject _sandSpearPrefab;

        private float _currentCinch = 0f;
        private float _angle = 0f;
        private int _totalFrags = 3;

        /// <summary>
        /// Initializer for defense count.
        /// </summary>
        public void InitializeDefense(int frags) {
            _totalFrags = frags;
        }

        /// <summary>
        /// Set by ICharge input. Higher cinch pulls the cloud closer for solidification.
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
            float currentRadius = Mathf.Lerp(_baseRadius, 0.5f, _currentCinch);

            // Orbit positioning
            _angle += _rotationSpeed * Time.deltaTime;
            float rad = _angle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * currentRadius;
            
            transform.position = _owner.position + offset;
            
            // Visual feedback of solidification: rotate faster or make it brighter when cinched
        }

        /// <summary>
        /// Called by the Weapon when Attack is triggered at high charge.
        /// Turns the defense into a projectile.
        /// </summary>
        public void FireSolidifiedSpear(Vector2 direction) {
            if (_sandSpearPrefab != null) {
                var spear = Instantiate(_sandSpearPrefab, transform.position, Quaternion.identity);
                if (spear.TryGetComponent<IBullet>(out var bullet)) {
                     // Spear deals massive damage based on the weapon's base
                     bullet.SetDamage(GetDamage() * 2);
                     bullet.Fire(direction);
                     bullet.SetOwner(_owner);
                }
            }
            
            // Cleanup the drone (defense spent)
            Destroy(gameObject);
            
            PlayImpactSound(transform.position, "sand_spear_launch");
        }

        private void OnTriggerEnter2D(Collider2D other) {
            // Passive Defense Logic: Intercept projectiles specifically
            // Note: Projectiles should have a clear tag or layer for detection.
            if (!_targetMask.LayerMatchesObject(other) && other.CompareTag("HostileProjectile")) {
                 // Destroy the enemy projectile and lose a fragment or add CD
                 Destroy(other.gameObject);
                 PlayImpactSound(transform.position, "sand_block");
                 
                 // If we ran out of fragments, destroy the aegis early
            }
        }
    }
}
