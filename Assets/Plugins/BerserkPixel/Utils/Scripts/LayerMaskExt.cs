using UnityEngine;

namespace BerserkPixel.Utils {
    public static class LayerMaskExt {
        public static bool LayerMatchesObject(this LayerMask layer, Collider collider) {
            return LayerMatchesObject(layer, collider.gameObject);
        }

        public static bool LayerMatchesObject(this LayerMask layer, Collider2D collider) {
            return LayerMatchesObject(layer, collider.gameObject);
        }

        public static bool LayerMatchesObject(this LayerMask layer, Collision collision) {
            return LayerMatchesObject(layer, collision.gameObject);
        }

        public static bool LayerMatchesObject(this LayerMask layer, GameObject gameObject) {
            return ((1 << gameObject.gameObject.layer) & layer) != 0;
        }

        public static bool LayerMatchesObject(this int layer, string layerName) {
            LayerMask layerMask = LayerMask.NameToLayer(layerName);
            return ((1 << layer) & layerMask) != 0;
        }
    }
}