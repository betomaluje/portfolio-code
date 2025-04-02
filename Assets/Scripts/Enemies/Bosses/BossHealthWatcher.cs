using System.Linq;
using BerserkPixel.Health;
using Camera;
using UnityEngine;
using UnityEngine.Events;

namespace Enemies.Bosses {
    public class BossHealthWatcher : MonoBehaviour {
        [SerializeField]
        private CharacterHealth _bossHealth;

        [SerializeField]
        private Transform[] _toDestroy;

        [SerializeField]
        private UnityEvent OnBossDie;

        private void OnEnable() {
            _bossHealth.OnDie += HandleDie;
        }

        private void OnDisable() {
            _bossHealth.OnDie -= HandleDie;
        }

        private void HandleDie() {
            CinemachineCameraShake.Instance.ShakeCamera(transform, 12f, 2f, true);

            // we remove everything
            var aliveEnemies = FindObjectsByType<EnemyStateMachine>(FindObjectsSortMode.None)
                            .Select(enemy => enemy.GetComponent<CharacterHealth>())
                            .Where(health => health != null && !health.IsDead);

            foreach (var enemy in aliveEnemies) {
                enemy.AutoDestruct();
            }

            DestroyObjects();

            OnBossDie?.Invoke();

            enabled = false;
        }

        private void DestroyObjects() {
            if (_toDestroy != null && _toDestroy.Length > 0) {
                _toDestroy.ToList().ForEach(obj => obj.gameObject.SetActive(false));
                _toDestroy = null;
            }
        }
    }
}