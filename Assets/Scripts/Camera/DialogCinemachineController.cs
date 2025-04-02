using BerserkPixel.Prata;
using Cinemachine;
using UnityEngine;

namespace Camera {
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class DialogCinemachineController : MonoBehaviour {
        [SerializeField]
        private float zoomLens = 4;

        [SerializeField]
        private float zoomDuration = 2f;

        [SerializeField]
        private float screenY = -.15f;

        private CinemachineFramingTransposer framingTransposer;

        private float startLens;

        private float startScreenY;
        private float targetLens;
        private float targetScreenY;

        private CinemachineVirtualCamera virtualCamera;

        private void Awake() {
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
            startLens = virtualCamera.m_Lens.OrthographicSize;
            targetLens = startLens;

            framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            startScreenY = framingTransposer.m_ScreenY;
            targetScreenY = startScreenY;
        }

        private void Start() {
            DialogManager.Instance.OnDialogStart += HandleDialogStart;
            DialogManager.Instance.OnDialogEnds += HandleDialogEnd;
            DialogManager.Instance.OnDialogCancelled += HandleDialogCancelled;
        }

        private void Update() {
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(
                virtualCamera.m_Lens.OrthographicSize,
                targetLens,
                zoomDuration * Time.deltaTime);

            framingTransposer.m_ScreenY = Mathf.Lerp(
                framingTransposer.m_ScreenY,
                targetScreenY,
                zoomDuration * Time.deltaTime);
        }

        private void OnDestroy() {
            DialogManager.Instance.OnDialogStart -= HandleDialogStart;
            DialogManager.Instance.OnDialogEnds -= HandleDialogEnd;
            DialogManager.Instance.OnDialogCancelled -= HandleDialogCancelled;
        }

        private void HandleDialogStart(Interaction lastInteraction) {
            targetLens = zoomLens;
            targetScreenY = screenY;
        }

        private void HandleDialogCancelled() {
            targetLens = startLens;
            targetScreenY = startScreenY;
        }

        private void HandleDialogEnd(Interaction lastInteraction) {
            HandleDialogCancelled();
        }
    }
}