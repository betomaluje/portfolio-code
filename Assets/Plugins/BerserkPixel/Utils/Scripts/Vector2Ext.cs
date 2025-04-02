using UnityEngine;

namespace BerserkPixel.Utils {
    public static class VectorExt {
        public static float Vector2ToAngle(this Vector2 dir, float offsetAngle = 0) {
            return Vector2.SignedAngle(Vector2.up, dir.normalized) + offsetAngle;
        }

        public static float Vector3ToAngle(this Vector3 dir) {
            dir = dir.normalized;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (angle < 0) {
                angle += 360;
            }

            return angle;
        }
    }
}