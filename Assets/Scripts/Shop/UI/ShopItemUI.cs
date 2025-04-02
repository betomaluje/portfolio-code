using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Shop {
    public class ShopItemUI : MonoBehaviour, ISelectHandler, IDeselectHandler {
        [SerializeField]
        private Image _image;

        [SerializeField]
        private TextMeshProUGUI _name;

        [SerializeField]
        private TextMeshProUGUI _cost;

        [SerializeField]
        private GameObject _alreadyOwned;

        [Header("Status")]
        [SerializeField]
        private Image _buyButton;

        private ShopItemFX _shopItemFX;

        private IShopItem _item;

        public IShopItem Item => _item;

        public void Setup(ShopItemFX shopItemFX) {
            _shopItemFX = shopItemFX;
            _item = shopItemFX.item;
            _image.sprite = _item.Icon;
            _name.text = _item.Name;
            _cost.text = $"${_item.Cost}";

            _alreadyOwned.SetActive(false);
        }

        public void DoEnable() {
            _buyButton.DOColor(_shopItemFX.enabledColor, 0.5f).SetUpdate(true);
        }

        public void DoDisable(bool alreadyOwned) {
            _buyButton.DOColor(_shopItemFX.disabledColor, 0.5f).SetUpdate(true);

            if (alreadyOwned) {
                _alreadyOwned.SetActive(true);
            }
        }

        public void Click_Buy() {
            _shopItemFX.onClick?.Invoke(_item);
        }

        public void OnSelect(BaseEventData eventData) {
            _shopItemFX.onSelecttion?.Invoke(_item, true);
        }

        public void OnDeselect(BaseEventData eventData) {
            _shopItemFX.onSelecttion?.Invoke(_item, false);
        }
    }
}