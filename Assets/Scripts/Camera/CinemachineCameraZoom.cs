using Cinemachine;
using UnityEngine;

namespace Camera {
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CinemachineCameraZoom : MonoBehaviour {
        [SerializeField]
        private float _zoomSpeed = 2f;

        [SerializeField]
        private float _minZoom = 5;

        [SerializeField]
        private float _maxZoom = 20;

        private CinemachineVirtualCamera virtualCamera;

        private float _targetLens;

        private void Awake() {
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
            _targetLens = virtualCamera.m_Lens.OrthographicSize;
        }

        private void Update() {
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(
                virtualCamera.m_Lens.OrthographicSize,
                _targetLens,
                _zoomSpeed * Time.deltaTime);
        }

        public void ZoomIn() {
            _targetLens = _minZoom;
        }

        public void ZoomOut() {
            _targetLens = _maxZoom;
        }
    }
}