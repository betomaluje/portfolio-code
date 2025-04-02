using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BerserkTools.Health.UI {
    public class HealthBarColor : MonoBehaviour {
        [SerializeField]
        private Image _foregroundImage;

        [SerializeField]
        private float _duration = 1f;

        // Called from Editor
        public void ChangeColor(Color newColor) {
            StartCoroutine(DoChangeColor(newColor));
        }

        private IEnumerator DoChangeColor(Color newColor) {
            var preColor = _foregroundImage.color;
            var elapsed = 0f;
            while (elapsed < _duration) {
                elapsed += Time.deltaTime;
                _foregroundImage.color = Color.Lerp(preColor, newColor, elapsed / _duration);
                yield return null;
            }

            _foregroundImage.color = newColor;
        }
    }
}