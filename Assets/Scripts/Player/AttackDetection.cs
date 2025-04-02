using UnityEngine;

namespace Player {
    public class AttackDetection {
        private readonly Collider2D _collider;

        public AttackDetection(Collider2D collider) {
            _collider = collider;
        }

        public Collider2D Detect(LayerMask targetMask) {
            var bounds = _collider.bounds;

            var hit = Physics2D.OverlapBox(bounds.center, bounds.size, 0, targetMask);

            return hit;
        }
    }
}