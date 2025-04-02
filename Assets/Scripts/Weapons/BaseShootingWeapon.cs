using Base;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Weapons {
    public abstract class BaseShootingWeapon : Weapon {
        [Tooltip("The maximum ammo this weapon can hold.")]
        [SerializeField]
        protected int MaxAmmo;

        [Tooltip("The animation to play when reloading.")]
        public string ReloadAnimation;

        [Tooltip("<i>Optional</i>. The prefab to spawn when firing.")]
        public GameObject BulletPrefab;

        private int _ammo;

        protected void ShootBullet(Vector3 position, Vector3 direction) {
            if (BulletPrefab != null) {
                var bullet = Instantiate(BulletPrefab, position, Quaternion.identity);
                if (bullet.TryGetComponent<IBullet>(out var ibullet)) {
                    ibullet.SetWeapon(this);
                    ibullet.Fire(direction.normalized);
                }

                _ammo--;
            }
        }

        public void Reload(CharacterAnimations animations) {
            _ammo = MaxAmmo;

            animations?.Play(ReloadAnimation);
        }

        protected bool HasAmmo() => _ammo > 0;

        private void OnValidate() {
            if (_ammo <= 0) {
                _ammo = MaxAmmo;
            }

            AttackType = AttackType.Gun;
        }

        [Button]
        private void RestoreBullets() {
            _ammo = MaxAmmo;
        }
    }
}