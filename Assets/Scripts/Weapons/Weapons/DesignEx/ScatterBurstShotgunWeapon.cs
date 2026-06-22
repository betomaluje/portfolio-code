using Base;
using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// A shotgun-style weapon that fires multiple projectiles in a cone spread simultaneously.
    /// </summary>
    [CreateAssetMenu(fileName = "ScatterBurstShotgun", menuName = "Aurora/Weapons/Expanded/Scatter Burst Shotgun")]
    public class ScatterBurstShotgunWeapon : BaseShootingWeapon {

        [Header("Shotgun Spread")]
        [Tooltip("Number of pellets fired per shot.")]
        [SerializeField] [Min(1)] private int _pelletCount = 5;

        [Tooltip("The total arc angle of the spread in degrees.")]
        [SerializeField] [Min(5f)] private float _spreadArcAngle = 45f;

        /// <summary>
        /// Calculates the spread and instantiates multiple bullets at once.
        /// </summary>
        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            if (HasAmmo()) {
                animations?.Play(AttackAnimation);

                float startAngle = -(_spreadArcAngle / 2f);
                float angleStep = _pelletCount > 1 ? _spreadArcAngle / (_pelletCount - 1) : 0f;

                float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                for (int i = 0; i < _pelletCount; i++) {
                    float currentOffset = startAngle + (angleStep * i);
                    
                    // Convert angle back to direction vector
                    Quaternion rotation = Quaternion.AngleAxis(currentOffset, Vector3.forward);
                    Vector2 projectileDir = rotation * direction;

                    ShootBullet(position, projectileDir, animations?.Transform.root);
                }

                StartCooldown();
            } else {
                Reload(animations);
            }
        }
    }
}
