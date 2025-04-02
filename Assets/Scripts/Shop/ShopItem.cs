using Companions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Shop {
    public abstract class ShopItem<T> : ScriptableObject, IShopItem where T : ScriptableObject {
        [Tooltip("The name of the item")]
        public string Name;

        [Tooltip("The type of the item")]
        public ShopItemCategory ItemType;

        [Tooltip("The cost of the item")]
        [Min(1)]
        public int Cost;

        [Tooltip("The ScriptableObject that represents the item")]
        public T Item;

        [Tooltip("The Icon for the item")]
        [PreviewField]
        public Sprite Icon;

        Sprite IShopItem.Icon { get => Icon; }

        string IShopItem.Name => Name;

        int IShopItem.Cost => Cost;

        public abstract string Stats();

        private void OnValidate() {
            if (Item != null) {
                if (string.IsNullOrEmpty(Name)) {
                    Name = Item.name;
                }

                // TODO: add other types of items
                if (Item is CompanionStats) {
                    ItemType = ShopItemCategory.Companion;
                }
                else if (Item is Weapons.Weapon) {
                    ItemType = ShopItemCategory.Weapon;
                }
                else {
                    ItemType = ShopItemCategory.Consumable;
                }
            }
        }
    }

    [System.Serializable]
    public enum ShopItemCategory {
        Weapon,
        Armor,
        Consumable,
        Companion
    }
}