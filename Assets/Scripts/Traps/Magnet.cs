using BerserkPixel.Utils;
using Level;
using UnityEngine;

namespace Traps {
    public class Magnet : MonoBehaviour {
        [SerializeField]
        private LayerMask _target;

        [SerializeField]
        private float _magnetStrength = 1f;

        private void CheckTrigger(GameObject other) {
            if (_target.LayerMatchesObject(other)) {
                if (other.TryGetComponent(out Rigidbody2D rb)) {
                    PostProcessingManager.Instance.SetProfile("Magnet");
                    rb.AddForce((transform.position - other.transform.position).normalized * _magnetStrength, ForceMode2D.Force);
                }
            }
        }

        private void OnTriggerStay2D(Collider2D other) {
            CheckTrigger(other.gameObject);
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (_target.LayerMatchesObject(other)) {
                var manager = PostProcessingManager.Instance;
                if (manager != null) {
                    manager.Reset();
                }
            }
        }
    }
}