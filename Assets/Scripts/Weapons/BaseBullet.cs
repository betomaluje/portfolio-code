using BerserkPixel.Health;
using BerserkPixel.Utils;
using Extensions;
using UnityEngine;
using Utils;

namespace Weapons {
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class BaseBullet : MonoBehaviour, IBullet {
        [Tooltip("The layer mask to use for hit detection.")]
        [SerializeField]
        protected LayerMask _targetMask;

        [Tooltip("The speed of this spit.")]
        [SerializeField]
        protected float _speed = 10f;

        [Tooltip("The prefab to spawn when firing.")]
        [SerializeField]
        protected Rigidbody2D _rb;

        [SerializeField]
        private bool _rotateOnFire = true;

        private Weapon _weapon;
        private int _damage;

        protected virtual void OnValidate() {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void SetWeapon(Weapon weapon) {
            _weapon = weapon;
            _damage = _weapon.GetDamage();
        }

        public void SetDamage(int damage) {
            _damage = damage;
        }

        public virtual void Fire(Vector2 direction) {
            if (_rb == null) {
                return;
            }

            if (_rotateOnFire) {
                transform.RotateTo(direction);
            }

            _rb.AddForce(direction * _speed, ForceMode2D.Impulse);
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