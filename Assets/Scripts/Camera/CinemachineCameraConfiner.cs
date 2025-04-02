using Cinemachine;
using UnityEngine;
using Utils;

namespace Camera {
    [RequireComponent(typeof(CinemachineVirtualCamera), typeof(CinemachineConfiner2D))]
    public class CinemachineCameraConfiner : Singleton<CinemachineCameraConfiner> {
        private CinemachineVirtualCamera _virtualCamera;
        private CinemachineConfiner2D _cameraConfiner;

        protected override void Awake() {
            base.Awake();
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();
            _cameraConfiner = GetComponent<CinemachineConfiner2D>();

            _cameraConfiner.m_BoundingShape2D = null;
            _cameraConfiner.enabled = false;
        }

        public void Confine(Collider2D collider) {
            if (collider == null || collider == _cameraConfiner.m_BoundingShape2D) {
                return;
            }

            _virtualCamera.m_LookAt = collider.transform;

            _cameraConfiner.m_BoundingShape2D = collider;
            _cameraConfiner.enabled = true;
            _cameraConfiner.InvalidateCache();
        }

        public void DoReset() {
            _cameraConfiner.m_BoundingShape2D = null;
            _cameraConfiner.enabled = false;
            _virtualCamera.m_LookAt = null;
        }
    }
}