using BerserkPixel.StateMachine;
using Interactable;
using Sounds;
using UnityEngine;
using Utils;

namespace Player.States {
    [CreateAssetMenu(menuName = "Aurora/Player/States/Interact")]
    internal class InteractState : State<PlayerStateMachine> {
        [SerializeField]
        private LayerMask _interactLayerMask;

        [SerializeField]
        private float _interactCooldown = .5f;

        private float _elapsedTime;
        private bool _isInteracting;
        private IInteract _lastInteraction;

        public override void Enter(PlayerStateMachine parent) {
            base.Enter(parent);
            
            parent.Animations.PlayInteract();
            _elapsedTime = 0f;
            _isInteracting = true;
            _lastInteraction = null;
        }

        public override void AnimationTriggerEvent(AnimationTriggerType triggerType) {
            base.AnimationTriggerEvent(triggerType);
            if (triggerType == AnimationTriggerType.Interact) {
                DetectInteract();
            }
        }

        private void DetectInteract() {
            var hit = _machine.AttackCollider.Detect(_interactLayerMask);

            if (!hit) {
                _isInteracting = false;
                _lastInteraction?.CancelInteraction();
                _lastInteraction = null;
                return;
            }

            var tempInteraction = hit.transform.GetComponentInChildren<IInteract>();
            tempInteraction ??= hit.transform.GetComponentInParent<IInteract>();

            if (tempInteraction != null) {
                _lastInteraction = tempInteraction;
                SoundManager.instance.Play("interaction");
                _lastInteraction.DoInteract();
            }

            _isInteracting = false;
        }

        public override void Tick(float deltaTime) => _elapsedTime += deltaTime;

        public override void ChangeState() {
            if (_machine.IsMoving) {
                _machine.SetState(typeof(MoveState));
                return;
            }

            // check if it's already attacking or the cooldown hasn't finish
            if (_isInteracting || _elapsedTime < _interactCooldown) {
                return;
            }

            _machine.SetState(typeof(IdleState));
        }

        public override void Exit() {
            base.Exit();
            _lastInteraction = null;
        }
    }
}