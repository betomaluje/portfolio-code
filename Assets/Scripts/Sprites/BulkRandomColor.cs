using Sirenix.OdinInspector;
using UnityEngine;

namespace Sprites {
    [ExecuteInEditMode]
    public class BulkRandomColor : MonoBehaviour {
        [SerializeField]
        [Range(0, 1)]
        [OnValueChanged("SetRandomColor")]
        private float minHue;

        [SerializeField]
        [Range(0, 1)]
        [OnValueChanged("SetRandomColor")]
        private float maxHue;

        private SpriteRenderer[] spriteRenderers;

        private void Awake() {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            SetRandomColor();
        }

        private void SetRandomColor() {
            var color = Random.ColorHSV(minHue, maxHue);
            foreach (var spriteRenderer in spriteRenderers) spriteRenderer.color = color;
        }
    }
}