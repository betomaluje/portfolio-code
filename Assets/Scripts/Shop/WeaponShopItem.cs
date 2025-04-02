using System.Text;
using UnityEngine;

namespace Shop {
    [CreateAssetMenu(fileName = "WeaponItem", menuName = "Aurora/Shop/WeaponItem")]
    public class WeaponShopItem : ShopItem<Weapons.Weapon> {
        public override string Stats() {
            StringBuilder sb = new();

            sb.AppendLine($"Damage: {Item.GetDamage()}");
            sb.AppendLine($"Range: {Item.Range}");
            sb.AppendLine($"Knockback force: {Item.KnockbackForce}");
            sb.AppendLine($"AttackCooldown: {Item.AttackCooldown}");
            sb.AppendLine($"AttackType: {Item.AttackType}");

            return sb.ToString();
        }
    }
}