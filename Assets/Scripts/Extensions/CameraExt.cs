using UnityEngine;

namespace Extensions {
    public static class CameraExt {
        public static bool IsObjectVisible(this UnityEngine.Camera camera, Transform transform) {
            if (transform == null) {
                return false;
            }
            if (transform.TryGetComponent<Collider2D>(out var collider)) {
                return IsObjectVisible(camera, collider);
            }
            if (transform.TryGetComponent<Renderer>(out var renderer)) {
                return IsObjectVisible(camera, renderer);
            }

            return false;
        }

        public static bool IsObjectVisible(this UnityEngine.Camera camera, Renderer renderer) =>
            IsObjectVisible(camera, renderer.bounds);

        public static bool IsObjectVisible(this UnityEngine.Camera camera, Collider2D collider) =>
            IsObjectVisible(camera, collider.bounds);

        private static bool IsObjectVisible(this UnityEngine.Camera camera, Bounds bounds) =>
            GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(camera), bounds);
    }
}