using Base;
using BerserkPixel.Health;
using BerserkPixel.StateMachine;
using Player.Components;
using Sounds;
using Unity.Mathematics;
using UnityEngine;
using Utils;
using Weapons;

namespace Player.States {
    [CreateAssetMenu(menuName = "Aurora/Player/States/Attack")]
    internal class AttackState : State<PlayerStateMachine> {
        [SerializeField]
        private AttackConfig _attackConfig;

        private float _elapsedTime;
        // shortest time between weapon cooldown and animation attack
        private float _longestTime;

        private PlayerAim _playerAim;

        public override void Enter(PlayerStateMachine parent) {
            base.Enter(parent);

            if (parent.WeaponManager.CanAttack()) {
                SoundManager.instance.Play("woosh");
                parent.Animations.PlayAttack(parent.WeaponManager.Weapon.AttackType);
                parent.WeaponManager.Attack();
            }

            // this should always be true since AttackState is only called from Player's StateMachine
            if (parent.WeaponManager is WeaponManager weaponManager) {
                _playerAim = weaponManager.PlayerAim;
                _playerAim.SetTargetMask(_attackConfig.TargetMask);
            }

            _longestTime = math.max(
                parent.WeaponManager.Weapon.AttackCooldown,
                parent.WeaponManager.Animations.GetAnimationLength(parent.WeaponManager.Weapon.AttackType)
            );
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
            var hitCount = _machine.AttackCollider.DetectAll(_attackConfig.TargetMask, out var hits);
            if (hitCount == 0) {
                return;
            }

            // TODO: maybe move this to the hit state on enemy
            SoundManager.instance.Play("hit");

            var luckController = _machine.LuckController;
            var criticalHitChance = luckController.GetCriticalHitChance();
            var criticalHitMultiplier = luckController.GetCriticalHitMultiplier();

            foreach (var hit in hits) {
                if (hit == null) {
                    continue;
                }

                var dir = (hit.transform.position - _machine.transform.position).normalized;

                var hitData = new HitDataBuilder()
                    .WithWeapon(_machine.WeaponManager.Weapon)
                    .WithDamage(_machine.CalculateWeaponDamage())
                    .WithCriticalHitChance(criticalHitChance)
                    .WithCriticalHitMultiplier(criticalHitMultiplier)
                    .WithDirection(dir)
                    .Build(_machine.transform, hit.gameObject.transform);

                var timescaleData = new TimescaleData(hitData.isCritical ? _attackConfig.CritHitTime : _attackConfig.NormalHitTime, 0f);
                hitData.timescaleData = timescaleData;

                hitData.PerformDamage(hit);

                _machine.AddHitData(hitData);
            }

            _machine.AddEnemiesHit(hitCount);
        }

        public override void ChangeState() {
            // if we have a weapon that is aiming, don't change state
            if (_playerAim != null && _playerAim.IsCurrentlyAttacking) {
                return;
            }

            if (_machine.IsMoving) {
                _machine.SetState(typeof(MoveState));
                return;
            }

            // check if the cooldown hasn't finish
            if (_elapsedTime < _longestTime) {
                return;
            }

            _machine.SetState(typeof(IdleState));
        }
    }
}