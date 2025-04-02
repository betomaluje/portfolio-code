using System;
using UnityEngine;

namespace Player {
    public class PlayerMoneyManager : MonoBehaviour {
        public static Action<int> OnMoneyChange = delegate { };

        [SerializeField]
        private int _initialAmount = 10;

        public int CurrentMoney { get; private set; }

        private void Awake() {
            CurrentMoney = _initialAmount;
        }

        public void SetMoney(int amount) {
            CurrentMoney = amount;
            OnMoneyChange?.Invoke(CurrentMoney);
        }

        public bool CanBuy(int cost) => CurrentMoney >= cost;

        public void GiveMoney(int amount) {
            CurrentMoney += amount;
            OnMoneyChange?.Invoke(CurrentMoney);
        }

        public void TakeAmount(int amount) {
            CurrentMoney -= amount;
            CurrentMoney = Mathf.Max(0, CurrentMoney);
            OnMoneyChange?.Invoke(CurrentMoney);
        }
    }
}