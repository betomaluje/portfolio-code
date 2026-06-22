using UnityEngine;

namespace Weapons {
    public interface IBullet {
        void SetWeapon(Weapon weapon);
        void SetDamage(int damage);
        int GetDamage();
        void SetOwner(Transform owner);
        void SetSpeed(float speed);
        float GetSpeed();
        void Fire(Vector2 direction);
    }
}