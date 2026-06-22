using UnityEngine;

namespace Weapons {
    public class WeaponEquip : MonoBehaviour {
        [SerializeField]
        private Weapon _weapon;

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.TryGetComponent<WeaponManager>(out var weaponManager)) {
                weaponManager.Equip(_weapon);
            }
        }
    }
}