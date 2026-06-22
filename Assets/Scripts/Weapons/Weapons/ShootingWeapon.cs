using Base;
using UnityEngine;

namespace Weapons {
    [CreateAssetMenu(fileName = "ShootingWeapon", menuName = "Aurora/Weapons/Shooting Weapon")]
    public class ShootingWeapon : BaseShootingWeapon {
        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) {
                return;
            }

            if (HasAmmo()) {
                animations?.Play(AttackAnimation);
                ShootBullet(position, direction);
                StartCooldown();
            }
            else {
                Reload(animations);
            }
        }
    }
}