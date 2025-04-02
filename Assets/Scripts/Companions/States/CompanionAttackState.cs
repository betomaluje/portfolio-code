using Base;
using BerserkPixel.Health;
using BerserkPixel.StateMachine;
using Detection;
using Extensions;
using UnityEngine;
using Utils;

namespace Companions.States {
    [CreateAssetMenu(menuName = "Aurora/Companions/States/Attack")]
    public class CompanionAttackState : State<CompanionStateMachine> {
        [SerializeField]
        private AttackConfig _attackConfig;

        private TargetDetection _enemyDetection;
        private Transform _target;
        private float _elapsedTime;

        public override void Enter(CompanionStateMachine parent) {
            base.Enter(parent);

            _enemyDetection = this.GetEnemy<State<CompanionStateMachine>, CompanionStateMachine>();

            if (_enemyDetection == null) {
                parent.SetState(typeof(CompanionFollowState));
                return;
            }

            _target = _enemyDetection.Target;

            var direction = (_target.position - parent.transform.position).normalized;
            parent.AttackCollider.transform.parent.RotateTo(parent.Movement.LastX * direction);
            parent.Animations.PlayAttack();
            parent.MakeBodyDynamic();

            _elapsedTime = 0f;
        }

        public override void Tick(float deltaTime) => _elapsedTime += deltaTime;

        public override void AnimationTriggerEvent(AnimationTriggerType triggerType) {
            base.AnimationTriggerEvent(triggerType);
            if (triggerType == AnimationTriggerType.HitBox) {
                DetectAttack();
            }
        }

        private void DetectAttack() {
            var hit = _machine.AttackCollider.Detect(_attackConfig.TargetMask);

            if (!hit) {
                return;
            }

            var dir = (hit.transform.position - _machine.transform.position).normalized;

            var hitData = new HitDataBuilder()
                .WithWeapon(_machine.WeaponManager.Weapon)
                .WithDirection(dir)
                .Build(_machine.transform, hit.gameObject.transform);

            hitData.PerformDamage(hit);
        }

        public override void ChangeState() {
            if (_target == null) {
                _machine.SetState(typeof(CompanionFollowState));
                return;
            }

            // check if it's already attacking or the cooldown hasn't finish
            if (_elapsedTime < _machine.WeaponManager.Weapon.AttackCooldown) {
                return;
            }

            var distance = Vector3.Distance(_machine.transform.position, _target.position);
            if (distance > _machine.WeaponManager.Weapon.Range) {
                _machine.SetState(typeof(CompanionChaseEnemyState));
                return;
            }

            // if the player is still near, attack
            if (_enemyDetection.IsPlayerNear) {
                _machine.SetState(typeof(CompanionAttackState));
            }
            else {
                _machine.SetState(typeof(CompanionChaseEnemyState));
            }
        }
    }
}