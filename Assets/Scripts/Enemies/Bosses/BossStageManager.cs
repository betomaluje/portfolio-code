using System;
using System.Linq;
using BerserkPixel.Health;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Weapons;

namespace Enemies.Bosses {
    [RequireComponent(typeof(EnemyStateMachine), typeof(CharacterHealth), typeof(IWeaponManager))]
    public class BossStageManager : MonoBehaviour {
        [SerializeField]
        private BossStage[] _bossStages;

        [FoldoutGroup("Events")]
        [SerializeField]
        private UnityEvent<Color> OnStageChanged;

        [FoldoutGroup("Events")]
        [SerializeField]
        private UnityEvent<int> OnStageChangedIndex;

        private EnemyStateMachine _stateMachine;
        private CharacterHealth _health;
        private IWeaponManager _weaponManager;

        private BossStage _currentStage;
        private int _stageIndex;

        private void Awake() {
            _health = GetComponent<CharacterHealth>();
            _stateMachine = GetComponent<EnemyStateMachine>();
            _weaponManager = GetComponent<IWeaponManager>();
        }

        private void Start() {
            _stageIndex = 0;
            _currentStage = _bossStages[_stageIndex];

            OnStageChangedIndex?.Invoke(_stageIndex);
            ChangeBossStage(_currentStage);
        }

        private void OnEnable() {
            _health.OnPercentageChanged += HandleDamage;
            _health.OnDie += HandleDie;
        }

        private void OnDisable() {
            _health.OnPercentageChanged -= HandleDamage;
            _health.OnDie -= HandleDie;
        }

        private void OnValidate() {
            if (_bossStages != null && _bossStages.Length != 0) {
                var currentOrder = _bossStages.OrderByDescending(stage => stage.PercentageToActivate);
                if (!currentOrder.SequenceEqual(_bossStages)) {
                    _bossStages = currentOrder.ToArray();
                }
            }
        }

        private void HandleDie() {
            Destroy(this);
        }

        private void HandleDamage(float percentage) {
            // check if there is any next stage
            if (_stageIndex + 1 >= _bossStages.Length) {
                return;
            }

            var nextStage = _bossStages[_stageIndex + 1];

            if (percentage <= nextStage.PercentageToActivate) {
                _stageIndex++;
                _currentStage = _bossStages[_stageIndex];

                OnStageChangedIndex?.Invoke(_stageIndex);
                ChangeBossStage(_currentStage);
            }
        }

        private void ChangeBossStage(BossStage stage) {
            if (stage.States != null && stage.States.Count != 0) {
                _stateMachine.SetStates(stage.States);
            }

            if (stage.Weapons != null && stage.Weapons.Count != 0) {
                _weaponManager.Clear();
                _weaponManager.EquipAll(stage.Weapons);
            }

            OnStageChanged?.Invoke(stage.HealthBarColor);
        }
    }
}