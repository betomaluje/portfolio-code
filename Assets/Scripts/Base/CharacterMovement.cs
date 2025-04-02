using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Base {
    public class CharacterMovement : IMove {
        private readonly Rigidbody2D _rigidbody;
        private readonly Transform _spriteTransform;

        private float _lastX = 1;

        public float LastX => _lastX;

        private float _localScaleX, _localScaleY;

        public ParticleSystem MovementParticles { get; set; }

        // movement speed influencer
        private float _moveFactor = 1f;

        // sprite scale influencer
        private float _scaleFactor = 1f;
        private bool _isScaleInfluenced = false;

        /// <summary>
        /// Initializes the rigidbody, sprite transform, and movement influencer.
        /// </summary>
        /// <param name="rigidbody"></param>
        /// <param name="spriteTransform"></param>
        /// <param name="influencer"></param>
        public CharacterMovement(Rigidbody2D rigidbody, Transform spriteTransform) {
            _rigidbody = rigidbody;
            _spriteTransform = spriteTransform;
            _localScaleX = _spriteTransform.localScale.x;
            _localScaleY = _spriteTransform.localScale.y;
            _rigidbody.linearDamping = 10f;
        }

        public void Stop() {
            _rigidbody.linearVelocity = Vector2.zero;
        }

        /// <summary>
        /// Moves an entity using it's rigidbody. If the entity has a movement particles, it will play it.
        /// Decided to do it this way because of: https://youtu.be/EMhTROG0nAw?si=kG-wwKySA11uR_n9&t=372
        /// </summary>
        /// <param name="velocity">The velocity Vector. Basically a speed multiplied with a direction</param>
        public void Move(Vector2 velocity) {
            _rigidbody.linearVelocity = velocity * _moveFactor;
            TryAndPlayParticles(velocity);
        }

        private void TryAndPlayParticles(Vector2 velocity) {
            if (velocity.sqrMagnitude > .1f && MovementParticles != null) {
                MovementParticles.Play();
            }
        }

        public async void ApplyForce(Vector2 direction, float speed, float duration) {
            var timer = duration;
            direction = direction.normalized;
            TryAndPlayParticles(direction);
            while (timer > 0) {
                timer -= Time.deltaTime;
                if (_rigidbody != null) {
                    _rigidbody.linearVelocity = _moveFactor * speed * direction;
                }

                await UniTask.Yield();
            }

            if (_rigidbody != null) {
                _rigidbody.linearVelocity = Vector2.zero;
            }
        }

        public async void MoveToPoint(Vector2 endPoint, float duration) {
            var elapsed = 0f;
            var initialPosition = _rigidbody.transform.position;
            while (elapsed < duration) {
                elapsed += Time.deltaTime;
                if (_rigidbody != null) {
                    _rigidbody.transform.position = Vector2.Lerp(initialPosition, endPoint, elapsed / duration);
                }

                await UniTask.Yield();
            }

            if (_rigidbody != null) {
                _rigidbody.transform.position = endPoint;
                _rigidbody.linearVelocity = Vector2.zero;
            }
        }

        public void FlipSprite(Vector2 direction) {
            // any input? or if we are moving only vertically
            if (direction.sqrMagnitude <= .1f || direction.x == 0f) {
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
            localScale.x = _localScaleX * _lastX;
            localScale.y = _localScaleY;
            localScale.z = 1f;
            _spriteTransform.localScale = localScale * _scaleFactor;
        }

        /// <summary>
        /// Sets the movement influence factor.
        /// </summary>
        /// <param name="amount">The amount to multiply to the current speed</param>
        public void SetMovementInfluence(float amount) {
            if (amount == _moveFactor) {
                return;
            }

            _moveFactor = amount;
        }

        public void ResetMovementInfluence() {
            _moveFactor = 1f;
        }

        /// <summary>
        /// Sets the scale influence factor.
        /// </summary>
        /// <param name="amount">The amount to multiply to the current scale</param>
        /// <param name="duration">If negative, it's forever, otherwise it's the duration</param>
        public void SetScaleInfluence(float amount) {
            if (_isScaleInfluenced && amount == _scaleFactor) {
                return;
            }

            _isScaleInfluenced = true;
            _scaleFactor = amount;
            SetScale();
        }

        public void ResetScaleInfluence() {
            _scaleFactor = 1f;
            _isScaleInfluenced = false;
            SetScale();
        }
    }
}