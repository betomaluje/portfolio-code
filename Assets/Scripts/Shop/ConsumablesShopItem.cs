using Consumables;
using UnityEngine;

namespace Shop {
    [CreateAssetMenu(fileName = "ConsumableItem", menuName = "Aurora/Shop/ConsumableItem")]
    public class ConsumablesShopItem : ShopItem<ConsumableSO> {
        [SerializeField]
        [TextArea(1, 3)]
        private string _stats;

        public override string Stats() {
            return _stats;
        }
    }
}