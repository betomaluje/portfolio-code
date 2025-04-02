using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Enemies.Bosses {
    public class BossSwarmEnemies : MonoBehaviour {
        [SerializeField]
        private EnemySwarm[] _swarmEnemies;

        private EnemySwarm _currentEnemySwarm;

        private int _delay = 4000;

        public async void OnBossStageChanged(int stage) {
            if (_currentEnemySwarm != null) {
                _currentEnemySwarm.OnWaveCompleted -= ResumeWaves;
                _currentEnemySwarm.gameObject.SetActive(false);
            }

            if (stage < _swarmEnemies.Length) {
                _currentEnemySwarm = _swarmEnemies[stage];
                _currentEnemySwarm.gameObject.SetActive(true);
                _currentEnemySwarm.OnWaveCompleted += ResumeWaves;

                await UniTask.Delay(_delay);
                _currentEnemySwarm.ResumeWaves();
                _delay -= 1000;
            }
        }

        public void ResumeWaves(int wave, int totalWaves) {
            _currentEnemySwarm.ResumeWaves();
        }

    }
}