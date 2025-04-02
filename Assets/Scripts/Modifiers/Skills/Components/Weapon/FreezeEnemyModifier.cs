using UnityEngine;

namespace Modifiers.Skills.Components {
    [CreateAssetMenu(menuName = "Aurora/Skills/Weapons/Freeze Modifier")]
    public class FreezeEnemyModifier : WeaponModifier {
        public override void Activate(Transform target) {
            base.Activate(target);
            _target?.Movement?.SetMovementInfluence(EndValue);
        }

        public override void Deactivate() {
            base.Deactivate();
            _target?.Movement?.ResetMovementInfluence();
        }
    }
}