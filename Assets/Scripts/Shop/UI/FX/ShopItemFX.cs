using System;
using UnityEngine;

namespace Shop {
    [Serializable]
    public class ShopItemFX {
        public Action<IShopItem> onClick;
        public Action<IShopItem, bool> onSelecttion;
        public IShopItem item;
        public Color enabledColor;
        public Color disabledColor;
    }
}