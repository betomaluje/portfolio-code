using System;
using Base;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Weapons {
    [CreateAssetMenu(fileName = "MultipleSpreadShootingWeapon", menuName = "Aurora/Weapons/Multiple Spread Shooting Weapon")]
    public class MultipleSpreadShootingWeapon : BaseShootingWeapon {
        [SerializeField]
        [Range(0, 360)]
        private int _spreadAngle = 30;

        [SerializeField]
        [Range(0f, 2f)]
        private float _timeBetweenShots = 0.5f;

        [Header("Extra rotations")]
        [SerializeField]
        [Range(0, 360)]
        private float _rotationBetweenShots = 45f;

        [SerializeField]
        [Min(1)]
        private int _extraRotations = 1;

        [SerializeField]
        [Range(0f, 2f)]
        private float _timeBetweenRotations = 0.5f;

        protected override void StartCooldown() =>
            _nextFireTime = Time.time + AttackCooldown + _extraRotations * _timeBetweenRotations;

        public async override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) {
                return;
            }

            if (HasAmmo()) {
                animations?.Play(AttackAnimation);

                for (int i = 0; i < _extraRotations; i++) {
                    await HandleShooting(position, _rotationBetweenShots * i);

                    if (_timeBetweenRotations > 0) {
                        await UniTask.Delay(TimeSpan.FromSeconds(_timeBetweenRotations));
                    }
                }

                Reload(animations);
                StartCooldown();
            }
            else {
                Reload(animations);
            }
        }

        private async UniTask HandleShooting(Vector3 position, float extraAngle = 0) {
            var maxAngle = 360 + extraAngle;
            float angle = extraAngle;

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