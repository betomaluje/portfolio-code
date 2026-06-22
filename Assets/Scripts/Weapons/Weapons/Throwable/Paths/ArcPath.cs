using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Weapons {
    [CreateAssetMenu(menuName = "Aurora/ThrowablePaths/Arc Path")]
    public class ArcPath : ThrowablePath {
        [SerializeField]
        private AnimationCurve _speedOverTime = AnimationCurve.Linear(0, 1, 1, 1);

        [SerializeField, Tooltip("Controls the width of the elliptical path.")]
        private float _arcWidth = 5f;

        [SerializeField, Tooltip("Controls the height of the elliptical path.")]
        private float _arcHeight = 2f;

        [SerializeField, Tooltip("Total duration of the boomerang's flight.")]
        private float _totalDuration = 3.0f;

		public override UniTask<Vector2> GetPosition(float elapsedTime, Vector2 startingPoint, Vector2 direction, float speed) {
			// Clamp time to range [0, _totalDuration]
			elapsedTime = Mathf.Clamp(elapsedTime, 0, _totalDuration);

			// Normalize time to [0, 1] for the full path
			float normalizedTime = elapsedTime / _totalDuration;

			// Calculate the progress for the forward and return paths
			float progress = normalizedTime <= 0.5f
				? normalizedTime * 2f // Forward: [0, 1]
				: (1f - normalizedTime) * 2f; // Return: [1, 0]

			// Evaluate speed using the curve
			float adjustedSpeed = speed * _speedOverTime.Evaluate(progress);

			// Ellipse parameters
			float angle = progress * Mathf.PI; // Map progress to an arc [0, π]
			float x = Mathf.Cos(angle) * _arcWidth * adjustedSpeed; // Horizontal movement
			float y = Mathf.Sin(angle) * _arcHeight; // Vertical arc

			// Calculate direction-aligned offsets
			Vector2 forwardOffset = direction.normalized * x;
			Vector2 heightOffset = Vector2.Perpendicular(direction).normalized * y;

			// Combine offsets with the starting point
			return UniTask.FromResult(startingPoint + forwardOffset + heightOffset);
		}
	}

}