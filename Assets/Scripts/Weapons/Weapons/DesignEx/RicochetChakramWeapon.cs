using Base;
using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// A localized weapon that resets its own cooldown instantly if the player catches the returning projectile.
    /// Features unique return-trip logic via the custom Chakram bullet.
    /// </summary>
    [RequiredBullet(typeof(RicochetChakramBullet))]
    [CreateAssetMenu(fileName = "RicochetChakramWeapon", menuName = "Aurora/Weapons/Expanded/Ricochet Chakram")]
    public class RicochetChakramWeapon : BaseShootingWeapon {

        [Tooltip("How long to wait before the player can shoot again if they MISS a catch.")]
        [SerializeField] private float _missPenaltyCooldown = 3.0f;

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            if (HasAmmo()) {
                animations?.Play(AttackAnimation);
                
                // Spawn the bullet locally to pass our custom Weapon reference
                if (BulletPrefab != null) {
                    var bullet = Instantiate(BulletPrefab, position, Quaternion.identity);
                    if (bullet.TryGetComponent<IBullet>(out var ibullet)) {
                        ibullet.SetWeapon(this);
                        ibullet.SetOwner(animations?.Transform.root);
                        
                        // Pass specific ricochet state
                        if (bullet.TryGetComponent<RicochetChakramBullet>(out var chakram)) {
                            chakram.InitializeChakram(this);
                        }

                        ibullet.Fire(direction.normalized);
                    }
                }

                // If they miss catching it, they suffer the long cooldown.
                _nextFireTime = Time.time + _missPenaltyCooldown; 
            } else {
                Reload(animations);
            }
        }

        /// <summary>
        /// Highly unique mechanic: Called by the projectile when it collides back with its owner.
        /// Rewards the player with an instant cooldown reset.
        /// </summary>
        public void CatchChakram() {
            // Reset the cooldown entirely! Reward mastery.
            _nextFireTime = 0f;
            
            // You can also hook into an event here to play a catch sound or VFX on the player.
        }

    }
}
