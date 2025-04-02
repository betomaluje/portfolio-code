using System.Text;
using Companions;
using UnityEngine;

namespace Shop {
    [CreateAssetMenu(menuName = "Aurora/Shop/CompanionItem")]
    public class CompanionShopItem : ShopItem<CompanionStats> {
        public override string Stats() {
            StringBuilder sb = new();

            sb.AppendLine($"Max Health: {Item.MaxHealth}");
            return sb.ToString();
        }
    }
}