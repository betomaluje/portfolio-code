using System;
using Base;
using BerserkPixel.Health;
using Camera;
using DebugTools;
using Detection;
using Enemies.Components;
using Enemies.States;
using Extensions;
using Modifiers.Powerups;
using UI;
using UnityEngine;
using Weapons;

namespace Enemies {
    public class EnemyStateMachine : CharacterStateMachine<EnemyStateMachine> {
        [SerializeField]
        private TargetDetection _enemyDetection;

        [SerializeField]
        private Component[] _extraDestroyComponents;

        public EnemyExpressionManager ExpressionManager { get; private set; }

        public CharacterHealth CharacterHealth => characterHealth;

        // sends the experience gained for defeating this enemy
        public static Action<int> OnEnemyDefeated = delegate { };

        private EnemyWeaponManager _weaponManager;

        protected override void Awake() {
            base.Awake();
            CloneStates();
            _weaponManager = WeaponManager as EnemyWeaponManager;
            ExpressionManager = GetComponent<EnemyExpressionManager>();
            gameObject.GetOrAdd<CharacterHealthModifiers>();
        }

        private void OnEnable() {
            characterHealth.OnBlock += HandleBlockChance;
            characterHealth.OnDamagePerformed += HandleHurt;
            characterHealth.OnDie += HandleDie;

            _enemyDetection?.OnPlayerDetected.AddListener(HandlePlayerDetected);
        }

        private void OnDisable() {
            characterHealth.OnBlock -= HandleBlockChance;
            characterHealth.OnDamagePerformed -= HandleHurt;
            characterHealth.OnDie -= HandleDie;

            _enemyDetection?.OnPlayerDetected.RemoveListener(HandlePlayerDetected);
        }

        private void HandlePlayerDetected(Transform player) {
            // if we are currently going for an ally, don't change state
            if (CurrentState != null && CurrentState == typeof(EnemyAllyCheckState)) {
                return;
            }

            if (CurrentState != typeof(EnemyDetectState)) {
                SetState(typeof(EnemyDetectState));
            }
        }

        /// <summary>
        /// Called from Editor UnityEvent on MultipleTargetDetection script
        /// </summary>
        public void HandleTargetDetected(Transform target, float distance) {
            _weaponManager.ChangeWeaponByRange(distance);

            // TODO: use some sort of brain here. If target is close attack if target is far, chase
            // see https://youtu.be/S4oyqrsU2WU
        }

        /// <summary>
        /// Called from EnemyAllyCheckState when it's close to the target.
        /// </summary>
        /// <param name="ally"></param>
        /// <returns>True if it was handled by AllyDetection, False otherwise</returns>
        public bool AllyTargetReached(Transform ally) {
            if (ally == null)
                return false;

            if (TryGetComponent<AllyDetection>(out var allyDetection)) {
                allyDetection.OnTargetReached(ally);
                return true;
            }

            return false;
        }

        private void HandleBlockChance() {
            SetState(typeof(EnemyBlockState));
            CinemachineCameraShake.Instance.ShakeCamera(transform);
        }

        private void HandleHurt(HitData hitData) {
            if (CharacterHealth.CurrentHealth > 0) {
                SetState(typeof(EnemyHitState));
            }
            CinemachineCameraShake.Instance.ShakeCamera(transform);
        }

        private void HandleDie() {
            SetState(typeof(EnemyDeadState));
            OnEnemyDefeated?.Invoke(CharacterHealth.ExperienceGained);
            DestroyObjects();
        }

        private void DestroyObjects() {
            if (_enemyDetection != null) {
                Destroy(_enemyDetection.gameObject);
            }

            if (TryGetComponent<HealthBarHider>(out var healthBarHider)) {
                Destroy(healthBarHider.gameObject);
            }

            if (TryGetComponent<AllyDetection>(out var allyDetection)) {
                Destroy(allyDetection);
            }

            if (TryGetComponent<CharacterPowerup>(out var characterPowerup)) {
                Destroy(characterPowerup);
            }

            var extraParticles = GetComponentsInChildren<ParticleSystem>();
            if (extraParticles != null && extraParticles.Length > 0) {
                foreach (var particleSystem in extraParticles) {
                    Destroy(particleSystem.gameObject);
                }
            }

            if (this.FindInChildren<CharacterHealthModifiers>(out var healthModifiers)) {
                Destroy(healthModifiers);
            }

            if (_extraDestroyComponents != null && _extraDestroyComponents.Length > 0) {
                foreach (var obj in _extraDestroyComponents) {
                    if (obj != null)
                        Destroy(obj);
                }
            }
        }

        public void MakeBodyKinematic() {
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        }

        public void MakeBodyDynamic() {
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }

        // Called from EnemyChase ChangeState function
        public void OnTargetReached(TargetType targetType) {
            switch (targetType) {
                case TargetType.Ally:
                    DebugLog.Log("OnTargetReached TargetType Ally");
                    break;

                case TargetType.Enemy:
                    SetState(typeof(EnemyAttackState));
                    break;
            }
        }
    }
}