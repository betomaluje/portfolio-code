using DG.Tweening;
using Player;
using TMPro;
using UnityEngine;

namespace UI {
    public class PlayerMoneyUI : MonoBehaviour {
        [SerializeField]
        private TextMeshProUGUI[] _moneyTexts;

        private PlayerMoneyManager _playerMoneyManager;
        private DisablePanelComponent _disablePanelComponent;

        private void Awake() {
            _playerMoneyManager = FindFirstObjectByType<PlayerMoneyManager>();
            if (_playerMoneyManager != null) {
                DisplayMoneyText(_playerMoneyManager.CurrentMoney);
            }

            _disablePanelComponent = new DisablePanelComponent(GetComponent<RectTransform>());
        }

        private void Start() {
            if (_playerMoneyManager == null) {
                // hide all children
                PlayerMoneyManager.OnMoneyChange -= DisplayMoneyText;
                _disablePanelComponent.Hide();
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
            foreach (var moneyText in _moneyTexts) {
                moneyText.DOText($"x{amount}", .3f).SetUpdate(true);

                if (moneyText.transform.localScale != Vector3.one) {
                    var sequence = DOTween.Sequence();
                    sequence.Append(moneyText.transform.DOPunchScale(new Vector3(1.2f, 1.2f, 1.2f), .3f));
                    sequence.Append(moneyText.transform.DOPunchScale(Vector3.one, .3f));
                    sequence.SetUpdate(true).Play();
                }
            }
        }
    }
}