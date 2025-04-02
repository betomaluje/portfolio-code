using UnityEngine;
using UnityEngine.VFX;

namespace Dungeon.FogOfWar {
    public class VFXSortOrder : MonoBehaviour {
        [SerializeField]
        private VisualEffect _visualEffect;

        [SerializeField]
        private int _sortingOrder = 100;

        [SerializeField]
        private string _layer = "Foreground";

        private void OnValidate() {
            if (TryGetComponent(out _visualEffect) && _visualEffect.TryGetComponent(out VFXRenderer vFXRenderer)) {
                vFXRenderer.sortingOrder = _sortingOrder;
                vFXRenderer.sortingLayerName = _layer;
            }
        }
    }
}