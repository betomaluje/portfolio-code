using Base;
using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// A weapon where primary fire applies a setup marker, 
    /// and a secondary mechanic executes the explosive payoff.
    /// </summary>
    [RequiredBullet(typeof(MarkDartBullet))]
    [CreateAssetMenu(fileName = "MarkDetonatePistolWeapon", menuName = "Aurora/Weapons/Expanded/Mark and Detonate Pistol")]
    public class MarkDetonatePistolWeapon : BaseShootingWeapon {

        /// <summary>
        /// Fires a marking dart.
        /// </summary>
        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            if (HasAmmo()) {
                animations?.Play(AttackAnimation);
                ShootBullet(position, direction, animations?.Transform.root);
                StartCooldown();
            } else {
                Reload(animations);
            }
        }

        /// <summary>
        /// Intended secondary trigger method to manually trigger explosion on all active darts.
        /// Will require WeaponManager integration.
        /// </summary>
        public void DetonateMarks() {
            // Logic to locate all active mark bullet components on screen and trigger their explosion event
            var darts = Object.FindObjectsByType<MarkDartBullet>(FindObjectsSortMode.None);
            foreach (var dart in darts) {
                dart.ExecuteDetonation();
            }
        }

    }
}
