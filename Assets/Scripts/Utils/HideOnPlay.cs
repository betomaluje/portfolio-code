using UnityEngine;

namespace Utils {
    public class HideOnPlay : MonoBehaviour {
        private void OnEnable() {
            gameObject.hideFlags = gameObject.hideFlags | HideFlags.HideInHierarchy;
        }

        private void OnDisable() {
            if (gameObject.hideFlags.HasFlag(HideFlags.HideInHierarchy)) {
                gameObject.hideFlags = gameObject.hideFlags & ~HideFlags.HideInHierarchy;
            }
        }
    }
}