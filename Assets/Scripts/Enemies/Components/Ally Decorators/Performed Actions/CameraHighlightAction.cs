using Camera;
using UnityEngine;

namespace Enemies.Components.Actions {
    public class CameraHighlightAction : MonoBehaviour {
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
            CinemachineCameraHighlight.Instance.HighlightInBounds(target);
        }
    }
}