using System.Linq;
using BerserkPixel.Health;
using UnityEngine;

namespace Weapons {
    public class ChaseBullet : BaseBullet {
        [SerializeField]
        private float _lifetime = 3f;

        [SerializeField]
        private int _bulletDamage = 6;

        [SerializeField]
        private float _detectionRadius = 4f;

        private void Start() {
            if (gameObject.activeInHierarchy) {
                Destroy(gameObject, _lifetime);
            }

            SetDamage(_bulletDamage);

            DetectEnemies();
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);
        }

        private void DetectEnemies() {
            Transform closestEnemy = null;
            float closestDistance = float.MaxValue;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _detectionRadius, _targetMask);

            foreach (Collider2D collider in colliders.Where(c => c.TryGetComponent<CharacterHealth>(out var health) && !health.IsDead)) {
                // we want the closest one
                var distance = Vector2.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance) {
                    closestEnemy = collider.transform;
                    closestDistance = distance;
                }
            }

            if (closestEnemy != null) {
                var direction = (closestEnemy.position - transform.position).normalized;
                Fire(direction);
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (CheckCollision(other)) {
                Destroy(gameObject);
            }
        }
    }
}