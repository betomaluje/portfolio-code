using BerserkPixel.StateMachine;
using Detection;
using Extensions;
using UnityEngine;

namespace Companions.States {
    [CreateAssetMenu(menuName = "Aurora/Companions/States/Follow")]
    public class CompanionFollowState : BaseCompanionState {
        [SerializeField]
        [Range(0, 20)]
        private float _speed = 10f;

        [SerializeField]
        private float _minChaseDistance = .1f;

        [SerializeField]
        private float _maxChaseDistance = 10f;

        private TargetDetection _playerDetection;
        private Transform _target;
        private float _distance;

        override public void Enter(CompanionStateMachine parent) {
            base.Enter(parent);

            parent.Sounds.PlayFollow();
            parent.Animations.PlayRun();
            parent.MakeBodyDynamic();

            DetectPlayer(parent);
        }

        private void DetectPlayer(CompanionStateMachine parent) {
            if (_playerDetection == null) {
                _playerDetection = this.GetAlly<State<CompanionStateMachine>, CompanionStateMachine>(false);
            }

            if (_playerDetection == null || _playerDetection.Target == null) {
                parent.SetState(typeof(CompanionIdleState));
            }
            else {
                _target = _playerDetection.Target;
                parent.ExpressionManager.SetExpression("Follow");
            }
        }

        public override void Tick(float deltaTime) {
            base.Tick(deltaTime);
            if (_target == null) {
                _machine.SetState(typeof(CompanionIdleState));
                return;
            }

            Vector2 direction = (_target.position - _machine.transform.position).normalized;
            _distance = Vector2.Distance(_machine.transform.position, _target.position);

            if (_distance > _minChaseDistance) {
                _machine.Movement.Move(_speed * direction);
            }

            _machine.Movement.FlipSprite(direction);
        }

        public override void ChangeState() {
            if (_distance >= _maxChaseDistance) {
                _machine.SetState(typeof(CompanionIdleState));
            }
        }

    }
}