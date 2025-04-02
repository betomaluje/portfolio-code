using Camera;
using EditorTool;
using Interactable;
using Sounds;
using UnityEngine;

namespace Portal {
    public class EditorPortal : MonoBehaviour, IInteract {
        [SerializeField]
        private bool _shakeOnAppear = true;

		private void OnEnable() {
            if (_shakeOnAppear) {
                CinemachineCameraShake.Instance.ShakeCamera(transform);
            }
        }

        public void DoInteract() {
            SoundManager.instance.Play("build_place");

            EditorStateManager.Instance.Unload();
        }

        public void CancelInteraction() { }
    }
}