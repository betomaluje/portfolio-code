using System.Collections.Generic;
using Extensions;
using Modifiers;
using Player.Input;
using Sirenix.OdinInspector;
using Sounds;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace UI {
    public class PlayerEquippedModifiers : Singleton<PlayerEquippedModifiers>, IModifierScreen {
        [FoldoutGroup("Container UI")]
        [SerializeField]
        private GameObject _container;

        [FoldoutGroup("Container UI")]
        [SerializeField]
        private RectTransform _itemContainer;

        [FoldoutGroup("Container UI")]
        [SerializeField]
        private int _totalSlots = 12;

        [FoldoutGroup("Container UI")]
        [SerializeField]
        private ModifierItemUI _modifierPrefab;

        [FoldoutGroup("Fake UI")]
        [SerializeField]
        private RectTransform _fakeItemContainer;

        [FoldoutGroup("Fake UI")]
        [SerializeField]
        private GameObject _modifierEmptyPrefab;

        [FoldoutGroup("Details UI")]
        [SerializeField]
        private ModifierDetailsUI _modifierDetailsUI;

        private readonly List<IModifier> _modifiers = new();

        private PlayerBattleInput _playerInput;
        private PlayerUIInput _playerUIInput;
        private DisablePanelComponent _disablePanelComponent;

        protected override void Awake() {
            base.Awake();
            _disablePanelComponent = new DisablePanelComponent(GetComponent<RectTransform>());
            _playerUIInput = GetComponent<PlayerUIInput>();
            _playerInput = FindFirstObjectByType<PlayerBattleInput>();
        }

        private void Start() {
            if (_playerUIInput == null || _playerInput == null) {
                _disablePanelComponent.Hide();
                return;
            }

            _container.SetActive(false);
            for (int i = 0; i < _totalSlots; i++) {
                var empty = Instantiate(_modifierEmptyPrefab, _fakeItemContainer);
                empty.name = $"Empty Slot {i}";
            }
        }

        private void OnEnable() {
            if (_playerUIInput == null || _playerInput == null) {
                return;
            }

            _playerInput.NorthButtonEvent += ToggleUI;
            _playerUIInput.CancelEvent += CancelInGameMenu;
        }

        private void OnDisable() {
            if (_playerUIInput == null || _playerInput == null) {
                return;
            }

            _playerInput.NorthButtonEvent -= ToggleUI;
            _playerUIInput.CancelEvent -= CancelInGameMenu;
        }

        private void CancelInGameMenu() {
            _container.SetActive(false);
            Time.timeScale = 1;
        }

        [Button]
        private void ToggleUI() {
            var alreadyVisible = _container.activeInHierarchy;
            _container.SetActive(!alreadyVisible);

            if (_container.activeInHierarchy) {
                Time.timeScale = 0;
                SoundManager.instance.Play("menu_open");

                if (_itemContainer.childCount > 0) {
                    var firstItem = _itemContainer.GetChild(0).gameObject;
                    EventSystem.current.firstSelectedGameObject = firstItem;
                    EventSystem.current.SetSelectedGameObject(firstItem);
                }
            }
            else {
                CancelInGameMenu();
            }
        }

        [Button]
        private async void ClearUI() {
            if (_itemContainer.childCount > 0) {
                await _itemContainer.DestroyChildrenAsync();
            }
        }

        public void EquipModifier(IModifier modifier) {
            if (_modifiers.Contains(modifier) || _modifierPrefab == null) {
                return;
            }

            if (_modifiers.Count >= _totalSlots) {
                DebugTools.DebugLog.LogWarning("No more slots available for modifiers.");
                return;
            }

            _modifiers.Add(modifier);

            var uiModifier = Instantiate(_modifierPrefab, _itemContainer);
            uiModifier.SetModifier(this, modifier);
        }

        public void EquipModifier(IModifier[] modifiers) {
            foreach (var modifier in modifiers) {
                EquipModifier(modifier);
            }
        }

        public void ShowModifierDetails(IModifier modifier) {
            _modifierDetailsUI.ShowModifierDetails(modifier);
        }

        public void HideModifierDetails() {
            _modifierDetailsUI.HideModifierDetails();
        }

        public void SelectModifier(IModifier modifier) { }

#if UNITY_EDITOR
        [Button]
        private void DebugUI(int amount) {
            ClearUI();

            for (int i = 0; i < amount; i++) {
                if (_modifierPrefab != null) {
                    var uiModifier = Instantiate(_modifierEmptyPrefab, _itemContainer);
                }
            }
        }
#endif
    }
}