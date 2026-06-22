using BerserkPixel.Health;
using BerserkPixel.Utils;
using DG.Tweening;
using Extensions;
using UnityEngine;
using Utils;

namespace Weapons {
    [RequireComponent(typeof(Rigidbody2D))]
    public class ThrowableProjectile : MonoBehaviour {
        [SerializeField]
        private LayerMask _targetMask;

        [Tooltip("The prefab to spawn when firing.")]
        [SerializeField]
        protected Rigidbody2D _rb;

        private Weapon _weapon;
        private Vector2 _startPoint;
        private Vector2 _direction;
        private ThrowablePath _path;
        private float _speed = 5f;
        private float _elapsedTime;
        private GameObject _originalWeapon;

        private void OnValidate() {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void Launch(Weapon weapon, Vector3 startPoint, Vector2 direction, ThrowablePath path, float speed, GameObject originalWeapon) {
            _weapon = weapon;
            _startPoint = startPoint;
            _direction = direction;
            _path = path;
            _speed = speed;
            _elapsedTime = 0f;
            _originalWeapon = originalWeapon;

            _originalWeapon.SetActive(false);

            transform.RotateTo(direction);

            if (_path is IRangeLimitable rangeLimitable) {
                rangeLimitable.SetMaxRange(weapon.Range);
                rangeLimitable.OnOutOfRange += RestoreGameObject;
            }
        }

        private void RestoreGameObject() {
            if (_originalWeapon == null) {
                Destroy(gameObject);
            }
            else {
                transform.DOMove(_originalWeapon.transform.position, 0.2f).OnComplete(() => {
                    _originalWeapon.SetActive(true);
                    Destroy(gameObject);
                });
            }
        }

        private void OnDestroy() {
            if (_path is IRangeLimitable rangeLimitable) {
                rangeLimitable.OnOutOfRange -= RestoreGameObject;
            }
        }

        private async void FixedUpdate() {
            if (_path != null && _rb != null) {
                _elapsedTime += Time.fixedDeltaTime;
                // Calculate the next position using the path
                Vector2 nextPosition = await _path.GetPosition(_elapsedTime, _startPoint, _direction, _speed);

                // Move the Rigidbody2D to the calculated position
                _rb.MovePosition(nextPosition);
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (CheckCollision(other)) {
                RestoreGameObject();
            }
        }

        private bool CheckCollision(Collider2D other) {
            if (_weapon != null && _targetMask.LayerMatchesObject(other)) {
                var hit = other.Detect(_targetMask);
                var dir = (hit.transform.position - transform.position).normalized;
                var hitData = new HitDataBuilder()
                    .WithDirection(dir)
                    .WithWeapon(_weapon)
                    .Build(transform, hit.gameObject.transform);

                hitData.PerformDamage(other);

                return true;
            }

            return false;
        }
    }
}
