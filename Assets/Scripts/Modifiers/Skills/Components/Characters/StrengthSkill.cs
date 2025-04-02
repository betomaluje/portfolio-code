using UnityEngine;

namespace Modifiers.Skills {
    [CreateAssetMenu(menuName = "Aurora/Skills/Strength Skill")]
    public class StrengthSkill : SkillConfig {
        private bool _conditionsOnActivate;

        public override void Activate(Transform target) {
            base.Activate(target);
            if (_conditionsOnActivate = CheckConditions()) {
                _holder?.WeaponManager?.SetStrengthInfluence(EndValue);
            }
        }

        public override void Deactivate() {
            base.Deactivate();
            if (_conditionsOnActivate) {
                _holder?.WeaponManager?.ResetStrengthInfluence();
            }
        }
    }
}