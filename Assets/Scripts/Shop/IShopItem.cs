using UnityEngine;

namespace Shop {
    public interface IShopItem {
        public string Name { get; }
        public int Cost { get; }
        public Sprite Icon { get; }
        public string Stats();
    }
}