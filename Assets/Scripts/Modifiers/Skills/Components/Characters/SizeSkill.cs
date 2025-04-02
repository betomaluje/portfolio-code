using UnityEngine;

namespace Modifiers.Skills {
    [CreateAssetMenu(menuName = "Aurora/Skills/Size Skill")]
    public class SizeSkill : SkillConfig {
        private bool _conditionsOnActivate;

        public override void Activate(Transform target) {
            base.Activate(target);
            if (_conditionsOnActivate = CheckConditions()) {
                _holder?.Movement?.SetScaleInfluence(EndValue);
            }
        }

        public override void Deactivate() {
            base.Deactivate();
            if (_conditionsOnActivate) {
                _holder?.Movement?.ResetScaleInfluence();
            }
        }

    }
}