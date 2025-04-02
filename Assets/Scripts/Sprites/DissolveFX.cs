using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sprites {
    [RequireComponent(typeof(SpriteRenderer))]
    public class DissolveFX : MonoBehaviour {
        private const string fadeProperty = "_FadeAmount";

        [SerializeField]
        private bool _destroyContainer = true;

        [ShowIf("_destroyContainer")]
        [SerializeField]
        private GameObject _container;

        [SerializeField]
        private float _duration = 1f;

        private SpriteRenderer _spriteRenderer;

        private void Awake() {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Dissolve() {
            StartCoroutine(DoDissolve());
        }

        private IEnumerator DoDissolve() {
            var preAmount = _spriteRenderer.material.GetFloat(fadeProperty);
            var endAmount = 1f;
            var elapsed = 0f;
            while (elapsed < _duration) {
                elapsed += Time.deltaTime;
                var percentage = Mathf.Lerp(preAmount, endAmount, elapsed / _duration);
                _spriteRenderer.material.SetFloat(fadeProperty, percentage);
                yield return null;
            }

            _spriteRenderer.material.SetFloat(fadeProperty, endAmount);

            if (_destroyContainer && _container != null) {
                Destroy(_container);
            }
        }
    }
}