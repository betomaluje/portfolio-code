using UnityEngine;

namespace Weapons {
    public class PoolBullet : BaseBullet {
        [SerializeField]
        private float _lifetime = 6f;

        private float _currentTime;

        private void OnEnable() {
            _currentTime = 0f;
        }

        private void Update() {
            _currentTime += Time.deltaTime;
            if (_currentTime >= _lifetime) {
                gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (CheckCollision(other)) {
                gameObject.SetActive(false);
            }
        }
    }
}