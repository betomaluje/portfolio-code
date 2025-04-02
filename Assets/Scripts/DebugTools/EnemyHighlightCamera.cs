using System.Collections.Generic;
using System.Linq;
using BerserkPixel.Health;
using BerserkPixel.StateMachine;
using Cinemachine;
using Enemies;
using UnityEngine;
using Utils;

namespace DebugTools {
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class EnemyHighlightCamera : MonoBehaviour {
        private CinemachineVirtualCamera _camera;
        private bool _isCameraActive;

        private List<Transform> _enemies;
        private int _currentIndex = 0;
        private CountdownTimer _enemyFetchTimer;

        private void Awake() {
            _camera = GetComponent<CinemachineVirtualCamera>();
        }

        private void Start() {
            _camera.enabled = false;
            _enemyFetchTimer = new CountdownTimer(2f);
            _enemyFetchTimer.OnTimerStop += GetEnemies;
            _enemyFetchTimer.Start();
        }

        private void OnDestroy() {
            _enemyFetchTimer.OnTimerStop -= GetEnemies;
        }

        private void GetEnemies() {
            _enemies = FindObjectsByType<StateMachine<EnemyStateMachine>>(FindObjectsSortMode.None)
                    .Where(x => x.TryGetComponent<CharacterHealth>(out var health) && !health.IsDead)
                    .Select(x => x.transform)
                    .ToList();

            if (_enemies.Count == 0) {
                _enemyFetchTimer.Reset();
                _enemyFetchTimer.Start();
            }
        }

        private void Update() {
            _enemyFetchTimer.Tick(Time.deltaTime);

            if (_enemies == null || _enemies.Count == 0) {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                _isCameraActive = !_isCameraActive;

                _camera.enabled = _isCameraActive;

                if (_isCameraActive) {
                    ShowNextEnemy();
                }
            }

            if (!_isCameraActive) {
                return;
            }

            if (Input.GetKeyDown(KeyCode.M)) {
                ShowNextEnemy();
            }

            if (Input.GetKeyDown(KeyCode.N)) {
                ShowPreviousEnemy();
            }
        }

        private void ShowPreviousEnemy() {
            _currentIndex--;
            if (_currentIndex < 0) {
                _currentIndex = _enemies.Count - 1;
            }

            _camera.Follow = _enemies[_currentIndex];
        }

        private void ShowNextEnemy() {
            _currentIndex = (_currentIndex + 1) % _enemies.Count;

            _camera.Follow = _enemies[_currentIndex];
        }
    }
}