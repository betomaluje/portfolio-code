using Base;
using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// Configuration for an Orbital Beacon Bit weapon.
    /// Spawns a drone that orbits the character.
    /// Mastery: Using ICharge (Holding Attack) cinches the bit closer to the player.
    /// </summary>
    [RequiredBullet(typeof(SolarFlareBit))]
    [CreateAssetMenu(fileName = "SolarFlareBitWeapon", menuName = "Aurora/Weapons/Expanded/Solar Flare Bit")]
    public class SolarFlareBitWeapon : BaseShootingWeapon, ICharge {
        
        [Header("Orbital Properties")]
        [Tooltip("Minimum radius (orbit when fully charged).")]
        [SerializeField] private float _minOrbitRadius = 1.0f;
        
        [Tooltip("Maximum radius (orbit when idling).")]
        [SerializeField] private float _maxOrbitRadius = 6.0f;

        [Tooltip("Time in seconds to reach maximum charge output.")]
        [SerializeField] private float _chargeTime = 2.0f;

        private float _chargeValue;
        private SolarFlareBit _activeBit;

        /// <summary>
        /// Property for managing the cinch distance via external input hold.
        /// </summary>
        public float Charge {
            set {
                _chargeValue = Mathf.Clamp01(value);
                // Propagate the charge to the active bit for radius adjustment
                if (_activeBit != null) {
                    _activeBit.SetCinchRatio(_chargeValue);
                }
            }
        }

        public float ChargeTime => _chargeTime;

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            // Firing this weapon spawns the bit if NOT active.
            if (_activeBit == null) {
                animations?.Play(AttackAnimation);
                
                if (BulletPrefab != null) {
                    var bullet = Instantiate(BulletPrefab, position, Quaternion.identity);
                    if (bullet.TryGetComponent<SolarFlareBit>(out var bit)) {
                        _activeBit = bit;
                        bit.SetWeapon(this);
                        bit.SetOwner(animations?.Transform.root);
                        bit.InitializeOrbit(_minOrbitRadius, _maxOrbitRadius);
                    }
                }
                StartCooldown();
            }
            else {
                // If it's already active, it just keeps moving/cinching.
                // We'll trust the WeaponManager to pass Charge value to us.
            }
        }

    }
}
