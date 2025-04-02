using UnityEngine;
using Sirenix.OdinInspector;

namespace Shop.Keeper {
    public class ShopKeeperRequirements : MonoBehaviour {
        [SerializeField]
        [Required]
        private ShopUI _shopUI;

        [SerializeField]
        private string _ownersName;

        private void Start() {
            var shop = Instantiate(_shopUI, Vector3.zero, Quaternion.identity);
            shop.SetOwner(GetComponent<ShopKeeperStateMachine>(), _ownersName);
        }
    }
}