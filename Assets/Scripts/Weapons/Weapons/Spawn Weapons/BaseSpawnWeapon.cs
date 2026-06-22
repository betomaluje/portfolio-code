using BerserkPixel.Health;
using BerserkPixel.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

namespace Weapons {
    public abstract class BaseSpawnWeapon : MonoBehaviour, ISpawnedWeapon {
        [BoxGroup("General Settings")]
        [Tooltip("The layer mask to use for hit detection.")]
        [SerializeField]
        protected LayerMask _targetMask;

        protected abstract MovementConfig MovementConfig { get; }

        protected Weapon _weapon;
        protected int _damage;
        protected Vector3 _direction;

        public abstract void Shoot();

        public void SetWeapon(Weapon weapon) {
            _weapon = weapon;
            _damage = _weapon.GetDamage();
        }

        public void SetDirection(Vector2 direction) {
            _direction = direction;
        }

        public void SetDamage(int damage) {
            _damage = damage;
        }

        public void Activate() {
            gameObject.SetActive(true);
        }

        protected void Deactivate() {
            gameObject.SetActive(false);
        }

        protected virtual bool CheckCollision(Collider2D other) {
            if (_targetMask.LayerMatchesObject(other)) {
                var hit = other.Detect(_targetMask);
                var dir = (hit.transform.position - transform.position).normalized;
                var builder = new HitDataBuilder()
                    .WithDirection(dir);

                if (_weapon != null) {
                    builder.WithWeapon(_weapon);
                }
                else {
                    builder.WithDamage(_damage);
                }

                var hitData = builder.Build(transform, hit.gameObject.transform);

                hitData.PerformDamage(other);

                return true;
            }

            return false;
        }
    }
}