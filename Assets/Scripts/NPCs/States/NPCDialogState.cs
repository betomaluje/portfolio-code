using BerserkPixel.Prata;
using BerserkPixel.StateMachine;
using NPCs.Expressions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NPCs.States {
    [InlineEditor]
    [CreateAssetMenu(menuName = "Aurora/NPC/States/Dialog")]
    internal class NPCDialogState : State<NPCStateMachine> {
        [SerializeField]
        private float _detectionRadius = 3f;

        private Interaction _interaction;
        private bool _playerHasLeft;

        public void SetInteraction(Interaction interaction) {
            if (_interaction == null) {
                _interaction = interaction;

                return;
            }

            if (_interaction != null && !_interaction.Equals(interaction)) {
                _interaction = interaction;
            }
        }

        public void ContinueDialog() {
            if (_interaction == null) {
                return;
            }
            DialogManager.Instance.Talk(_interaction);
        }

        public override void Enter(NPCStateMachine parent) {
            base.Enter(parent);

            DialogManager.Instance.Talk(_interaction);

            parent.MakeBodyKinematic();
            parent.SetExpression(ExpressionType.Talk);
            parent.Animations.PlayIdle();
            parent.Movement.Stop();

            _playerHasLeft = false;

            // face player
            var currentPosition = (Vector2)parent.transform.position;
            var playerPosition = (Vector2)parent.PlayerTransform.position;
            var direction = (playerPosition - currentPosition).normalized;
            parent.Movement.FlipSprite(direction);
        }

        public override void Tick(float deltaTime) {
            base.Tick(deltaTime);
            // overlap circle cast to detect player
            if (_machine.PlayerTransform == null) {
                _playerHasLeft = true;
                return;
            }

            var playerPosition = (Vector2)_machine.PlayerTransform.position;
            var distance = Vector2.Distance(_machine.transform.position, playerPosition);
            if (distance > _detectionRadius) {
                _playerHasLeft = true;
            }
        }

        public override void OnDrawGizmosSelected() {
            base.OnDrawGizmosSelected();
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_machine.transform.position, _detectionRadius);
        }

        public override void ChangeState() {
            if (_playerHasLeft) {
                _machine.CancelInteraction();
                _machine.SetState(_interaction.IsLastDialog() ? typeof(NPCFollowPayerState) : typeof(NPCIdleState));
                return;
            }

            if (_machine.HasBeenRescued) {
                _machine.SetState(typeof(NPCFollowPayerState));
            }
        }
    }
}