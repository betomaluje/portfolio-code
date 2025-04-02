using System;
using System.Collections.Generic;
using System.Linq;
using Base.Swarm;
using BerserkPixel.Health;
using BerserkPixel.StateMachine;
using BerserkPixel.Utils;
using Cysharp.Threading.Tasks;
using Dungeon;
using Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Enemies {
    public class EnemySwarm : Swarm<EnemySwarmConfig> {
        [SerializeField]
        private Transform[] _defaultSpawnPoints;

        [SerializeField]
        private Transform _swarmContainer;

        [SerializeField]
        private bool _spawnOnStart = false;

        [FoldoutGroup("Events")]
        [Space]
        public UnityEvent OnEnemiesDefeated;

        [Tooltip("Triggered when a wave of enemies is defeated. Parameters: current wave, total amount of waves")]
        public event Action<int, int> OnWaveCompleted = delegate { };

        private List<CharacterHealth> _allEnemies = new();
        private int _currentEnemies;
        private int _currentWave;

        private WeightedList<CharacterHealth> _randomObjects;

        private void Awake() {
            _currentWave = _swarmConfig.AmountOfWaves;
            System.Random random = new(DateTime.Now.TimeOfDay.Minutes);
            _randomObjects = new(_swarmConfig.Prefabs, random);

            if (_swarmContainer == null) {
                _swarmContainer = transform;
            }
        }

        private void Start() {
            if (_spawnOnStart) {
                Spawn();
            }
        }

        private void OnDestroy() {
            if (_allEnemies is not { Count: > 0 }) {
                return;
            }

            Cleanup();
        }

        protected override void PopulatePoints() {
            var amount = _swarmConfig.AmountToSpawn;
            if (_defaultSpawnPoints != null && _defaultSpawnPoints.Length > 0) {
                _spawnPoints = _defaultSpawnPoints
                                .Select(p => p.position.ToInt())
                                .ToArray()
                                .SimpleShuffle();
                return;
            }

            _spawnPoints = DungeonPositionsHolder.Instance.GetPointsOutsideFirstRoom(amount);
        }

        protected override void Cleanup() {
            foreach (var enemyHealth in _allEnemies) {
                if (enemyHealth != null) {
                    enemyHealth.OnDie -= HandleEnemyDead;
                }
            }
        }

        protected override async void Spawn() {
            _currentEnemies = _swarmConfig.AmountToSpawn;

            if (destroyCancellationToken.IsCancellationRequested) {
                return;
            }

            PopulatePoints();

            OnSpawnStart?.Invoke();

            var allEnemyTasks = new UniTask[_currentEnemies];

            for (var i = 0; i < _currentEnemies; i++) {
                allEnemyTasks[i] = SpawnEnemy(_spawnPoints[i]);
            }

            await UniTask.WhenAll(allEnemyTasks);

            OnSpawnEnd?.Invoke();
        }

        private async UniTask SpawnEnemy(Vector3 position) {
            try {
                if (_swarmConfig.AppearFxPrefab != null) {
                    Instantiate(_swarmConfig.AppearFxPrefab, position, Quaternion.Euler(_swarmConfig.FxRotation), _swarmContainer);
                }

                await UniTask.Delay((int)(_swarmConfig.TimeForSpawnEnemy * 1000));

                var enemyHealth = Instantiate(_randomObjects.Next(), position, Quaternion.identity);

                enemyHealth.transform.SetParent(_swarmContainer);

                enemyHealth.OnDie += HandleEnemyDead;

                _allEnemies.Add(enemyHealth);
            }
            catch (OperationCanceledException) { }
        }

        private void HandleEnemyDead() {
            _currentEnemies -= 1;

            if (_currentEnemies <= 0) {
                _currentWave -= 1;

                if (_currentWave <= 0) {
                    OnEnemiesDefeated?.Invoke();
                }
                else {
                    OnWaveCompleted?.Invoke(_swarmConfig.AmountOfWaves - _currentWave, _swarmConfig.AmountOfWaves);
                }
            }
        }

        public void ResumeWaves() => Spawn();

        public void AddEnemy(Transform enemyToAdd) {
            if (!enemyToAdd.TryGetComponent(out StateMachine<EnemyStateMachine> stateMachine)) {
                return;
            }

            _currentEnemies++;

            stateMachine.transform.SetParent(_swarmContainer);
            if (stateMachine.TryGetComponent(out CharacterHealth characterHealth)) {
                characterHealth.OnDie += HandleEnemyDead;
                _allEnemies.Add(characterHealth);
            }
        }

        [Button]
        private void KillAllEnemies() {
            foreach (var enemy in _allEnemies) {
                if (enemy != null) {
                    enemy.AutoDestruct();
                }
            }
        }
    }
}