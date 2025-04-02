using UnityEngine;

namespace Weapons {
    public interface IBullet {
        public void SetWeapon(Weapon weapon);
        void Fire(Vector2 direction);
    }
}