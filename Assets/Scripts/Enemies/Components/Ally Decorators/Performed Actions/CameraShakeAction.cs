using Camera;
using UnityEngine;

namespace Enemies.Components.Actions {
    public class CameraShakeAction : MonoBehaviour {
        private IAllyActionDecorator _actionDecorator;

        private void Awake() {
            _actionDecorator = GetComponent<IAllyActionDecorator>();
        }

        private void OnEnable() {
            _actionDecorator.OnPerformed += DoAction;
        }

        private void OnDisable() {
            _actionDecorator.OnPerformed -= DoAction;
        }

        private void DoAction(Transform actor, Transform target) {
            CinemachineCameraShake.Instance.ShakeCamera(target, 12f, 2f, true);
        }
    }
}