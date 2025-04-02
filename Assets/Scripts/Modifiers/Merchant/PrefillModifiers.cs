using Dungeon;
using UnityEngine;

namespace Modifiers.Merchant {
    [RequireComponent(typeof(ModifierMerchant))]
    public class PrefillModifiers : MonoBehaviour {
        [SerializeField]
        private FirstRoomSetup _firstRoomSetup;

        private ModifierMerchant _modifierMerchant;

        private void Awake() {
            _modifierMerchant = GetComponent<ModifierMerchant>();
        }

        private void Start() {
            _modifierMerchant.SetupFirstRoom(_firstRoomSetup);
        }
    }

}