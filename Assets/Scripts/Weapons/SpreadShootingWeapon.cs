using System;
using Base;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Weapons {
    [CreateAssetMenu(fileName = "SpreadShootingWeapon", menuName = "Aurora/Weapons/Spread Shooting Weapon")]
    public class SpreadShootingWeapon : BaseShootingWeapon {
        [SerializeField]
        [Range(0, 360)]
        private int _spreadAngle = 30;

        [SerializeField]
        [Range(0f, 2f)]
        private float _timeBetweenShots = 0.5f;

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) {
                return;
            }

            if (HasAmmo()) {
                animations?.Play(AttackAnimation);
                HandleShooting(position);
                StartCooldown();
            }
            else {
                Reload(animations);
            }
        }

        private async void HandleShooting(Vector3 position) {
            var maxAngle = 360;
            var angle = -_spreadAngle;

            while (HasAmmo() && angle < maxAngle) {
                var chunckDirection = Quaternion.Euler(0, 0, angle) * Vector2.right;
                ShootBullet(position, chunckDirection);
                // we draw a gizmo to see the raycast
                Debug.DrawRay(position, chunckDirection * 3, Color.blue);
                angle += _spreadAngle;
                if (_timeBetweenShots > 0) {
                    await UniTask.Delay(TimeSpan.FromSeconds(_timeBetweenShots));
                }
            }
        }
    }
}