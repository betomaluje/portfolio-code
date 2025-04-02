using Base;
using Base.MovementPrediction;
using BerserkPixel.Health;
using BerserkPixel.StateMachine;
using Detection;
using Extensions;
using Player;
using UnityEngine;
using Utils;

namespace Enemies.States {
    [CreateAssetMenu(menuName = "Aurora/Enemy/States/Attack")]
    public class EnemyAttackState : State<EnemyStateMachine> {
        [SerializeField]
        private AttackConfig _attackConfig;

        [SerializeField]
        private PredictionConfig _predictionConfig;

        private float _elapsedTime;
        private TargetDetection _enemyDetection;
        private Vector2 _originalAttackPosition;
        private Transform _target;
        private MovementPredictor _movementPredictor;

        public override void Enter(EnemyStateMachine parent) {
            base.Enter(parent);
            if (_enemyDetection == null) {
                _enemyDetection = this.GetEnemy<State<EnemyStateMachine>, EnemyStateMachine>();
            }

            _target = _enemyDetection?.Target;
            if (_target == null) {
                _machine.SetState(typeof(EnemyIdleState));
                return;
            }

            if (_movementPredictor == null && _predictionConfig != null && _enemyDetection.Target != null) {
                _movementPredictor = new MovementPredictor(_enemyDetection.Target, parent.transform, _predictionConfig.Chance);
            }

            if (parent.WeaponManager.CanAttack()) {
                Vector3 direction;
                if (_movementPredictor != null) {
                    direction = _movementPredictor.PredictTargetDirection();
                }
                else {
                    direction = (_enemyDetection.GetTargetPosition - parent.transform.position).normalized;
                }

                _originalAttackPosition = parent.AttackCollider.transform.position;

                parent.AttackCollider.transform.position = parent.transform.position + direction * parent.WeaponManager.Weapon.Range;

                parent.WeaponManager.ChangeWeapon(Random.Range(0, parent.WeaponManager.TotalWeapons));
                parent.WeaponManager.Attack(direction);

                parent.Movement.FlipSprite(direction);
            }

            parent.MakeBodyDynamic();
            parent.Movement.Stop();

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

            Vector3 dir;
            if (_movementPredictor != null) {
                dir = _movementPredictor.PredictTargetDirection();
            }
            else {
                dir = (hit.transform.position - _machine.transform.position).normalized;
            }

            _machine.Movement.FlipSprite(dir);

            var damageMultiplier = 0f;

            if (hit.TryGetComponent(out PlayerDefenseController defenseController)) {
                damageMultiplier = defenseController.ReducedDamageMultiplier;
            }

            var hitData = new HitDataBuilder()
                .WithWeapon(_machine.WeaponManager.Weapon)
                .WithReduceDamageMultiplier(damageMultiplier)
                .WithCriticalHitChance(_attackConfig.CritChance)
                .WithCriticalHitMultiplier(_attackConfig.CritMultiplier)
                .WithDirection(dir)
                .Build(_machine.transform, hit.gameObject.transform);

            hitData.PerformDamage(hit);
        }

        public override void ChangeState() {
            // check if it's already attacking or the cooldown hasn't finish
            if (_elapsedTime < _machine.WeaponManager.Weapon.AttackCooldown) {
                return;
            }

            if (_target == null) {
                _machine.SetState(typeof(EnemyIdleState));
                return;
            }

            // if player is far, go to chase
            var distance = Vector3.Distance(_machine.transform.position, _target.position);
            if (distance > _machine.WeaponManager.Weapon.Range) {
                _machine.SetState(typeof(EnemyChaseState));
                return;
            }

            // if the player is still near, attack
            if (_enemyDetection.IsPlayerNear) {
                _machine.SetState(typeof(EnemyAttackState));
            }
            else {
                _machine.SetState(typeof(EnemyIdleState));
            }
        }

        public override void Exit() {
            base.Exit();
            _machine.AttackCollider.transform.position = _originalAttackPosition;
        }
    }
}