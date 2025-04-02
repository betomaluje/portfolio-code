using System.Collections.Generic;
using System.Linq;
using Companions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Shop {
    public class CompanionsShop : ShopUI {
        private CompanionShopItem[] _companionsItems;

        private CompanionsHolder _companionsHolder;

        protected override void Awake() {
            base.Awake();

            _companionsHolder = FindFirstObjectByType<CompanionsHolder>();

            _companionsItems = Resources.LoadAll<CompanionShopItem>("Shop/Companions");

            // we need to sort the _weaponItems by their WeaponType
            if (_companionsItems != null) {
                var currentOrder = _companionsItems.OrderBy(companion => companion.Item.Name).ThenBy(companion => companion.Cost);
                if (!currentOrder.SequenceEqual(_companionsItems)) {
                    _companionsItems = currentOrder.ToArray();
                }
            }
        }

        protected override void Start() {
            base.Start();
            FillCompanions();
        }

        private void FillCompanions() {
            var list = new List<ShopItemUI>(_companionsItems.Length);
            foreach (var consumableItem in _companionsItems) {
                var shopItem = Instantiate(_prefab, _gridContainer);
                var shopItemFX = new ShopItemFX {
                    onClick = Click_BuyItem,
                    onSelecttion = Hover_ItemSelection,
                    item = consumableItem,
                    enabledColor = _enabledColor,
                    disabledColor = _disabledColor
                };
                shopItem.Setup(shopItemFX);
                shopItem.gameObject.SetActive(false);
                list.Add(shopItem);
            }

            _shopItemsByType[ShopType.Companion] = list;
        }

        private void PopulateCompanions(List<ShopItemUI> items) {
            foreach (var item in items) {
                if (item.Item is CompanionShopItem companion && _playerMoneyManager.CanBuy(companion.Cost)) {
                    item.DoEnable();
                }
                else {
                    // by default all consumables can't be owned
                    item.DoDisable(false);
                }
                item.gameObject.SetActive(true);
            }
        }

        protected override void UpdateAvailableItems() {
            // populate companions
            PopulateCompanions(_shopItemsByType[ShopType.Companion]);

            // now we select the first item
            foreach (Transform child in _gridContainer) {
                if (child != null && child.gameObject.activeInHierarchy) {
                    EventSystem.current.SetSelectedGameObject(child.gameObject);
                    break;
                }
            }
        }

        private void Click_BuyItem(IShopItem item) {
            if (item is CompanionShopItem companion) {
                Click_BuyCompanion(companion);
            }
        }

        private void Hover_ItemSelection(IShopItem item, bool isSelected) {
            if (item is CompanionShopItem companion) {
                Hover_CompanionSelection(companion, isSelected);
            }
        }

        private void Click_BuyCompanion(CompanionShopItem item) {
            if (_playerMoneyManager.CanBuy(item.Cost)) {
                _playerMoneyManager.TakeAmount(item.Cost);

                _companionsHolder.EquipCompanion(item.Name);

                UpdateAvailableItems();
            }
        }

        private void Hover_CompanionSelection(CompanionShopItem item, bool isSelected) {
            if (isSelected) {
                _itemDetails.SetItemDescription(item);
            }

            if (_playerMoneyManager.CanBuy(item.Cost)) {
                if (isSelected) {
                    _playerShopMoneyUI?.SetPreviewMoney(item.Cost, _disabledColor, _enabledColor);
                }
                else {
                    _playerShopMoneyUI?.ClearPreview(_enabledColor);
                }
            }
            else {
                _playerShopMoneyUI?.ClearPreview(_enabledColor);
            }
        }
    }
}