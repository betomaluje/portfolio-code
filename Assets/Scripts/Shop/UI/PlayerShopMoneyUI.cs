using DG.Tweening;
using Player;
using TMPro;
using UnityEngine;

namespace Shop {
    public class PlayerShopMoneyUI : MonoBehaviour {
        [SerializeField]
        private TextMeshProUGUI _moneyText;

        [SerializeField]
        private TextMeshProUGUI _previewMoneyText;

        [SerializeField]
        [Min(0)]
        private float _animDuration = .3f;

        private PlayerMoneyManager _playerMoneyManager;

        private void Awake() {
            _playerMoneyManager = FindFirstObjectByType<PlayerMoneyManager>();
            if (_playerMoneyManager != null) {
                DisplayMoneyText(_playerMoneyManager.CurrentMoney);
            }
        }

        private void OnEnable() {
            PlayerMoneyManager.OnMoneyChange += DisplayMoneyText;
        }

        private void OnDisable() {
            PlayerMoneyManager.OnMoneyChange -= DisplayMoneyText;
        }

        private void OnDestroy() {
            PlayerMoneyManager.OnMoneyChange -= DisplayMoneyText;
        }

        private void DisplayMoneyText(int amount) {
            _moneyText.DOText($"${amount}", _animDuration).SetUpdate(true);
        }

        public void SetPreviewMoney(int amount, Color red, Color green) {
            var remainingAmount = Mathf.Max(0, _playerMoneyManager.CurrentMoney - amount);

            _moneyText.DOColor(red, _animDuration).SetUpdate(true);
            _moneyText.DOText($"<s>${_playerMoneyManager.CurrentMoney}</s>", _animDuration).SetUpdate(true);

            _previewMoneyText.DOColor(green, _animDuration).SetUpdate(true);
            _previewMoneyText.DOText($"-> ${remainingAmount}", _animDuration).SetUpdate(true);
        }

        public void ClearPreview(Color green) {
            _moneyText.DOColor(green, _animDuration).SetUpdate(true);
            _moneyText.DOText($"${_playerMoneyManager.CurrentMoney}", _animDuration).SetUpdate(true);
            _previewMoneyText.DOText("", _animDuration).SetUpdate(true);
        }
    }
}