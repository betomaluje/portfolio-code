using Sirenix.OdinInspector;
using UnityEngine;

namespace Modifiers.Powerups {
    [CreateAssetMenu(menuName = "Aurora/Powerups/Defense Powerup")]
    [TypeInfoBox("Powerup that reduces damage taken by a percentage [0, 1] for a certain amount of time")]
    public class DefensePowerup : PowerupConfig {
        [Tooltip("If the target needs to spawn an object, this is the prefab to use")]
        [SerializeField]
        private Transform _prefabEquipped;

        private Transform _equipped;

        public override void Setup(Transform owner) {
            base.Setup(owner);

            if (_prefabEquipped != null) {
                _equipped = Instantiate(_prefabEquipped, owner);
                _equipped.name = $"{Name} Equipped";
                _equipped.position = Vector3.zero;
            }
        }

        public override void Activate(Transform target) {
            base.Activate(target);
            _playerStatsManager.AddStatModifier(_statType, EndValue);

            if (_equipped != null) {
                _equipped.gameObject.SetActive(true);
            }
        }

        public override void Deactivate() {
            base.Deactivate();
            _playerStatsManager.ResetStatModifier(_statType);

            if (_equipped != null) {
                _equipped.gameObject.SetActive(false);
            }
        }

        private void OnValidate() {
            _statType = Stats.StatType.Defense;

            EndValue = Mathf.Clamp01(EndValue);
        }
    }
}