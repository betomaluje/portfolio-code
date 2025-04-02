using Cinemachine;
using Enemies.Bosses;
using Player.Input;
using UnityEngine;

namespace Level {
    public class BossSceneOrchestrator : MonoBehaviour {
        [SerializeField]
        private PlayerBattleInput _playerInput;

        [SerializeField]
        private BossStageManager _bossStageManager;

        [SerializeField]
        private CinemachineVirtualCamera _virtualCamera;

        [SerializeField]
        private GameObject[] _toActivate;

        private void Awake() {
            _playerInput.enabled = false;
            _bossStageManager.enabled = false;

            foreach (GameObject obj in _toActivate) {
                obj.SetActive(false);
            }
        }

        public void StartBossBattle() {
            _playerInput.enabled = true;
            _bossStageManager.enabled = true;

            _virtualCamera.m_Follow = _playerInput.transform;

            foreach (GameObject obj in _toActivate) {
                obj.SetActive(true);
            }
        }
    }
}