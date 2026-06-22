using System;
using Base;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Weapons {
    [CreateAssetMenu(fileName = "SpreadShootWithMeleeWeapon", menuName = "Aurora/Weapons/Spread Shoot & Melee Weapon")]
    public class SpreadShootWithMeleeWeapon : BaseShootingWeapon, IWeaponCollider {
        [SerializeField]
        [Range(0, 360)]
        private int _spreadAngle = 30;

        [SerializeField]
        [Range(0f, 2f)]
        private float _timeBetweenShots = 0.5f;

        [SerializeField]
        private Vector2 _attackSize = Vector2.one;
        [SerializeField]
        private Vector2 _attackOffset = Vector2.zero;

        public Vector2 AttackSize { get => _attackSize; }
        public Vector2 AttackOffset { get => _attackOffset; }

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