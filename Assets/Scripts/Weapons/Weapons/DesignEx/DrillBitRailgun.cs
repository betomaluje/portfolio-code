using Base;
using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// Configuration for a linear pierce weapon. 
    /// The bullet speeds up and deals more damage for every enemy it penetrates.
    /// </summary>
    [RequiredBullet(typeof(DrillBitBullet))]
    [CreateAssetMenu(fileName = "DrillBitRailgun", menuName = "Aurora/Weapons/Expanded/Drill-Bit Railgun")]
    public class DrillBitRailgun : BaseShootingWeapon {

        [Header("Railgun Drill Properties")]
        [Tooltip("How much speed increases after every pierce.")]
        [Range(1.0f, 2.0f)] [SerializeField] private float _speedMultiplierPerPierce = 1.25f;

        [Tooltip("Cooldown period between Railgun shots.")]
        [SerializeField] private float _railgunCooldown = 1.0f;

        /// <summary>
        /// Fires the Drill-Bit Railgun slug.
        /// </summary>
        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            if (HasAmmo()) {
                animations?.Play(AttackAnimation);
                
                // Spawn the bullet locally to pass custom logic
                if (BulletPrefab != null) {
                    var bullet = Instantiate(BulletPrefab, position, Quaternion.identity);
                    if (bullet.TryGetComponent<DrillBitBullet>(out var drill)) {
                        drill.SetWeapon(this);
                        drill.SetOwner(animations?.Transform.root);
                        drill.InitializeDrill(_speedMultiplierPerPierce);
                        drill.Fire(direction.normalized);
                    }
                    else if (bullet.TryGetComponent<IBullet>(out var ibullet)) {
                        ibullet.SetWeapon(this);
                        ibullet.SetOwner(animations?.Transform.root);
                        ibullet.Fire(direction.normalized);
                    }
                }
                
                // Set the custom cooldown directly 
                _nextFireTime = Time.time + _railgunCooldown;
            } else {
                Reload(animations);
            }
        }

    }
}
