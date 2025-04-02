using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace NPCs {
    public class NPCChangeClothes : MonoBehaviour {
        [SerializeField]
        private SpriteLibrary _spriteLibrary;

        [Button]
        public void ChangeSprites(SpriteLibraryAsset spriteLibraryAsset) {
            _spriteLibrary.spriteLibraryAsset = spriteLibraryAsset;
        }
    }
}