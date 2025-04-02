using BerserkPixel.Health;
using Dungeon;
using Player.Input;
using UnityEngine;

namespace Enemies {
    [RequireComponent(typeof(EnemySwarm))]
    public class EnemyWaveController : MonoBehaviour {
        [SerializeField]
        private SafeZone _safeZonePrefab;

        private EnemySwarm _enemySwarm;
        private Transform _playerTransform;
        private SafeZone _safeZone;

        private void Awake() {
            _enemySwarm = GetComponent<EnemySwarm>();
            _playerTransform = FindFirstObjectByType<PlayerBattleInput>().transform;
        }

        private void OnEnable() => _enemySwarm.OnWaveCompleted += OnWaveCompleted;

        private void OnDisable() => _enemySwarm.OnWaveCompleted -= OnWaveCompleted;

        private void Start() {
            var closestRoomToPlayer = DungeonPositionsHolder.Instance.GetClosestRoomToPoint(_playerTransform.position);
            var room = closestRoomToPlayer.Room;
            closestRoomToPlayer.SetRoomForSafeZone(this);
            if (_playerTransform.TryGetComponent<CharacterHealth>(out var characterHealth)) {
                characterHealth.SetImmune();
            }

            if (_safeZonePrefab != null) {
                _safeZone = Instantiate(_safeZonePrefab, closestRoomToPlayer.transform);
                _safeZone.SetupSafeZone(room, false);
            }
        }

        public void OnWaveCompleted(int wave, int totalWaves) {
            var closestRoomToPlayer = DungeonPositionsHolder.Instance.GetClosestRoomToPoint(_playerTransform.position);
            var room = closestRoomToPlayer.Room;
            closestRoomToPlayer.SetRoomForSafeZone(this);
            if (_playerTransform.TryGetComponent<CharacterHealth>(out var characterHealth)) {
                characterHealth.SetImmune();
            }

            if (_safeZonePrefab != null) {
                _safeZone = Instantiate(_safeZonePrefab, closestRoomToPlayer.transform);
                _safeZone.SetupSafeZone(room);
            }
        }

        public void ResumeWaves() {
            if (_safeZone != null && _safeZone.gameObject.activeInHierarchy) {
                _safeZone.RemoveSafeZone(() => {
                    if (_playerTransform.TryGetComponent<CharacterHealth>(out var characterHealth)) {
                        characterHealth.ResetImmune();
                    }
                    _enemySwarm.ResumeWaves();
                });
            }
        }
    }
}