using System;
using System.Linq;
using Base.Swarm;
using BerserkPixel.Utils;
using Cysharp.Threading.Tasks;
using Dungeon;
using Extensions;
using UnityEngine;

namespace Loot {
    public class LootSwarm : Swarm<LootSwarmConfig> {
        [SerializeField]
        private Transform[] _defaultSpawnPoints;

        [SerializeField]
        private float _timeForSpawnEnemy = 1.5f;

        [Header("Spawn FX")]
        [Space]
        [SerializeField]
        private Transform _appearFxPrefab;

        [SerializeField]
        private Vector3 _fxRotation = new(-90, 0, 0);

        private int _currentLootItems;

        private WeightedList<Transform> _randomObjects;

        private void Awake() {
            System.Random random = new(DateTime.Now.TimeOfDay.Minutes);
            _randomObjects = new(_swarmConfig.Prefabs, random);
            Spawn();
        }

        protected override void Cleanup() { destroyCancellationToken.ThrowIfCancellationRequested(); }

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

        protected override async void Spawn() {
            try {
                _currentLootItems = _swarmConfig.AmountToSpawn;

                await UniTask.Delay((int)(_spawnDelay * 1000), cancellationToken: destroyCancellationToken);

                if (destroyCancellationToken != null && destroyCancellationToken.IsCancellationRequested) {
                    return;
                }

                PopulatePoints();

                OnSpawnStart?.Invoke();

                var allItemsTasks = new UniTask[_currentLootItems];

                for (var i = 0; i < _currentLootItems; i++) {
                    allItemsTasks[i] = SpawnLoot(_spawnPoints[i]);
                }

                await UniTask.WhenAll(allItemsTasks);
                OnSpawnEnd?.Invoke();
            }
            catch (OperationCanceledException) { }
        }

        public async void SpawnMore() {
            int time = 10;
            try {
                await UniTask.Delay(time * 1000, cancellationToken: destroyCancellationToken);
                Spawn();
            }
            catch (OperationCanceledException) { }
        }

        private async UniTask SpawnLoot(Vector3 position) {
            if (_appearFxPrefab != null) {
                Instantiate(_appearFxPrefab, position, Quaternion.Euler(_fxRotation));
            }
            try {
                await UniTask.Delay((int)(_timeForSpawnEnemy * 1000), cancellationToken: destroyCancellationToken);

                var loot = Instantiate(_randomObjects.Next(), position, Quaternion.identity);
                loot.SetParent(transform);
            }
            catch (OperationCanceledException) { }
        }
    }
}