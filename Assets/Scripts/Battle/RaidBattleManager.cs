using System.Collections.Generic;
using Cinemachine;
using Enemies;
using UnityEngine;
using UnityEngine.Events;

namespace Battle {
    public class RaidBattleManager : MonoBehaviour {
        [SerializeField]
        private CinemachineVirtualCamera _virtualCamera;

        private List<EnemyStateMachine> _allEnemies = new();

        [Header("Events")]
        [Space]
        public UnityEvent OnEnemiesDefeated;

        private int _currentEnemies = 0;

        /// <summary>
        /// Called from EditorTool when an object is placed on the grid
        /// </summary>
        /// <param name="placedObject">The Object that was placed</param>
        public void OnObjectPlaced(GameObject placedObject) {
            if (placedObject.CompareTag("Player")) {
                _virtualCamera.Follow = placedObject.transform;
            }
            else if (placedObject.TryGetComponent<EnemyStateMachine>(out var enemy)) {
                enemy.CharacterHealth.OnDie += HandleEnemyDead;
                _allEnemies.Add(enemy);
            }
        }

        /// <summary>
        /// Called from EditorTool when the grid is fully loaded
        /// </summary>
        public void OnGridLoaded() {
            _currentEnemies = _allEnemies.Count;
        }

        private void OnDestroy() {
            if (_allEnemies is not { Count: > 0 }) {
                return;
            }

            Cleanup();
        }

        private void Cleanup() {
            foreach (var enemy in _allEnemies) {
                enemy.CharacterHealth.OnDie -= HandleEnemyDead;
            }
        }

        private void HandleEnemyDead() {
            _currentEnemies -= 1;

            if (_currentEnemies <= 0) {
                OnEnemiesDefeated?.Invoke();
            }
        }
    }
}