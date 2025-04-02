using UnityEngine;

namespace Weapons {
    [RequireComponent(typeof(SpriteRenderer))]
    public class WeaponSprite : MonoBehaviour {
        [SerializeField]
        private string _sortingLayer = "Player";

        [SerializeField]
        private int _sortingOrder = 4;

        public void SortSprite() {
            if (TryGetComponent(out SpriteRenderer spriteRenderer) && IsDifferent(spriteRenderer)) {
                spriteRenderer.sortingLayerName = _sortingLayer;
                spriteRenderer.sortingOrder = _sortingOrder;
            }
        }

        private bool IsDifferent(SpriteRenderer spriteRenderer) => spriteRenderer.sortingLayerName != _sortingLayer || spriteRenderer.sortingOrder != _sortingOrder;
    }
}