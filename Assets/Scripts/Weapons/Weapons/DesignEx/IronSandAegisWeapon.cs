using Base;
using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// A defensive/offensive manipulation tool. 
    /// Spawns a floating iron-sand cloud that protects the player.
    /// Mastery: Using ICharge (Holding Attack) consumes the sand to fire a massive spear. 
    /// This rewards players for choosing the perfect timing to switch from defense to offense.
    /// </summary>
    [RequiredBullet(typeof(IronSandAegis))]
    [CreateAssetMenu(fileName = "IronSandAegisWeapon", menuName = "Aurora/Weapons/Expanded/Iron-Sand Aegis")]
    public class IronSandAegisWeapon : BaseShootingWeapon, ICharge {
        
        [Header("Defense properties")]
        [Tooltip("Number of orbiting sand fragments.")]
        [SerializeField] private int _sandFragments = 3;

        [Tooltip("Time in seconds to reach maximum charge output.")]
        [SerializeField] private float _chargeTime = 2.0f;

        private float _chargeValue;
        private IronSandAegis _activeAegis;

        /// <summary>
        /// Property for managing the "Solidification" process.
        /// </summary>
        public float Charge {
            set {
                _chargeValue = value;
                if (_activeAegis != null) {
                    _activeAegis.SetCinchRatio(_chargeValue);
                }
            }
        }

        public float ChargeTime => _chargeTime;

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            // Initial call: Spawn Aegis if not present
            if (_activeAegis == null) {
                animations?.Play(AttackAnimation);

                if (BulletPrefab != null) {
                    var bullet = Instantiate(BulletPrefab, position, Quaternion.identity);
                    if (bullet.TryGetComponent<IronSandAegis>(out var aegis)) {
                        _activeAegis = aegis;
                        aegis.SetWeapon(this);
                        aegis.SetOwner(animations?.Transform.root);
                        aegis.InitializeDefense(_sandFragments);
                    }
                }
                
                StartCooldown();
            }
            else if (_chargeValue >= 0.9f) {
                // Secondary call: "Attack" fires the solidified spear if fully charged
                animations?.Play(AttackAnimation); // Use a distinct "Spear" string if preferred
                _activeAegis.FireSolidifiedSpear(direction.normalized);
                _activeAegis = null; // Consume the defense
                StartCooldown();
            }
        }

    }
}
