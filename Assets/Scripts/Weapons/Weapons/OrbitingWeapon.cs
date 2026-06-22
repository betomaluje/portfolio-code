using Base;
using UnityEngine;

namespace Weapons {
    /// <summary>
    /// A weapon that spawns orbiting projectiles to protect the player.
    /// </summary>
    [RequiredBullet(typeof(OrbitBullet))]
    [CreateAssetMenu(fileName = "OrbitingWeapon", menuName = "Aurora/Weapons/Orbiting Weapon")]
    public class OrbitingWeapon : BaseShootingWeapon {
        [Tooltip("Initial number of orbiting bullets spawned when executing attack.")]
        [SerializeField] private int _bulletsPerAttack = 1;

        /// <summary>
        /// Spawns orbit projectiles that will automatically start circling the character.
        /// </summary>
        /// <param name="animations">The character's animations.</param>
        /// <param name="direction">Unused for orbit, but required by interface.</param>
        /// <param name="position">Starting world position of the bullet.</param>
        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            if (HasAmmo()) {
                animations?.Play(AttackAnimation);
                
                for (int i = 0; i < _bulletsPerAttack; i++) {
                    ShootBullet(position, Vector2.zero, animations?.Transform);
                }
                
                StartCooldown();
            } else {
                Reload(animations);
            }
        }

        protected override void OnValidate() {
            base.OnValidate();
            if (BulletPrefab != null && BulletPrefab.GetComponent<OrbitBullet>() == null) {
                Debug.LogWarning($"[{name}] OrbitingWeapon requires a BulletPrefab that has the OrbitBullet component!");
                BulletPrefab = null;
            }
        }
    }
}
