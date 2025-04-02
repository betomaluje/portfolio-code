using DebugTools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Dungeon {
    public class DungeonWinCounter : Singleton<DungeonWinCounter> {
        private string _winPrefsName;
        private string _lossPrefsName;

        private int _currentWins;
        private int _currentLosses;

        protected override void Awake() {
            base.Awake();
            _winPrefsName = DungeonWinsUtils.GetWinsPrefsName(SceneManager.GetActiveScene().name);
            _lossPrefsName = DungeonWinsUtils.GetLossesPrefsName(SceneManager.GetActiveScene().name);

            _currentWins = PlayerPrefs.GetInt(_winPrefsName, 0);
            _currentLosses = PlayerPrefs.GetInt(_lossPrefsName, 0);
        }

        public void AddWin() {
            _currentWins++;
            PlayerPrefs.SetInt(_winPrefsName, _currentWins);
            DebugLog.Log($"Add Win {_winPrefsName} -> {_currentWins}");
        }

        public void AddLoss() {
            _currentLosses++;
            PlayerPrefs.SetInt(_lossPrefsName, _currentLosses);
        }

        [Button]
        private void ResetWins() {
            _currentWins = 0;
            PlayerPrefs.SetInt(_winPrefsName, _currentWins);
        }

        [Button]
        private void ResetLosses() {
            _currentWins = 0;
            PlayerPrefs.SetInt(_lossPrefsName, _currentLosses);
        }
    }
}