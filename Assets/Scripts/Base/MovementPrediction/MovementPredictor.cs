using UnityEngine;

namespace Base.MovementPrediction {
    public class MovementPredictor {
        private readonly Rigidbody2D _targetRigidbody;
        private readonly Transform _targetTransform;
        private readonly Transform _selfTransform;
        private readonly float _predictionChance;

        public MovementPredictor(Transform targetTransform, Transform selfTransform, float predictionChance = .85f) {
            _targetTransform = targetTransform;
            _selfTransform = selfTransform;
            _predictionChance = Mathf.Clamp01(predictionChance);

            _targetRigidbody = targetTransform.GetComponent<Rigidbody2D>();
        }

        public Vector2 PredictTargetDirection(bool isDebug = false) {
            if (_targetTransform == null || _targetRigidbody == null || _selfTransform == null) {
                return Vector2.zero;
            }

            Vector2 targetPosition = _targetTransform.position;
            Vector2 targetVelocity = _targetRigidbody.linearVelocity;
            Vector2 currentPosition = _selfTransform.position;

            Vector2 basicDirection = (targetPosition - currentPosition).normalized;

            if (_predictionChance < 1 && Random.value > _predictionChance) {
                // if by chance we don't predict, return current direction
                return basicDirection;
            }

            if (targetVelocity.sqrMagnitude < Mathf.Epsilon) {
                // Return direction to current position
                return basicDirection;
            }

            float targetSpeed = targetVelocity.magnitude;

            // Relative position and velocity
            Vector2 relativePosition = targetPosition - currentPosition;
            Vector2 relativeVelocity = targetVelocity;

            // Solve using the quadratic formula: at^2 + bt + c = 0
            float a = relativeVelocity.sqrMagnitude - targetSpeed * targetSpeed;
            float b = 2 * Vector2.Dot(relativePosition, relativeVelocity);
            float c = relativePosition.sqrMagnitude;

            // Handle linear case when `a` is nearly zero
            if (Mathf.Abs(a) < Mathf.Epsilon) {
                float time = relativePosition.magnitude / targetSpeed;
                Vector2 linearPrediction = targetPosition + targetVelocity * time;

                var linearDirection = (linearPrediction - currentPosition).normalized;

                if (isDebug) {
                    DrawDebugPrediction(linearDirection);
                }

                return linearDirection;
            }

            float discriminant = b * b - 4 * a * c;
            if (discriminant < 0) {
                // No valid interception point, fall back to current direction
                if (isDebug) {
                    DrawDebugPrediction(basicDirection);
                }
                return basicDirection;
            }

            float sqrtDiscriminant = Mathf.Sqrt(discriminant);
            float t1 = (-b + sqrtDiscriminant) / (2 * a);
            float t2 = (-b - sqrtDiscriminant) / (2 * a);

            float interceptionTime = Mathf.Min(t1, t2);
            if (interceptionTime < 0) {
                interceptionTime = Mathf.Max(t1, t2); // Use the larger positive root
            }

            if (interceptionTime < 0) {
                // If no valid positive time, return current direction
                if (isDebug) {
                    DrawDebugPrediction(basicDirection);
                }
                return basicDirection;
            }

            Vector2 predictedPosition = targetPosition + targetVelocity * interceptionTime;

            // Return normalized direction towards the predicted position
            var result = (predictedPosition - currentPosition).normalized;

            if (isDebug) {
                DrawDebugPrediction(result);
            }

            return result;
        }

        // Helper method to visualize prediction in debug
        private void DrawDebugPrediction(Vector2 direction, float duration = .5f) {
#if UNITY_EDITOR
            Debug.DrawRay(_selfTransform.position, direction, Color.yellow, duration);
            Debug.DrawLine(_selfTransform.position, _targetTransform.position, Color.magenta, duration);
#endif
        }
    }
}