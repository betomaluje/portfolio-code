using System.Collections.Generic;
using UnityEngine;

namespace Utils {
    public static class CollisionDetection {
        public static Collider2D Detect(this Collider2D collider, LayerMask targetMask) {
            var bounds = collider.bounds;

            var hit = Physics2D.OverlapBox(bounds.center, bounds.size, 0, targetMask);

            return hit;
        }

        public static RaycastHit2D DetectWithAngle(this Collider2D collider, LayerMask targetMask) {
            // we raycast each 30 degrees to detect if there is a wall
            var increment = 30;
            return DetectWithAngle(collider, targetMask, increment);
        }

        public static RaycastHit2D DetectWithAngle(this Collider2D collider, LayerMask targetMask, int increment = 30, bool debug = false) {
            var bounds = collider.bounds;

            // we raycast each X degrees to detect if there is a wall
            var maxAngle = 360;
            var angle = 0;
            RaycastHit2D hit = default;
            while (angle < maxAngle) {
                var direction = Quaternion.Euler(0, 0, angle) * Vector2.right;
                hit = Physics2D.Raycast(bounds.center, direction, bounds.extents.x, targetMask);

                if (debug) {
                    // we draw a gizmo to see the raycast
                    Debug.DrawRay(bounds.center, direction * bounds.extents.x, hit ? Color.blue : Color.red);
                }

                if (hit) {
                    break;
                }
                angle += increment;
            }

            return hit;
        }

        public static int DetectAll(this Collider2D collider, LayerMask targetMask, out Collider2D[] hits) {
            var bounds = collider.bounds;

            hits = new Collider2D[5];
            var filter = new ContactFilter2D {
                useLayerMask = true,
                useTriggers = true
            };
            filter.SetLayerMask(targetMask);
            var hit = Physics2D.OverlapBox(bounds.center, bounds.size, 0, filter, hits);
            return hit;
        }

        public static int DetectAllWithAngle(this Collider2D collider, LayerMask targetMask, out Collider2D[] hits, int increment = 30, bool debug = false) {
            var bounds = collider.bounds;

            // we raycast each X degrees to detect if there is a wall
            var maxAngle = 360;
            var angle = 0;
            int maxHits = 5;
            var tempHits = new List<Collider2D>(maxHits);

            while (angle < maxAngle) {
                var direction = Quaternion.Euler(0, 0, angle) * Vector2.right;
                var hit = Physics2D.Raycast(bounds.center, direction, bounds.extents.x, targetMask);

                if (debug) {
                    // we draw a gizmo to see the raycast
                    Debug.DrawRay(bounds.center, direction * bounds.extents.x, hit ? Color.blue : Color.red);
                }

                if (hit && !tempHits.Contains(hit.collider)) {
                    tempHits.Add(hit.collider);

                    if (tempHits.Count >= maxHits) {
                        break;
                    }
                }
                angle += increment;
            }

            hits = tempHits.ToArray();

            return tempHits.Count;
        }
    }
}