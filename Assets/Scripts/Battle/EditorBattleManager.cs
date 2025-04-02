using System.Collections.Generic;
using Enemies;
using UnityEngine;

namespace Battle {
    public class EditorBattleManager : MonoBehaviour {
        private List<EnemyStateMachine> _allEnemies = new();

        private int _currentEnemies = 0;
        private Portal.EditorPortal _portal;

        /// <summary>
        /// Called from EditorTool when an object is placed on the grid
        /// </summary>
        /// <param name="placedObject">The Object that was placed</param>
        public void OnObjectPlaced(GameObject placedObject) {
            if (placedObject.TryGetComponent<EnemyStateMachine>(out var enemy)) {
                enemy.CharacterHealth.OnDie += HandleEnemyDead;
                _allEnemies.Add(enemy);
                return;
            }

            if (placedObject.TryGetComponent(out _portal)) {
                _portal.gameObject.SetActive(false);
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
                _portal.gameObject.SetActive(true);
            }
        }
    }
}