using UnityEngine;

namespace Modifiers.Skills {
    public static class SkillFactory {
        public static SkillConfig CreateSkill(SkillType type) {
            return type switch {
                SkillType.Health => ScriptableObject.CreateInstance<HealthSkill>(),
                SkillType.Immunity => ScriptableObject.CreateInstance<ImmunitySkill>(),
                SkillType.OnHit => ScriptableObject.CreateInstance<OnHitSkill>(),
                SkillType.Size => ScriptableObject.CreateInstance<SizeSkill>(),
                SkillType.SpawnObject => ScriptableObject.CreateInstance<SpawnObjectSkill>(),
                SkillType.Strength => ScriptableObject.CreateInstance<StrengthSkill>(),
                SkillType.BigAttack => ScriptableObject.CreateInstance<BigAttackSkill>(),
                SkillType.TimeWarp => ScriptableObject.CreateInstance<TimeWarpSkill>(),
                SkillType.DetectAndSpawn => ScriptableObject.CreateInstance<DetectAndSpawnSkill>(),
                _ => null,
            };
        }
    }
}