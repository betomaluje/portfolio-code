using UnityEngine;

namespace BerserkPixel.Health {
    public static class HealthExt {
        /// <summary>
        /// Searches all IHealth components in the parent and its children so we can perform damage
        /// </summary>
        /// <param name="hitData"></param>
        /// <param name="other"></param>
        public static void PerformDamage(this HitData hitData, GameObject other) {
            if (other.FindAllInChildren(out IHealth[] healths)) {
                foreach (var health in healths) {
                    health.PerformDamage(hitData);
                }
            }
        }

        public static void PerformDamage(this HitData hitData, Collider2D other) {
            if (other.gameObject.FindAllInChildren(out IHealth[] healths)) {
                foreach (var health in healths) {
                    health.PerformDamage(hitData);
                }
            }
        }

        private static bool FindAllInChildren<T>(this GameObject parent, out T[] components) {
            components = parent.GetComponentsInChildren<T>();
            return components != null && components.Length > 0;
        }
    }
}