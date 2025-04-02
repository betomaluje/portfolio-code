using System.Collections;
using BerserkPixel.Health;
using UnityEngine;

namespace UI {
    public class HealthBarHider : MonoBehaviour {
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private float _timeToShow = .5f;

        [SerializeField]
        private float _timeBetween = 2f;

        [SerializeField]
        private float _timeToHide = 1f;

        private Coroutine _currentCoroutine;

        private CharacterHealth _health;

        private void Awake() {
            _health = GetComponentInParent<CharacterHealth>();
        }

        private void Start() {
            _health.OnDamagePerformed += HandleDamagePerformed;
            _health.OnGiveHealth += HandleHealthChanged;
            _canvasGroup.alpha = 0;
        }

        private void OnDestroy() {
            _health.OnDamagePerformed -= HandleDamagePerformed;
            _health.OnGiveHealth -= HandleHealthChanged;

            StopAllCoroutines();
        }

        private void HandleHealthChanged(int currentHealth) {
            StopCoroutine();

            _currentCoroutine = StartCoroutine(Show());
        }

        private void HandleDamagePerformed(HitData hitData) {
            StopCoroutine();

            _currentCoroutine = StartCoroutine(Show());
        }

        private IEnumerator Hide() {
            if (_canvasGroup.alpha <= 0) {
                StopCoroutine();
                yield break;
            }

            var timer = _timeToHide;
            while (timer > 0) {
                timer -= Time.deltaTime;

                _canvasGroup.alpha = Mathf.Lerp(0, 1, timer / _timeToHide);

                yield return null;
            }

            _canvasGroup.alpha = 0;
        }

        private void StopCoroutine() {
            if (_currentCoroutine != null) {
                StopCoroutine(_currentCoroutine);
            }
        }

        private IEnumerator Show() {
            if (_canvasGroup.alpha >= 1) {
                StopCoroutine();
                _currentCoroutine = StartCoroutine(Hide());
                yield break;
            }

            var timer = _timeToShow;
            while (timer > 0) {
                timer -= Time.deltaTime;

                _canvasGroup.alpha = Mathf.Lerp(1, 0, timer / _timeToShow);

                yield return null;
            }

            _canvasGroup.alpha = 1;

            yield return new WaitForSeconds(_timeBetween);
            _currentCoroutine = StartCoroutine(Hide());
        }
    }
}