using System;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace Detection {
    public class MultipleTargetDetection : Detection {
        [SerializeField]
        [Min(0)]
        private float _distanceThreshold = 1f;

        public UnityEvent<Transform, float> OnClosestDetected;

        private float _lastDistance = 0;

        protected override void Detect() {
            var hitCount = _collider.DetectAllWithAngle(_targetMask, out var hits, _detectionAngle);

            if (hitCount == 0 || hits.Length == 0) {
                return;
            }

            Transform closest = null;
            float closestDistance = float.MaxValue;
            var currentPosition = transform.position;

            foreach (var hit in hits) {
                if (hit == null) {
                    continue;
                }

                var directionToTarget = (hit.transform.position - currentPosition).sqrMagnitude;
                if (directionToTarget < closestDistance) {
                    closestDistance = directionToTarget;
                    closest = hit.transform;
                }
            }

            if (closest != null && Math.Abs(_lastDistance - closestDistance) > _distanceThreshold) {
                _lastDistance = closestDistance;
                OnClosestDetected?.Invoke(closest, closestDistance);
            }
        }
    }
}