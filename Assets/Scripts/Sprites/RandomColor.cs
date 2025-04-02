using Sirenix.OdinInspector;
using UnityEngine;

namespace Sprites {
    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteRenderer))]
    public class RandomColor : MonoBehaviour {
        [SerializeField]
        [Range(0, 1)]
        [OnValueChanged("SetRandomColor")]
        private float minHue;

        [SerializeField]
        [Range(0, 1)]
        [OnValueChanged("SetRandomColor")]
        private float maxHue;

        private SpriteRenderer spriteRenderer;

        private void Awake() {
            spriteRenderer = GetComponent<SpriteRenderer>();
            SetRandomColor();
        }

        private void SetRandomColor() {
            spriteRenderer.color = Random.ColorHSV(minHue, maxHue);
        }
    }
}