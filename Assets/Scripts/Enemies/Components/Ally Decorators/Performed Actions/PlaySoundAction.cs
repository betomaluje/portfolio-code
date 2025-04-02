using Sounds;
using UnityEngine;

namespace Enemies.Components.Actions {
    public class PlaySoundAction : MonoBehaviour {
        [SerializeField]
        private string _soundName;

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
            if (string.IsNullOrEmpty(_soundName)) {
                return;
            }

            SoundManager.instance.PlayOnSpot(_soundName, actor.position);
        }
    }
}