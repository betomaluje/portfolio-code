using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using Base.Swarm;
using BerserkPixel.Utils;
using Dungeon;
using EditorTool;
using Extensions;
using UnityEngine;

namespace Traps {
    public class TrapSwarm : Swarm<TrapSwarmConfig> {

        private WeightedList<Transform> _randomObjects;

        private void Awake() {
            System.Random random = new(DateTime.Now.TimeOfDay.Minutes);
            _randomObjects = new(_swarmConfig.Prefabs, random);
        }

        private void Start() {
            PopulatePoints();
            Spawn();
        }

        protected override async void Cleanup() {
            await transform.DestroyChildrenAsync();
        }

        protected override void PopulatePoints() {
            var amount = _swarmConfig.AmountToSpawn;
            _spawnPoints = DungeonPositionsHolder.Instance.GetPointsOutsideFirstRoom(amount);
        }

        protected override async void Spawn() {
            await UniTask.Delay((int)(_spawnDelay * 1000));

            OnSpawnStart?.Invoke();

            for (var i = 0; i < _swarmConfig.AmountToSpawn; i++) {
                SpawnTrap(ref _spawnPoints[i]);
            }

            OnSpawnEnd?.Invoke();
        }

        private void SpawnTrap(ref Vector3Int position) {
            var enemyHealth = Instantiate(_randomObjects.Next(), position, Quaternion.identity);
            enemyHealth.transform.SetParent(transform);
        }
    }
}