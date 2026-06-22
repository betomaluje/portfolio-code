using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Preferences;
using UnityEngine;

namespace Base {
    /// <summary>
    /// Base concrete implementation for handling physical entity movement, sprite scaling/flipping, 
    /// and interpolating movements via the Rigidbody2D.
    /// </summary>
    public class CharacterMovement : IMove, IDisposable {
        private readonly Rigidbody2D _rigidbody;
        private readonly Transform _spriteTransform;
        private readonly PreferencesStorage _preferencesStorage = new();

        private readonly float _localScaleX, _localScaleY, _localScaleZ;
        protected float _lastX = 1;

        // Influencers
        private float _moveFactor = 1f;
        private float _scaleFactor = 1f;
        private bool _isScaleInfluenced = false;
        private float _mouseSensitivity = 0.005f;

        // Cancellation for fire-and-forget async methods
        private CancellationTokenSource _asyncCts = new();

        private bool _isDisposed;

        public float LastX => _lastX;
        public ParticleSystem MovementParticles { get; set; }

        public CharacterMovement(Rigidbody2D rigidbody, Transform spriteTransform) {
            _rigidbody = rigidbody;
            _spriteTransform = spriteTransform;
            _localScaleX = _spriteTransform.localScale.x;
            _localScaleY = _spriteTransform.localScale.y;
            _localScaleZ = _spriteTransform.localScale.z;

            _rigidbody.linearDamping = 10f;
            _mouseSensitivity = _preferencesStorage.GetMouseSensitivity();

            PreferencesStorage.OnPreferenceChanged += OnPreferenceChanged;
        }

        public void Dispose() {
            if (_isDisposed) return;
            _isDisposed = true;

            PreferencesStorage.OnPreferenceChanged -= OnPreferenceChanged;

            // Cancel any in-flight ApplyForce / MoveToPoint tasks
            _asyncCts.Cancel();
            _asyncCts.Dispose();

            GC.SuppressFinalize(this);
        }

        private void OnPreferenceChanged(string preferenceName, bool isChecked) {
            if (preferenceName.Equals(PreferencesStorage.EVENT_MOUSE_SENSITIVITY)) {
                _mouseSensitivity = _preferencesStorage.GetMouseSensitivity();
            }
        }

        /// <summary>
        /// Immediately halts all linear velocity on the attached Rigidbody2D.
        /// </summary>
        public void Stop() {
            if (_rigidbody != null) _rigidbody.linearVelocity = Vector2.zero;
            StopParticles();
        }

        /// <summary>
        /// Applies a continuous velocity-based movement scaled by the current influencer factor.
        /// Automatically plays movement particles if velocity exceeds minimum thresholds,
        /// and stops them when the character is stationary.
        /// </summary>
        public void Move(Vector2 velocity) {
            if (_rigidbody != null) _rigidbody.linearVelocity = velocity * _moveFactor;
            HandleParticles(velocity);
        }

        private void HandleParticles(Vector2 velocity) {
            if (MovementParticles == null) return;

            if (velocity.sqrMagnitude > 0.1f) {
                if (!MovementParticles.isPlaying) MovementParticles.Play();
            } else {
                StopParticles();
            }
        }

        private void StopParticles() {
            if (MovementParticles != null && MovementParticles.isPlaying) {
                MovementParticles.Stop();
            }
        }

        /// <summary>
        /// Applies a direct push force linearly over a specific duration via async ticking.
        /// Automatically cancelled when the character is disposed or a new async movement begins.
        /// </summary>
        public async void ApplyForce(Vector2 direction, float speed, float duration) {
            var token = ResetAsyncCts();

            float timer = duration;
            direction = direction.normalized;
            HandleParticles(direction);

            try {
                while (timer > 0 && _rigidbody != null) {
                    token.ThrowIfCancellationRequested();
                    timer -= Time.deltaTime;
                    _rigidbody.linearVelocity = _moveFactor * speed * direction;
                    await UniTask.Yield(token);
                }
            } catch (OperationCanceledException) {
                return;
            }

            if (_rigidbody != null) {
                _rigidbody.linearVelocity = Vector2.zero;
            }

            StopParticles();
        }

        /// <summary>
        /// Linearly interpolates the rigidbody toward a specific end point over the given duration.
        /// Automatically cancelled when the character is disposed or a new async movement begins.
        /// </summary>
        public async void MoveToPoint(Vector2 endPoint, float duration) {
            if (_rigidbody == null) return;

            var token = ResetAsyncCts();

            float elapsed = 0f;
            Vector2 initialPosition = _rigidbody.position;
            Vector2 direction = (endPoint - initialPosition).normalized;
            FlipSprite(direction);

            try {
                while (elapsed < duration && _rigidbody != null) {
                    token.ThrowIfCancellationRequested();
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / duration);

                    Vector2 newPosition = Vector2.Lerp(initialPosition, endPoint, t);
                    _rigidbody.MovePosition(newPosition);

                    await UniTask.Yield(token);
                }
            } catch (OperationCanceledException) {
                return;
            }

            if (_rigidbody != null) {
                _rigidbody.MovePosition(endPoint);
                _rigidbody.linearVelocity = Vector2.zero;
            }
        }

        /// <summary>
        /// Cancels any running async movement task and returns a fresh token for the next one.
        /// </summary>
        private CancellationToken ResetAsyncCts() {
            _asyncCts.Cancel();
            _asyncCts.Dispose();
            _asyncCts = new CancellationTokenSource();
            return _asyncCts.Token;
        }

        /// <summary>
        /// Flips the sprite's local scale correctly depending on the applied directional movement.
        /// Protects against micro-inputs via the underlying mouse sensitivity threshold.
        /// </summary>
        public virtual void FlipSprite(Vector2 direction) {
            if (direction.sqrMagnitude <= _mouseSensitivity) return;

            float sign = Mathf.Sign(direction.x);

            // Skip redundant updates — use tracked _lastX, not localScale, as the source of truth
            if (sign == _lastX) return;

            _lastX = sign;
            SetScale();
        }

        protected void SetScale() {
            if (_spriteTransform == null) return;

            _spriteTransform.localScale = new Vector3(
                _localScaleX * _lastX * _scaleFactor,
                _localScaleY * _scaleFactor,
                _localScaleZ
            );
        }

        /// <summary>
        /// Artificially restricts or boosts movement speed system-wide.
        /// </summary>
        public void SetMovementInfluence(float amount) {
            if (Mathf.Approximately(amount, _moveFactor)) return;
            _moveFactor = amount;
        }

        public void ResetMovementInfluence() => _moveFactor = 1f;

        /// <summary>
        /// Artificially scales the visual sprite system-wide.
        /// </summary>
        public void SetScaleInfluence(float amount) {
            if (_isScaleInfluenced && Mathf.Approximately(amount, _scaleFactor)) return;

            _isScaleInfluenced = true;
            _scaleFactor = amount;
            SetScale();
        }

        public void ResetScaleInfluence() {
            _scaleFactor = 1f;
            _isScaleInfluenced = false;
            SetScale();
        }

        public void MakeBodyKinematic() {
            if (_rigidbody != null) _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        }

        public void MakeBodyDynamic() {
            if (_rigidbody != null) _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}