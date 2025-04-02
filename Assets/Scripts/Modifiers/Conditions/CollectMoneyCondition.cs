using Player;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modifiers.Conditions {
    [CreateAssetMenu(menuName = "Aurora/Modifiers/Conditions/Collect Money Condition")]
    [TypeInfoBox("Condition that triggers when the player has collected a certain amount of money.")]
    public class CollectMoneyCondition : BaseCondition {
        [SerializeField]
        [Min(1)]
        private int _coinAmount = 3;

        [Tooltip("If true, we check that the player has gained money. If false, we check if the player has lost money.")]
        [SerializeField]
        private bool _gainedMoney = true;

        private PlayerMoneyManager _playerMoneyManager;

        private bool _moneyHasChanged;
        private int _currentAmount;

        public override void Setup(Transform owner) {
            _currentAmount = 0;
            if (owner.TryGetComponent(out _playerMoneyManager)) {
                PlayerMoneyManager.OnMoneyChange += HandleMoneyChange;
                _currentAmount = _playerMoneyManager.CurrentMoney;
            }

            _moneyHasChanged = false;
        }

        private void HandleMoneyChange(int currentMoney) {
            if (_currentAmount != currentMoney) {
                var difference = currentMoney - _currentAmount;
                if (_gainedMoney) {
                    // check if the player has gained money, aka the difference is positive
                    _moneyHasChanged = difference > 0 && difference >= _coinAmount;
                }
                else {
                    // check if the player has lost money, aka the difference is negative
                    _moneyHasChanged = difference < 0 && difference <= -_coinAmount;
                }
                _currentAmount = currentMoney;
            }
        }

        public override void ResetCondition() {
            base.ResetCondition();
            _moneyHasChanged = false;
        }

        public override bool Check(float deltaTime) => _playerMoneyManager != null && _moneyHasChanged;

        public override void Cleanup() {
            if (_playerMoneyManager != null) {
                PlayerMoneyManager.OnMoneyChange -= HandleMoneyChange;
            }

            _moneyHasChanged = false;
        }

        void OnDestroy() {
            Cleanup();
        }

    }
}