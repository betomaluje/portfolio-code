using UnityEngine;

namespace Extensions {
    public static class SpriteRendererExt {
        public static void UpdateColliderSize(this SpriteRenderer spriteRenderer, BoxCollider2D collider,
            Vector2 margin) {
            var size = (Vector2)spriteRenderer.sprite.bounds.size + margin;
            collider.size = size;
        }

        public static void Tint(this SpriteRenderer[] spriteRenderers, Color color) {
            foreach (var renderer in spriteRenderers) {
                var c = renderer.color;
                var newColor = color;
                newColor.a = c.a;
                renderer.color = newColor;
            }
        }
    }
}