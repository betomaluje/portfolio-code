using System.Collections.Generic;
using DG.Tweening;
using Extensions;
using Modifiers;
using Modifiers.Merchant;
using Player;
using Player.Input;
using Sirenix.OdinInspector;
using Sounds;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI {
    public class PlayerChooseModifier : MonoBehaviour, IModifierScreen {
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

        [FoldoutGroup("Details UI")]
        [SerializeField]
        private ModifierDetailsUI _modifierDetailsUI;

        [FoldoutGroup("Reroll")]
        [SerializeField]
        private ButtonLongPress _rerollButton;

        [FoldoutGroup("Reroll")]
        [SerializeField]
        private TextMeshProUGUI _rerollText;

        [FoldoutGroup("Reroll")]
        [SerializeField]
        private int _rerollCost = 100;

        [FoldoutGroup("Reroll")]
        [SerializeField]
        private int _maxRerollsAllowed = 3;

        private readonly List<IModifier> _modifiers = new();
        private PlayerUIInput _playerUIInput;
        private PlayerMoneyManager _playerMoneyManager;
        private ModifierMerchant _modifierMerchant;
        private int _rerolls = 1;
        private int _currentCost;

        private void Awake() {
            _playerUIInput = GetComponent<PlayerUIInput>();
            _playerMoneyManager = FindFirstObjectByType<PlayerMoneyManager>();
            _modifierMerchant = GetComponentInParent<ModifierMerchant>();
        }

        private void OnEnable() {
            _playerUIInput.CancelEvent += CancelInGameMenu;

            _currentCost = CalculateRerollCost(_rerollCost, 1.5f, _rerolls);
            _rerollText.text = $"Reroll (${_currentCost})";
        }

        private void OnDisable() {
            _playerUIInput.CancelEvent -= CancelInGameMenu;
        }

        private void Start() {
            _container.SetActive(false);
        }

        private void CancelInGameMenu() {
            _container.SetActive(false);
            Time.timeScale = 1;
        }

        private void UpdateReRollButton() {
            _currentCost = CalculateRerollCost(_rerollCost, 1.5f, _rerolls);
            _rerollText.DOText($"Reroll (${_currentCost})", .2f).SetUpdate(true);
        }

        [Button]
        public void ToggleUI() {
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

        public void AddModifier(IModifier modifier) {
            if (_modifiers.Contains(modifier) || _modifierPrefab == null) {
                return;
            }

            if (_modifiers.Count >= _totalSlots) {
                DebugTools.DebugLog.LogWarning("No more slots available for modifiers.");
                return;
            }

            _modifiers.Add(modifier);

            if (_itemContainer.childCount == _totalSlots) {
                int i = _modifiers.Count - 1;
                Transform child = _itemContainer.GetChild(i);
                if (child != null && !child.gameObject.activeInHierarchy && child.TryGetComponent<ModifierItemUI>(out var uiModifier)) {
                    uiModifier.SetModifier(this, modifier);
                    child.gameObject.SetActive(true);
                }
            }
            else {
                var uiModifier = Instantiate(_modifierPrefab, _itemContainer);
                uiModifier.SetModifier(this, modifier);
            }
        }

        public void ResetModifiers() {
            if (_modifiers != null && _modifiers.Count > 0) {
                _modifiers.Clear();
            }

            if (_itemContainer.childCount > 0) {
                // we need to disable children and then reuse them
                _itemContainer.HideChildren();
            }
        }

        [Button]
        private async void ClearUI() {
            if (_itemContainer.childCount > 0) {
                await _itemContainer.DestroyChildrenAsync();
            }
        }

        public void ShowModifierDetails(IModifier modifier) {
            _modifierDetailsUI.ShowModifierDetails(modifier);
        }

        public void HideModifierDetails() {
            _modifierDetailsUI.HideModifierDetails();
        }

        public void SelectModifier(IModifier modifier) {
            CancelInGameMenu();
            PlayerEquippedModifiers.Instance.EquipModifier(modifier);
            _modifierMerchant.Disappear(modifier);
        }

        // called from UI (Long press button)
        public void Reroll() {
            if (_rerolls >= _maxRerollsAllowed) {
                DebugTools.DebugLog.Log("Max rerolls reached.");
                DisableRerollButton();
                return;
            }

            if (!_playerMoneyManager.CanBuy(_currentCost)) {
                DebugTools.DebugLog.Log("Not enough money to reroll.");
                DisableRerollButton();
                return;
            }

            _rerolls++;
            _playerMoneyManager.TakeAmount(_currentCost);
            SoundManager.instance.Play("coin");
            _modifierMerchant.RefillModifiers();

            if (_rerolls > _maxRerollsAllowed) {
                DisableRerollButton();
            }
            else {
                UpdateReRollButton();
            }
        }

        private void DisableRerollButton() {
            if (_rerollButton.TryGetComponent<Button>(out var button)) {
                button.enabled = false;
            }
            else {
                _rerollButton.enabled = false;
            }
        }

        /// <summary>
        /// The cost grows exponentially to make later rerolls more expensive:
        /// </summary>
        /// <param name="baseCost"></param>
        /// <param name="multiplier"></param>
        /// <param name="rerollCount"></param>
        /// <returns></returns>
        private int CalculateRerollCost(int baseCost, float multiplier, int rerollCount) {
            return Mathf.RoundToInt(baseCost * Mathf.Pow(multiplier, rerollCount));
        }
    }
}