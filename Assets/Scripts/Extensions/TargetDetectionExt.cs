using BerserkPixel.Health;
using BerserkPixel.StateMachine;
using Detection;
using System.Linq;
using UnityEngine;

namespace Extensions {
    public static class TargetDetectionExt {
        /// <summary>
        /// Retrieves the enemy target detection for the given state.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="state">The state to retrieve the enemy target detection for.</param>
        /// <param name="includeDead">Whether to include dead enemies in the search. Defaults to false.</param>
        /// <returns>TargetDetection: The enemy target detection, or null if no enemy is found.</returns>d        
        public static TargetDetection GetEnemy<V, T>(this V state, bool includeDead = false) where V : State<T> where T : MonoBehaviour {
            return GetByType<V, T>(state, TargetType.Enemy, includeDead);
        }

        /// <summary>
        /// Retrieves the ally target detection for the given state.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="state">The state to retrieve the ally target detection for.</param>
        /// <param name="includeDead">Whether to include dead allies in the search. Defaults to true.</param>
        /// <returns>TargetDetection: The ally target detection, or null if no ally is found.</returns>
        public static TargetDetection GetAlly<V, T>(this V state, bool includeDead = true) where V : State<T> where T : MonoBehaviour {
            return GetByType<V, T>(state, TargetType.Ally, includeDead);
        }

        public static Transform GetTarget(this TargetDetection detection, bool includeDead = false) {
            detection.ForceDetection();
            var target = detection.Target;
            if (target == null) {
                return null;
            }
            else {
                if (includeDead) {
                    return detection.Target;
                }

                if (target.TryGetComponent<CharacterHealth>(out var health) && health.IsDead) {
                    return null;
                }
            }

            return detection.Target;
        }

        /// <summary>
        /// Retrieves a TargetDetection instance of the specified type from the given state.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="state">The state to retrieve the TargetDetection instance from.</param>
        /// <param name="type">The type of TargetDetection instance to retrieve.</param>
        /// <param name="includeDead">Whether to include dead targets in the search. Defaults to false.</param>
        /// <returns>TargetDetection: The TargetDetection instance of the specified type, or null if no matching instance is found.</returns>
        private static TargetDetection GetByType<V, T>(this V state, TargetType type, bool includeDead = false) where V : State<T> where T : MonoBehaviour {
            var allDetections = state.GetComponentsInChildren<TargetDetection>();
            if (allDetections == null || allDetections.Length == 0) {
                return null;
            }

            var detection = allDetections.FirstOrDefault(x => x.TargetType == type);
            if (detection == null) {
                return null;
            }

            detection.ForceDetection();

            var target = detection.Target;
            if (target == null) {
                return null;
            }
            else {
                if (includeDead) {
                    return detection;
                }

                if (target.TryGetComponent<CharacterHealth>(out var health) && health.IsDead) {
                    return null;
                }
            }

            return detection;
        }
    }
}