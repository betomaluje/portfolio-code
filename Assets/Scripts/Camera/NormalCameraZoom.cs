using UnityEngine;

namespace CameraUtils {
    public class NormalCameraZoom : MonoBehaviour {
        [SerializeField]
        private UnityEngine.Camera _mainCamera;

        [SerializeField]
        private float _zoomSpeed = 2f;

        [SerializeField]
        private float _minZoom = 5;

        [SerializeField]
        private float _maxZoom = 20;

        private float _targetZoom;

        private void Awake() {
            _targetZoom = _mainCamera.orthographicSize;
        }

        private void LateUpdate() {
            _mainCamera.orthographicSize = Mathf.Lerp(
              _mainCamera.orthographicSize,
              _targetZoom,
              _zoomSpeed * Time.deltaTime);

        }

        public void ZoomIn() {
            _targetZoom = _minZoom;
        }

        public void ZoomOut() {
            _targetZoom = _maxZoom;
        }
    }
}