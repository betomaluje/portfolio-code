using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Weapons {
    [CreateAssetMenu(menuName = "Aurora/ThrowablePaths/Linear Path")]
    public class LinearPath : ThrowablePath, IRangeLimitable {
        private float _maxDistance;

        public event Action OnOutOfRange;

        private bool _wasOutOfRange;

        public void SetMaxRange(float maxRange) {
            _maxDistance = maxRange;
            _wasOutOfRange = false;
        }

        public override async UniTask<Vector2> GetPosition(float elapsedTime, Vector2 startPoint, Vector2 direction, float speed) {
            // Calculate the intended position
            Vector2 intendedPosition = startPoint + direction.normalized * speed * elapsedTime;

            // Calculate the distance from the start point
            float distance = Vector2.Distance(startPoint, intendedPosition);

            // If the distance exceeds the max allowed distance, clamp to the max distance
            if (!_wasOutOfRange && distance > _maxDistance) {
                await UniTask.Yield();
                OnOutOfRange?.Invoke();
                _wasOutOfRange = true;
                return startPoint + direction.normalized * _maxDistance;
            }

            // Return the intended position if within range
            return intendedPosition;
        }
    }
}