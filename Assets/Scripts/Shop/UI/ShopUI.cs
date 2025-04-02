using System.Collections.Generic;
using Extensions;
using Player;
using Player.Input;
using Shop.Keeper;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Sirenix.OdinInspector;

namespace Shop {
    [System.Serializable]
    public enum ShopType {
        Weapon,
        Consumable,
        Companion
    }

    public abstract class ShopUI : MonoBehaviour {
        [SerializeField]
        protected PlayerUIInput _playerInput;

        [SerializeField]
        private RectTransform _container;

        [SerializeField]
        private TextMeshProUGUI _titleText;

        [SerializeField]
        protected RectTransform _gridContainer;

        [SerializeField]
        protected ShopItemUI _prefab;

        [SerializeField]
        private bool _persistChangesWhenClosed = true;

        [BoxGroup("UI Effects")]
        [SerializeField]
        protected PlayerShopMoneyUI _playerShopMoneyUI;

        [BoxGroup("UI Effects")]
        [SerializeField]
        protected Color _enabledColor;

        [BoxGroup("UI Effects")]
        [SerializeField]
        protected Color _disabledColor;

        [Header("Item Details")]
        [SerializeField]
        protected ShopItemDetails _itemDetails;

        private PlayerBattleInput _input;
        protected PlayerMoneyManager _playerMoneyManager;

        private PlayerPersistence _playerPersistence;

        private ShopKeeperStateMachine _shopKeeper;

        protected readonly Dictionary<ShopType, List<ShopItemUI>> _shopItemsByType = new();

        public void SetOwner(ShopKeeperStateMachine shopKeeper, string ownersName) {
            _shopKeeper = shopKeeper;

            _shopKeeper.OnInteract += ShowUI;
            _shopKeeper.OnCancelInteract += HideUI;

            if (_titleText != null && !string.IsNullOrEmpty(ownersName)) {
                _titleText.text = $"{ownersName}'s Shop";
            }
        }

        protected virtual void Awake() {
            _input = FindFirstObjectByType<PlayerBattleInput>();
            _playerMoneyManager = FindFirstObjectByType<PlayerMoneyManager>();
            _playerPersistence = FindFirstObjectByType<PlayerPersistence>();
        }

        protected virtual async void Start() {
            _container.gameObject.SetActive(false);
            GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

            await _gridContainer.DestroyChildrenAsync();
        }

        protected virtual void OnEnable() {
            _playerInput.CancelEvent += HideUI;
        }

        protected virtual void OnDisable() {
            _playerInput.CancelEvent -= HideUI;
        }

        private void OnDestroy() {
            if (_shopKeeper != null) {
                _shopKeeper.OnInteract -= ShowUI;
                _shopKeeper.OnCancelInteract -= HideUI;
            }
        }

        protected abstract void UpdateAvailableItems();

        private void ShowUI() {
            if (!_container.gameObject.activeInHierarchy) {
                _container.gameObject.SetActive(true);
                if (_input != null) {
                    _input.BattleActions.Disable();
                }

                UpdateAvailableItems();

                if (_gridContainer.childCount > 0) {
                    // we select the first item
                    var firstSelected = _gridContainer.GetChild(0).gameObject;
                    EventSystem.current.firstSelectedGameObject = firstSelected;
                    EventSystem.current.SetSelectedGameObject(firstSelected);
                }
            }
        }

        private void HideUI() {
            if (_container.gameObject.activeInHierarchy) {
                _container.gameObject.SetActive(false);

                // we save the player
                if (_persistChangesWhenClosed) {
                    _playerPersistence.SaveBoughtItems();
                }

                if (_input != null) {
                    _input.BattleActions.Enable();
                }
            }
        }
    }
}