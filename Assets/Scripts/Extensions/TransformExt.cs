using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Extensions {
    public static class TransformExt {
        public static void DestroyChildren(this Transform t) {
            if (t.childCount <= 0) {
                return;
            }

            while (t.childCount > 0) {
                var child = t.GetChild(0).gameObject;
#if UNITY_EDITOR
                UnityEngine.Object.DestroyImmediate(child);
#else
                UnityEngine.Object.Destroy(child);
#endif
            }
        }

        public static async UniTask DestroyChildrenAsync(this Transform t) {
            if (t.childCount <= 0) {
                return;
            }

            while (t.childCount > 0) {
                var child = t.GetChild(0).gameObject;
#if UNITY_EDITOR
                UnityEngine.Object.DestroyImmediate(child);
#else
                UnityEngine.Object.Destroy(child);
#endif

                await UniTask.Yield();
            }
        }

        public static void HideChildren(this Transform t) {
            if (t.childCount <= 0)
                return;

            foreach (Transform child in t) {
                child.gameObject.SetActive(false);
            }
        }

        public static void ShowChildren(this Transform t) {
            if (t.childCount <= 0)
                return;

            foreach (Transform child in t) {
                child.gameObject.SetActive(true);
            }
        }

        public static void HideChildren(this RectTransform t) {
            if (t.childCount <= 0)
                return;

            foreach (Transform child in t) {
                child.gameObject.SetActive(false);
            }
        }

        public static void ShowChildren(this RectTransform t) {
            if (t.childCount <= 0)
                return;

            foreach (Transform child in t) {
                child.gameObject.SetActive(true);
            }
        }

        public static Transform FindChildren(this Transform self, string exactName) => self.FindRecursive(child => child.name == exactName);

        public static Transform FindRecursive(this Transform self, Func<Transform, bool> selector) {
            foreach (Transform child in self) {
                if (selector(child)) {
                    return child;
                }

                var finding = child.FindRecursive(selector);

                if (finding != null) {
                    return finding;
                }
            }

            return null;
        }

        public static Quaternion GetRotation(Vector2 direction) {
            var aimAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return Quaternion.Euler(Vector3.forward * aimAngle);
        }

        public static void RotateTo(this Transform t, Vector2 direction) {
            var aimAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var rotation = Quaternion.Euler(Vector3.forward * aimAngle);
            t.rotation = rotation;
        }

        public static void LocalRotateTo(this Transform t, Vector2 direction) {
            var aimAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var rotation = Quaternion.Euler(Vector3.forward * aimAngle);
            t.localRotation = rotation;
        }

        /// <summary>
        /// Projects several points in a straight line from a given Transform.
        /// </summary>
        /// <param name="origin">The Transform to project from.</param>
        /// <param name="amount">The number of points to project.</param>
        /// <param name="range">The total range over which points are spread.</param>
        /// <param name="direction">The direction of projection (normalized vector).</param>
        /// <returns>List of Vector2 points representing projected positions.</returns>
        public static List<Vector2> ProjectPoints(this Transform origin, int amount, float range, Vector2 direction, bool bothWays = false) {
            // Ensure direction is normalized
            direction = direction.normalized;

            // Calculate spacing between points
            float spacing = range / (amount - 1);

            Vector2 startPosition;
            if (bothWays) {
                // Start position (centered if amount is odd, else slightly offset)
                startPosition = (Vector2)origin.position - direction * (range / 2);
            }
            else {
                // Start position is the origin's position
                startPosition = origin.position;
            }

            // Generate points
            List<Vector2> points = new();
            for (int i = 0; i < amount; i++) {
                Vector2 point = startPosition + direction * (i * spacing);
                points.Add(point);
            }

            return points;
        }
    }
}