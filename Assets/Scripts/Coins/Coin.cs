using BerserkPixel.Utils;
using Player;
using UI;
using UnityEngine;

namespace Coins {
    public class Coin : MonoBehaviour {
        [SerializeField]
        private LayerMask _playerMask;

        [SerializeField]
        private int _amountPerCoin = 5;

        private void OnTriggerEnter2D(Collider2D other) {
            if (_playerMask.LayerMatchesObject(other)) {
                if (other.TryGetComponent(out PlayerMoneyManager moneyManager)) {
                    moneyManager.GiveMoney(_amountPerCoin);
                    MoneyAnimationsUI.Instance.ShowCoins(_amountPerCoin, transform);

                }
                Destroy(gameObject);
            }
        }
    }
}