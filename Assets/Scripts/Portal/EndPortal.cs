using Camera;
using NPCs;
using Player;
using UnityEngine;

namespace Portal {
    public class EndPortal : MonoBehaviour {
        [SerializeField]
        [Min(0)]
        private int _moneyPerRescuedNPC = 50;

        private NPCSwarm _swarm;
        private PlayerMoneyManager _playerMoneyManager;

        private void Awake() {
            _swarm = FindFirstObjectByType<NPCSwarm>();
            _playerMoneyManager = FindFirstObjectByType<PlayerMoneyManager>();
            if (_swarm == null) {
                Destroy(this);
            }
        }

        private void OnEnable() {
            CinemachineCameraShake.Instance.ShakeCamera(_playerMoneyManager.gameObject.transform, 6f, 2f);
        }
        private void OnDisable() {
            var multiplier = 0;
            if (_swarm != null) {
                multiplier = _swarm.CurrentNPCRescued;
            }
            var extraNPCmoney = _moneyPerRescuedNPC * multiplier;
            _playerMoneyManager.GiveMoney(extraNPCmoney);
        }
    }
}