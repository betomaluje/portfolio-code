using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using Weapons;

namespace Shop {
    public class WeaponPotionsShop : ShopUI {

        private const float MAX_ALPHA = 1f;
        private const float MIN_ALPHA = .3f;

        private const float ALPHA_DURATION = .5f;

        [Header("Tabs")]
        [SerializeField]
        private CanvasGroup _weaponTab;

        [SerializeField]
        private CanvasGroup _consumableTab;

        private WeaponManager _playerWeaponManager;

        private WeaponShopItem[] _weaponItems;
        private ConsumablesShopItem[] _consumablesItems;

        private ShopType _shopType = ShopType.Weapon;

        protected override void Awake() {
            base.Awake();
            _playerWeaponManager = FindFirstObjectByType<WeaponManager>();

            _weaponItems = Resources.LoadAll<WeaponShopItem>("Shop/Weapons");
            _consumablesItems = Resources.LoadAll<ConsumablesShopItem>("Shop/Consumables");

            // we need to sort the _weaponItems by their WeaponType
            if (_weaponItems != null) {
                var currentOrder = _weaponItems.OrderBy(weapon => weapon.Item.AttackType).ThenBy(weapon => weapon.Cost);
                if (!currentOrder.SequenceEqual(_weaponItems)) {
                    _weaponItems = currentOrder.ToArray();
                }
            }

            // we need to sort the _consumableItems by their price
            if (_consumablesItems != null) {
                var currentOrder = _consumablesItems.OrderBy(weapon => weapon.Cost);
                if (!currentOrder.SequenceEqual(_consumablesItems)) {
                    _consumablesItems = currentOrder.ToArray();
                }
            }
        }

        protected override void OnEnable() {
            base.OnEnable();
            _playerInput.NextViewEvent += ToggleView;
            _playerInput.PreviousViewEvent += ToggleView;
        }

        protected override void OnDisable() {
            base.OnDisable();
            _playerInput.NextViewEvent -= ToggleView;
            _playerInput.PreviousViewEvent -= ToggleView;
        }

        override protected void Start() {
            base.Start();
            FillWeaponItems();
            FillConsumableItems();

            _weaponTab.DOFade(MAX_ALPHA, 0f).SetUpdate(true);
            _consumableTab.DOFade(MIN_ALPHA, 0f).SetUpdate(true);
        }

        private void ToggleView() {
            if (_shopType == ShopType.Weapon) {
                _shopType = ShopType.Consumable;
                UpdateAvailableItems();
            }
            else {
                _shopType = ShopType.Weapon;
                UpdateAvailableItems();
            }
        }

        private void FillWeaponItems() {
            var list = new List<ShopItemUI>(_weaponItems.Length);
            foreach (var weaponItem in _weaponItems) {
                var shopItem = Instantiate(_prefab, _gridContainer);
                var shopItemFX = new ShopItemFX {
                    onClick = Click_BuyItem,
                    onSelecttion = Hover_ItemSelection,
                    item = weaponItem,
                    enabledColor = _enabledColor,
                    disabledColor = _disabledColor
                };
                shopItem.Setup(shopItemFX);
                shopItem.gameObject.SetActive(false);
                list.Add(shopItem);
            }

            _shopItemsByType[ShopType.Weapon] = list;
        }

        private void FillConsumableItems() {
            var list = new List<ShopItemUI>(_consumablesItems.Length);
            foreach (var consumableItem in _consumablesItems) {
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

            _shopItemsByType[ShopType.Consumable] = list;
        }

        private void PopulateWeapons(List<ShopItemUI> items) {
            foreach (var item in items) {
                if (item.Item is WeaponShopItem weapon) {
                    var alreadyOwned = _playerWeaponManager.HasWeapon(weapon.Item);

                    if (alreadyOwned) {
                        item.DoDisable(true);
                    }
                    else {
                        var canBuy = _playerMoneyManager.CanBuy(weapon.Cost);
                        if (canBuy) {
                            item.DoEnable();
                        }
                        else {
                            item.DoDisable(false);
                        }
                    }
                }
                else {
                    item.DoDisable(false);
                }
                item.gameObject.SetActive(true);
            }
        }

        private void PopulateConsumables(List<ShopItemUI> items) {
            foreach (var item in items) {
                if (item.Item is ConsumablesShopItem consumable && _playerMoneyManager.CanBuy(consumable.Cost)) {
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
            _gridContainer.HideChildren();
            if (_shopType == ShopType.Weapon) {
                var list = _shopItemsByType[ShopType.Weapon];

                _weaponTab.DOFade(MAX_ALPHA, ALPHA_DURATION).SetUpdate(true);
                _consumableTab.DOFade(MIN_ALPHA, ALPHA_DURATION).SetUpdate(true);

                PopulateWeapons(list);
            }
            else if (_shopType == ShopType.Consumable) {
                var list = _shopItemsByType[ShopType.Consumable];

                _weaponTab.DOFade(MIN_ALPHA, ALPHA_DURATION).SetUpdate(true);
                _consumableTab.DOFade(MAX_ALPHA, ALPHA_DURATION).SetUpdate(true);

                PopulateConsumables(list);
            }

            // now we select the first item
            foreach (Transform child in _gridContainer) {
                if (child != null && child.gameObject.activeInHierarchy) {
                    EventSystem.current.SetSelectedGameObject(child.gameObject);
                    break;
                }
            }
        }

        private void Click_BuyItem(IShopItem item) {
            if (item is WeaponShopItem weapon) {
                Click_BuyWeapon(weapon);
            }
            else if (item is ConsumablesShopItem consumable) {
                Click_BuyConsumable(consumable);
            }
        }

        private void Hover_ItemSelection(IShopItem item, bool isSelected) {
            if (item is WeaponShopItem weapon) {
                Hover_WeaponSelection(weapon, isSelected);
            }
            else if (item is ConsumablesShopItem consumable) {
                Hover_ConsumableSelection(consumable, isSelected);
            }
        }

        /// <summary>
        /// Check if the player can buy the weapon. If the player doesn't own the weapon and can afford it
        /// </summary>
        /// <param name="item"></param>
        /// <returns>True if it's available, false otherwise</returns>
        private bool CheckAvialableWeapon(WeaponShopItem item) {
            return !_playerWeaponManager.HasWeapon(item.Item) && _playerMoneyManager.CanBuy(item.Cost);
        }

        #region Weapons
        private void Click_BuyWeapon(WeaponShopItem item) {
            if (CheckAvialableWeapon(item)) {
                _playerWeaponManager.Equip(item.Item);
                _playerMoneyManager.TakeAmount(item.Cost);
                UpdateAvailableItems();
            }
        }

        private void Hover_WeaponSelection(WeaponShopItem item, bool isSelected) {
            if (isSelected) {
                _itemDetails.SetItemDescription(item);
            }

            if (CheckAvialableWeapon(item)) {
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
        #endregion

        #region Consumables
        private void Click_BuyConsumable(ConsumablesShopItem item) {
            if (_playerMoneyManager.CanBuy(item.Cost)) {
                _playerMoneyManager.TakeAmount(item.Cost);

                // since the item is already a ConsumableSO -> IConsumable
                if (item.Item) {
                    item.Item.Consume(_playerMoneyManager.gameObject.transform);
                }

                UpdateAvailableItems();
            }
        }

        private void Hover_ConsumableSelection(ConsumablesShopItem item, bool isSelected) {
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
        #endregion
    }
}