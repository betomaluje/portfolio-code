using Sirenix.OdinInspector;
using UnityEngine;

namespace Sprites {
    [RequireComponent(typeof(SpriteRenderer))]
    public class RandomSprite : MonoBehaviour {
        [SerializeField]
        private Sprite[] sprites;

        [SerializeField]
        private bool changeAtRuntime = true;

        private SpriteRenderer spriteRenderer;

        private void Awake() {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (changeAtRuntime) {
                SetRandomSprite();
            }
        }

        [Button]
        private void EditorSetRandomSprite() {
            var s = sprites[Random.Range(0, sprites.Length)];
            GetComponent<SpriteRenderer>().sprite = s;
        }

        private void SetRandomSprite() {
            var s = sprites[Random.Range(0, sprites.Length)];
            spriteRenderer.sprite = s;
        }
    }
}