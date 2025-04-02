using System;
using System.Collections.Generic;
using BerserkPixel.Health;
using BerserkPixel.StateMachine;
using BerserkPixel.Utils;
using Camera;
using Cysharp.Threading.Tasks;
using Enemies;
using Extensions;
using Player.Input;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Dungeon {
    public class RoomEnemySwarm : MonoBehaviour {
        private readonly List<CharacterHealth> _allEnemies = new();
        private RoomWall _roomWallPrefab;
        private RoomWall _roomWall; // instantiated one
        private EnemySwarmConfig _swarmConfig;
        private WeightedList<CharacterHealth> _randomObjects;
        private int _currentEnemies;
        private Vector3Int[] _spawnPoints;
        private Room _room;
        private bool _hasAlreadySpawned = false;
        private bool _isPlayerInside = false;
        private float _timePlayerInside;
        private const float TimePlayerInside = 1f;

        private Action _parentAction;
        private bool _isWaveCleared = false;
        private Transform _playerBattleInput;

        private void OnDestroy() {
            Cleanup();
        }

        public void Setup(Room room, EnemySwarmConfig config, RoomWall roomWallPrefab, Action onAllEnemiesDefeated = null) {
            _room = room;
            _swarmConfig = config;
            _roomWallPrefab = roomWallPrefab;
            _parentAction = onAllEnemiesDefeated;
            _timePlayerInside = 0;
            _hasAlreadySpawned = false;
            _isPlayerInside = false;

            System.Random random = new(DateTime.Now.TimeOfDay.Minutes);
            _randomObjects = new(_swarmConfig.Prefabs, random);

            _roomWall = Instantiate(_roomWallPrefab, transform);
            _roomWall.gameObject.SetActive(false);

            _playerBattleInput = FindFirstObjectByType<PlayerBattleInput>().transform;
        }

        private void HandleEnemyDead() {
            _currentEnemies -= 1;

            // Triggered when all enemies are defeated
            if (_currentEnemies <= 0) {
                DestroyWalls();
                Cleanup();
                _isWaveCleared = true;
                _parentAction?.Invoke();
                Destroy(this);
            }
        }

        private void DestroyWalls() {
            if (_roomWall != null) {
                _roomWall.FadeOut();
            }
        }

        private async void Spawn() {
            if (gameObject.TryGetComponent(out Collider2D col)) {
                col.enabled = false;
            }

            // push the player towards the center of the room
            if (_playerBattleInput != null) {
                var maxDistance = 2.5f;

                var distance = Vector3.Distance(_playerBattleInput.position, _room.Center.ToVector3());
                if (distance >= maxDistance && _playerBattleInput.TryGetComponent(out Rigidbody2D rb)) {
                    var force = 25f;

                    var direction = (_room.Center.ToVector3() - _playerBattleInput.position).normalized;
                    rb.linearVelocity = direction * force;
                }
            }

            CinemachineCameraShake.Instance.ShakeCameraWithIntensity(transform, 8f);

            _isWaveCleared = false;

            GenerateWallColliders();

            _currentEnemies = _swarmConfig.AmountToSpawn;

            _spawnPoints = _room.AllInnerPositions.SimpleShuffle().Take(_currentEnemies).ToArray();

            var allEnemyTasks = new UniTask[_currentEnemies];

            for (var i = 0; i < _currentEnemies; i++) {
                allEnemyTasks[i] = SpawnEnemy(_spawnPoints[i]);
            }

            await UniTask.WhenAll(allEnemyTasks);
        }

        private async UniTask SpawnEnemy(Vector3 position) {
            if (_swarmConfig.AppearFxPrefab != null) {
                Instantiate(_swarmConfig.AppearFxPrefab, position, Quaternion.Euler(_swarmConfig.FxRotation), transform);
            }

            await UniTask.Delay((int)(_swarmConfig.TimeForSpawnEnemy * 1000));

            var enemyHealth = Instantiate(_randomObjects.Next(), position, Quaternion.identity, transform);

            enemyHealth.OnDie += HandleEnemyDead;

            _allEnemies.Add(enemyHealth);
        }

        private void GenerateWallColliders() {
            _roomWall.SetupCorners(_room);
            _roomWall.gameObject.SetActive(true);
        }

        // Called when reviving an enemy by AllyReviveActionDecorator
        public void AddEnemy(StateMachine<EnemyStateMachine> stateMachine) {
            _currentEnemies++;

            stateMachine.transform.SetParent(transform);
            if (stateMachine.TryGetComponent(out CharacterHealth characterHealth)) {
                characterHealth.OnDie += HandleEnemyDead;
                _allEnemies.Add(characterHealth);
            }
        }

        private void Update() {
            if (!_isPlayerInside || _hasAlreadySpawned) {
                return;
            }

            _timePlayerInside += Time.deltaTime;
            if (_timePlayerInside >= TimePlayerInside) {
                _hasAlreadySpawned = true;
                Spawn();
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (_hasAlreadySpawned) {
                return;
            }

            if (!_isPlayerInside && other.CompareTag("Player")) {
                _isPlayerInside = true;
                _hasAlreadySpawned = false;
                _timePlayerInside = 0f;
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (_hasAlreadySpawned) {
                return;
            }

            if (_isPlayerInside && other.CompareTag("Player")) {
                _isPlayerInside = false;
                _hasAlreadySpawned = false;
                _timePlayerInside = 0f;
            }
        }

        private void Cleanup() {
            if (_allEnemies is not { Count: > 0 }) {
                return;
            }

            foreach (var enemyHealth in _allEnemies) {
                if (enemyHealth != null) {
                    enemyHealth.OnDie -= HandleEnemyDead;
                }
            }

            _allEnemies.Clear();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            if (_room == null) {
                return;
            }

            if (_playerBattleInput != null) {
                Gizmos.color = Color.red;
                Vector3 endPosition = _room.Center.ToVector3();
                var distance = Vector3.Distance(_playerBattleInput.position, endPosition);
                var middlePoint = (_playerBattleInput.position + endPosition) / 2f;

                var distanceText = new GUIContent($"{distance:F2}");
                var style = new GUIStyle(GUI.skin.box) {
                    richText = true,
                    fontSize = 12,
                    normal = {
                        textColor = Color.black
                    }
                };

                Handles.Label(middlePoint, distanceText, style);

                Gizmos.DrawLine(_playerBattleInput.position, endPosition);
            }

            Gizmos.color = _isWaveCleared ? Color.grey : Color.red;
            foreach (var p in _room.AllInnerPositions) {
                Gizmos.DrawWireCube(p.ToVector3(), Vector3.one);
            }
        }
#endif

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