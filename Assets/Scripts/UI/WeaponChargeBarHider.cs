using System.Collections;
using UnityEngine;
using Weapons;

namespace UI {
    public class WeaponChargeBarHider : MonoBehaviour {
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private float _timeToShow = .5f;

        [SerializeField]
        private float _timeToHide = 1f;

        private Coroutine _currentCoroutine;

        private WeaponManager _weaponManager;

        private void Awake() {
            _weaponManager = GetComponentInParent<WeaponManager>();
        }

        private void Start() {
            _weaponManager.OnWeaponChanged += HandleWeaponChanged;

            _canvasGroup.alpha = 0;
        }

        private void OnDestroy() {
            _weaponManager.OnWeaponChanged -= HandleWeaponChanged;

            StopAllCoroutines();
        }

        private void HandleWeaponChanged(int weaponIndex) {
            IEnumerator coroutine;
            if (!_weaponManager.IsCharging) {
                coroutine = Hide();
            }
            else {
                coroutine = Show();
            }

            StopCoroutine();

            _currentCoroutine = StartCoroutine(coroutine);
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
                yield break;
            }

            var timer = _timeToShow;
            while (timer > 0) {
                timer -= Time.deltaTime;

                _canvasGroup.alpha = Mathf.Lerp(1, 0, timer / _timeToShow);

                yield return null;
            }

            _canvasGroup.alpha = 1;
        }
    }
}