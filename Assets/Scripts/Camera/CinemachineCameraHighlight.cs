using System.Collections;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Extensions;
using UnityEngine;
using Utils;

namespace Camera {
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CinemachineCameraHighlight : Singleton<CinemachineCameraHighlight> {
        private CinemachineVirtualCamera _highlightCamera;

        protected override void Awake() {
            base.Awake();
            _highlightCamera = GetComponent<CinemachineVirtualCamera>();
        }

        private void Start() {
            DoReset();
        }

        private void OnDisable() {
            StopAllCoroutines();
        }

        public void Highlight(Transform target, float duration = 1f) {
            if (target == null) {
                return;
            }

            _highlightCamera.Follow = target;
            _highlightCamera.enabled = true;

            StopAllCoroutines();
            StartCoroutine(DoResetCoRoutine(duration));
        }

        public async UniTask HighlightAsync(Transform target, float duration = 1f) {
            if (target == null) {
                return;
            }

            _highlightCamera.Follow = target;
            _highlightCamera.enabled = true;

            StopAllCoroutines();
            StartCoroutine(DoResetCoRoutine(duration));

            await UniTask.Delay((int)(duration * 100));
        }

        /// <summary>
        /// Focuses the Camera if the target is visible in the screen
        /// </summary>
        /// <param name="target"></param>
        /// <param name="duration">Duration in seconds</param>
        public void HighlightInBounds(Transform target, float duration = 1f) {
            if (target == null) {
                return;
            }

            if (UnityEngine.Camera.main.IsObjectVisible(target)) {
                _highlightCamera.Follow = target;
                _highlightCamera.enabled = true;
                StopAllCoroutines();
                StartCoroutine(DoResetCoRoutine(duration));
            }
        }

        private IEnumerator DoResetCoRoutine(float duration) {
            yield return new WaitForSeconds(duration);
            DoReset();
        }

        public void DoReset() {
            _highlightCamera.Follow = null;
            _highlightCamera.enabled = false;
        }
    }
}