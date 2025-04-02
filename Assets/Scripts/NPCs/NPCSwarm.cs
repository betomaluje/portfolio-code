using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Base.Swarm;
using Dungeon;
using UnityEngine;
using UnityEngine.Events;
using BerserkPixel.Utils;
using System;
using Sirenix.OdinInspector;

namespace NPCs {
    public class NPCSwarm : Swarm<NPCSwarmConfig> {
        [Space]
        [SerializeField]
        private bool _allNPCShouldFinishLevel = false;

        [FoldoutGroup("Events")]
        public UnityEvent<string> OnNPCAmountChange;

        [FoldoutGroup("Events")]
        public UnityEvent OnAllNPCDefeated;

        public int CurrentNPCRescued => _currentRescuedNPCs;

        private List<NPCStateMachine> _allNPCs;
        private WeightedList<NPCStateMachine> _randomObjects;
        private int _currentAvailableNPCs;
        private int _currentRescuedNPCs;

        private void Start() {
            PopulatePoints();

            // we calculate the already existing NPCs on the scene
            var npcsOnScene = FindObjectsByType<NPCStateMachine>(FindObjectsSortMode.None);
            _currentAvailableNPCs = npcsOnScene.Length;
            _allNPCs = new List<NPCStateMachine>();
            System.Random random = new(DateTime.Now.TimeOfDay.Minutes);
            _randomObjects = new(_swarmConfig.Prefabs, random);

            foreach (var npc in npcsOnScene) {
                _allNPCs.Add(npc);
                npc.Health.OnDie += HandleNPCDead;
                npc.OnRescued += HandleRescue;
            }

            OnNPCAmountChange?.Invoke($"{_currentRescuedNPCs}/{_currentAvailableNPCs}");

            Spawn();
        }

        private void OnDestroy() {
            if (_allNPCs is not { Count: > 0 }) {
                return;
            }

            Cleanup();
        }

        protected override void PopulatePoints() {
            var amount = _swarmConfig.AmountToSpawn;

            _spawnPoints = DungeonPositionsHolder.Instance.GetRandomPoints(amount);
        }

        protected override void Cleanup() {
            foreach (var npc in _allNPCs) {
                npc.Health.OnDie -= HandleNPCDead;
                npc.OnRescued -= HandleRescue;
            }
        }

        protected override async void Spawn() {
            await UniTask.Delay((int)(_spawnDelay * 1000));

            OnSpawnStart?.Invoke();

            _currentAvailableNPCs += _swarmConfig.AmountToSpawn;

            OnNPCAmountChange?.Invoke($"{_currentRescuedNPCs}/{_currentAvailableNPCs}");

            for (var i = 0; i < _swarmConfig.AmountToSpawn; i++) {
                var libraryIndex = UnityEngine.Random.Range(0, _swarmConfig.Prefabs.Length);

                var position = _spawnPoints[i];
                var npc = CreateNPC(
                    libraryIndex,
                    transform,
                    position
                );

                _allNPCs.Add(npc);
            }

            OnSpawnEnd?.Invoke();
        }

        private NPCStateMachine CreateNPC(int libraryIndex, Transform parent, Vector3Int position) {
            var npc = Instantiate(_randomObjects.Next(), parent, true);
            npc.name = "NPC";

            if (npc.TryGetComponent(out NPCChangeClothes script)) {
                var spriteLibraryAsset = _swarmConfig.SpriteLibraryAsset[libraryIndex];
                script.ChangeSprites(spriteLibraryAsset);
            }

            npc.transform.position = position;

            npc.Health.OnDie += HandleNPCDead;
            npc.OnRescued += HandleRescue;

            return npc;
        }

        private void HandleRescue(bool rescued) {
            if (rescued) {
                _currentRescuedNPCs += 1;
                _currentRescuedNPCs = Mathf.Min(_currentAvailableNPCs, _currentRescuedNPCs);
            }
            else {
                _currentRescuedNPCs -= 1;
                _currentRescuedNPCs = Mathf.Max(0, _currentRescuedNPCs);
            }

            OnNPCAmountChange?.Invoke($"{_currentRescuedNPCs}/{_currentAvailableNPCs}");
        }

        private void HandleNPCDead() {
            _currentAvailableNPCs -= 1;

            _currentAvailableNPCs = Mathf.Max(0, _currentAvailableNPCs);

            OnNPCAmountChange?.Invoke($"{_currentRescuedNPCs}/{_currentAvailableNPCs}");

            if (_allNPCShouldFinishLevel && _currentAvailableNPCs <= 0) {
                OnAllNPCDefeated?.Invoke();
            }
        }
    }
}