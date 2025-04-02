using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Modifiers.Skills {
    [DisallowMultipleComponent]
    public class CharacterSkills : MonoBehaviour, ICharacterSkills {
        [SerializeField]
        private bool _allowMultipleSkills = false;

        public event Action<SkillConfig> OnSkillEquipped;
        public event Action<SkillConfig> OnSkillActivated;
        public event Action<SkillConfig> OnSkillDeactivated;

        public readonly HashSet<SkillConfig> _activeSkills = new();

        private void Update() {
            if (_activeSkills.Count == 0) {
                return;
            }

            var toRemove = GenericPool<List<SkillConfig>>.Get();
            foreach (var skill in _activeSkills) {
                if (!skill.IsCurrentlyActive) {
                    continue;
                }

                skill.Tick(Time.deltaTime);

                if (skill.MarkedAsDone) {
                    skill.Deactivate();
                    OnSkillDeactivated?.Invoke(skill);
                    if (skill.OneTime) {
                        toRemove.Add(skill);
                    }
                }
            }

            foreach (var skill in toRemove) {
                skill.Cleanup();
                _activeSkills.Remove(skill);
            }
        }

        /// <summary>
        /// Activates all equipped Skills
        /// </summary>
        public void ActivateSkills() {
            foreach (var skill in _activeSkills) {
                if (skill.IsCurrentlyActive) {
                    return;
                }

                // DebugTools.DebugLog.Log($"Activating: {skill.Name}");
                skill.Activate(transform);
                OnSkillActivated?.Invoke(skill);
            }
        }

        /// <summary>
        /// Equips a skill to an Entity.
        /// </summary>
        /// <param name="config">The PowerupConfig to use</param>
        /// <param name="target">The transform target to apply the poweup to</param>
        /// <returns>True if the powerup was successfully equipped</returns>
        public bool EquipSkill(SkillConfig config, Transform target) {
            if (!_allowMultipleSkills) {
                // we only allow 1 Skill at a time
                _activeSkills.Clear();
            }

            if (!_activeSkills.Contains(config)) {
                // since we always want the Target to be the Player, we pass the Transform
                config.Setup(target);
                _activeSkills.Add(config);
                OnSkillEquipped?.Invoke(config);
                return true;
            }
            return false;
        }

        private void OnDestroy() {
            foreach (var skill in _activeSkills) {
                skill.Cleanup();
            }
            _activeSkills.Clear();
        }
    }
}