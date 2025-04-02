using UnityEngine;
using UnityEngine.U2D.Animation;

namespace NPCs {
    [RequireComponent(typeof(NPCChangeClothes), typeof(NPCStateMachine))]
    public class NPCVillageSetup : MonoBehaviour {
        [SerializeField]
        private SpriteLibraryAsset[] SpriteLibraryAsset;

        [SerializeField]
        private NPCCharacteristics[] _characteristics;

        private void Awake() {
            GetComponent<NPCChangeClothes>().ChangeSprites(SpriteLibraryAsset[Random.Range(0, SpriteLibraryAsset.Length)]);
            GetComponent<NPCStateMachine>().Characteristics = _characteristics[Random.Range(0, _characteristics.Length)];
        }

    }
}