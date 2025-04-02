using Base;
using BerserkPixel.Health;
using BerserkPixel.StateMachine;
using Detection;
using Enemies.States;
using UnityEngine;
using Utils;

namespace Enemies.Bosses {
    [CreateAssetMenu(menuName = "Aurora/Enemy/States/Random Attack")]
    public class RandomAttackState : State<EnemyStateMachine> {
        [SerializeField]
        private AttackConfig _attackConfig;

        private TargetDetection _enemyDetection;
        private Vector2 _originalAttackPosition;
        private bool _isAttacking;
        private float _elapsedTime;

        public override void Enter(EnemyStateMachine parent) {
            base.Enter(parent);

            if (_enemyDetection == null) {
                _enemyDetection = GetComponentInChildren<TargetDetection>();
            }

            parent.MakeBodyDynamic();

            var direction = (_enemyDetection.GetTargetPosition - parent.transform.position).normalized;

            _originalAttackPosition = parent.AttackCollider.transform.position;

            parent.AttackCollider.transform.position = parent.transform.position + direction * parent.WeaponManager.Weapon.Range;

            parent.Movement.Stop();
            parent.WeaponManager.ChangeWeapon(Random.Range(0, parent.WeaponManager.TotalWeapons));
            parent.WeaponManager.Attack(direction);

            parent.Movement.FlipSprite(direction);
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
                _isAttacking = false;
                return;
            }

            var dir = (hit.transform.position - _machine.transform.position).normalized;
            var hitData = new HitDataBuilder()
                .WithWeapon(_machine.WeaponManager.Weapon)
                .WithDamage(_machine.WeaponManager.Weapon.GetDamage())
                .WithDirection(dir)
                .Build(_machine.transform, hit.gameObject.transform);
            hitData.PerformDamage(hit);

            _isAttacking = false;
        }

        public override void ChangeState() {
            // check if it's already attacking or the cooldown hasn't finish
            if (_isAttacking || _elapsedTime < _machine.WeaponManager.Weapon.AttackCooldown) {
                return;
            }

            _machine.SetState(typeof(EnemyIdleState));
        }

        public override void Exit() {
            base.Exit();
            _machine.AttackCollider.transform.position = _originalAttackPosition;
        }
    }
}