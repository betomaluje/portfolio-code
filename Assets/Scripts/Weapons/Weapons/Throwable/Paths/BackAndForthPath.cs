using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Weapons {
    [CreateAssetMenu(menuName = "Aurora/ThrowablePaths/Back And Forth Path")]
    public class BackAndForthPath : ThrowablePath, IRangeLimitable {
        [SerializeField]
        private AnimationCurve _speedOverTime = new(
            new Keyframe(0f, 0f, 0f, 1f),   // Ease in
            new Keyframe(0.5f, 1f, 0f, 0f), // Smooth transition point
            new Keyframe(1f, 0f, -1f, 0f)   // Ease out
        );

        [Tooltip("The total duration of the back-and-forth motion.")]
        [SerializeField]
        private float _motionDuration = 2f;

        private float _maxDistance = 5f;

        public event Action OnOutOfRange;

        private bool _wasOutOfRange;

        public void SetMaxRange(float maxRange) {
            _maxDistance = maxRange;
            _wasOutOfRange = false;
        }

        public override async UniTask<Vector2> GetPosition(float elapsedTime, Vector2 startPoint, Vector2 direction, float speed) {
            if (!_wasOutOfRange && elapsedTime > _motionDuration) {
                await UniTask.Yield();
                OnOutOfRange?.Invoke();
                _wasOutOfRange = true;
                return startPoint;
            }

            float normalizedTime = Mathf.Clamp01(elapsedTime / _motionDuration);
            float smoothProgress = Mathf.SmoothStep(0f, 1f, normalizedTime);
            float backAndForthProgress = Mathf.Lerp(1f, -1f, smoothProgress);

            float adjustedSpeed = _speedOverTime.Evaluate(smoothProgress) * speed;
            float distanceTraveled = backAndForthProgress * _maxDistance * adjustedSpeed;

            distanceTraveled = Mathf.Clamp(distanceTraveled, -_maxDistance, _maxDistance);

            return startPoint + direction.normalized * distanceTraveled;
        }
    }
}