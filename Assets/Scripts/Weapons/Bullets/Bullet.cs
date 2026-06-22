using UnityEngine;

namespace Weapons {
    public class Bullet : BaseBullet {
        [SerializeField]
        [Min(0f)]
        private float _lifetime = 1f;

        private void Start() {
            if (gameObject.activeInHierarchy && _lifetime > 0) {
                Destroy(gameObject, _lifetime);
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (CheckCollision(other)) {
                SpawnCollisionParticles(other.transform.position);
                PlayImpactSound(other.transform.position);
                Destroy(gameObject);
            }

        }
    }
}