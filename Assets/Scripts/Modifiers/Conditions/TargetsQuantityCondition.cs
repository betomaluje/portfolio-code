using Player;
using Player.Components;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modifiers.Conditions {
    [CreateAssetMenu(fileName = "Target Quantity Condition", menuName = "Aurora/Modifiers/Conditions/Targets Quantity")]
    [TypeInfoBox("Checks if the player has hit at least a certain number of targets at a time.")]
    public class TargetsQuantityCondition : BaseCondition {
        [SerializeField]
        [Min(1)]
        private int _minTargets = 2;

        private HitComboCounter _comboCounter;

        public override void Setup(Transform owner) {
            if (owner.TryGetComponent<PlayerStateMachine>(out var stateMachine)) {
                _comboCounter = stateMachine.HitComboCounter;
            }
        }

        public override bool Check(float deltaTime) => _comboCounter != null && _comboCounter.TargetsHit >= _minTargets;

        public override void Cleanup() {
            _comboCounter = null;
        }
    }
}