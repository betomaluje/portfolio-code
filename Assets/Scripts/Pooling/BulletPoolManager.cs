using UnityEngine;
using Weapons;

namespace Pooling.Weapons {
    public class BulletPoolManager : PoolerBase<PoolBullet> {
        public BulletPoolManager(PoolBullet bulletPrefab, int poolSize = 10) {
            InitPool(bulletPrefab, 10);
        }

        public PoolBullet GetAndSetup(Weapon weapon, Vector3 position, GameObject parent) {
            var bullet = Get();
            bullet.SetWeapon(weapon);
            bullet.transform.position = position;
            bullet.transform.parent = parent.transform;

            return bullet;
        }
    }
}