using System;

namespace Modifiers.Skills {
    public interface ICharacterSkills {
        void ActivateSkills();
        public event Action<SkillConfig> OnSkillEquipped;
        public event Action<SkillConfig> OnSkillActivated;
        public event Action<SkillConfig> OnSkillDeactivated;
    }
}