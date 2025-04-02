using UnityEngine;

namespace Weapons {
    public class Bullet : BaseBullet {
        [SerializeField]
        private float _lifetime = 1f;

        private void Start() {
            if (gameObject.activeInHierarchy) {
                Destroy(gameObject, _lifetime);
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (CheckCollision(other)) {
                Destroy(gameObject);
            }
        }
    }
}