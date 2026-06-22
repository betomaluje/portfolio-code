using BerserkPixel.Health;
using BerserkPixel.Utils;
using BerserkPixel.Utils.ServiceLocator;
using Camera;
using Player;
using Sounds;
using UnityEngine;

namespace Weapons {
    [RequireComponent(typeof(Rigidbody2D))]
    public class BounceUntilCollision : MonoBehaviour {
        [Tooltip("The layer mask to use for hit detection.")]
        [SerializeField]
        private LayerMask _targetMask;

        [Tooltip("The layer mask to use for bouncing.")]
        [SerializeField]
        private LayerMask _bounceMask;

        [Tooltip("The speed of this object.")]
        [SerializeField]
        private float _speed = 10f;

        [Tooltip("The maximum number of allowed bounces.")]
        [SerializeField]
        private int _maxBounces = 5;

        [SerializeField]
        private int _damage = 10;

        [SerializeField]
        private Transform _explodeParticles;

        [SerializeField]
        private Transform _spriteTransform;

        private readonly float _mouseSensitivity = .005f;

        private int _currentBounces = 0;
        private Rigidbody2D _rigidbody;
        private Vector2 _direction;
        private float _lastX = 1;

        private void Awake() {
            _rigidbody = GetComponent<Rigidbody2D>();
            _rigidbody.gravityScale = 0;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        private void Start() {
            PlayerStateMachine player = FindFirstObjectByType<PlayerStateMachine>();
            if (player != null) {
                FlipSprite(player.LastDirection);
                _direction = new Vector2(player.LastDirection.x, 0f).normalized;
            }
            else {
                _direction = transform.right;
            }

            _rigidbody.linearVelocity = _direction * _speed;
        }

        private void FixedUpdate() {
            _rigidbody.linearVelocity = _direction * _speed;
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            if (_targetMask.LayerMatchesObject(collision)) {
                OnHitTarget(collision.gameObject);
            }
            else if (_bounceMask.LayerMatchesObject(collision)) {
                Bounce(collision.contacts[0].normal);
            }
            else {
                // If it hits something unexpected, still bounce
                Bounce(collision.contacts[0].normal);
            }
        }

        private void Bounce(Vector2 normal) {
            _currentBounces++;
            _direction = Vector2.Reflect(_direction, normal).normalized;

            FlipSprite(_direction);

            if (_currentBounces >= _maxBounces) {
                Explode();
            }
        }

        private void Explode() {
            if (_explodeParticles != null) {
                Instantiate(_explodeParticles, transform.position, Quaternion.identity);
            }

            NonPersistentServiceLocator.Get<SoundManager>().PlayWithPitchOnSpot("bomb_explosion", transform.position);
            NonPersistentServiceLocator.Get<CinemachineCameraShake>().ShakeCamera(transform, 6, 1);

            Destroy(gameObject);
        }

        private void FlipSprite(Vector2 direction) {
            // any input? or if we are moving only vertically
            if (direction.sqrMagnitude <= _mouseSensitivity) {
                return;
            }

            var sign = Mathf.Sign(direction.x);

            // check if we are already facing in that direction 
            if (Mathf.Sign(_spriteTransform.localScale.x) == _lastX && sign == _lastX) {
                return;
            }

            _lastX = sign;

            SetScale();
        }

        private void SetScale() {
            var localScale = _spriteTransform.localScale;
            localScale.x = _lastX;
            localScale.y = 1;
            localScale.z = 1f;
            _spriteTransform.localScale = localScale;
        }

        protected virtual void OnHitTarget(GameObject target) {
            var dir = (transform.position - target.transform.position).normalized;

            var hitData = new HitDataBuilder()
                .WithDamage(_damage)
                .WithDirection(dir)
                .Build(transform, target.transform);

            hitData.PerformDamage(target);

            Explode();
        }
    }
}