using Extensions;
using System.Collections;
using UnityEngine;

namespace Modifiers.Powerups {
    public class PowerupShineFX : MonoBehaviour {
        [Header("Animation")]
        [SerializeField]
        private Transform _target;

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private float _fxTime = 1f;

        [SerializeField]
        private float _timeBetweenFx = 4f;

        private Material _material;

        private void Awake() {
            _material = _spriteRenderer.material;
        }

        private void OnValidate() {
            var child = transform.FindChildren("Sprite");
            if (child != null && child.TryGetComponent(out _spriteRenderer)) {
                _target = _spriteRenderer.transform;
            }
        }

        private void Start() {
            InvokeRepeating(nameof(BeginFX), 0f, _timeBetweenFx);
        }

        private void OnDisable() {
            CancelInvoke(nameof(BeginFX));
        }

        private void BeginFX() {
            StartCoroutine(DoShine(0, 1));
        }

        private IEnumerator DoShine(float from, float to) {
            _material.SetFloat("_ShineLocation", 0);
            var elapsed = 0f;

            while (elapsed < _fxTime) {
                elapsed += Time.deltaTime;
                var percentage = Mathf.Lerp(from, to, elapsed / _fxTime);
                _material.SetFloat("_ShineLocation", percentage);
                yield return null;
            }

            _material.SetFloat("_ShineLocation", to);
            yield return new WaitForSeconds(_timeBetweenFx);
        }
    }
}