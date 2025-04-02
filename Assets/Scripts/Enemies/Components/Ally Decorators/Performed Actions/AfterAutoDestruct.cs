using BerserkPixel.Health;
using UnityEngine;

namespace Enemies.Components.Actions {
    public class AfterAutoDestruct : MonoBehaviour {
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
            if (actor.TryGetComponent<CharacterHealth>(out var characterHealth)) {
                // we change into dead state
                characterHealth.AutoDestruct();
            }
        }
    }
}