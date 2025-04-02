using Dungeon;
using UnityEngine;

namespace Modifiers.Merchant {
    [RequireComponent(typeof(ModifierMerchant))]
    public class AutoAddModifiers : MonoBehaviour {
        [SerializeField]
        private FirstRoomSetup _firstRoomSetup;

        private ModifierMerchant _modifierMerchant;

        private void Start() {
            _modifierMerchant = GetComponent<ModifierMerchant>();
            _modifierMerchant.SetupFirstRoom(_firstRoomSetup);
        }
    }
}