using System;
using Camera;
using Extensions;
using Interactable;
using Player;
using Sounds;
using UI;
using UnityEngine;

namespace Modifiers.Skills {
    public class SkillHolder : ModifierHolder<SkillConfig> {
        public override event Action<int> OnModifierSelected = delegate { };

        public override void CancelInteraction() { }

        public override void DoInteract() {
            var player = FindFirstObjectByType<PlayerStateMachine>();
            if (player != null) {
                HandleTargetDetected(player.transform);
            }
        }

        /// <summary>
        /// Called from the Editor as UnityEvent on children.
        /// </summary>
        /// <param name="target"></param>
        protected override void HandleTargetDetected(Transform target) {
            if (target.TryGetComponent<CharacterSkills>(out var characterPowerup)) {
                // at least one
                var powerupPerformed = false;
                foreach (var config in _itemsConfigs) {
                    if (characterPowerup.EquipSkill(config, target)) {
                        if (!powerupPerformed) {
                            powerupPerformed = true;
                        }

                        PlayerEquippedModifiers.Instance.EquipModifier(config);
                    }
                }

                if (powerupPerformed) {
                    SoundManager.instance.Play("coin");
                    OnModifierSelected?.Invoke(GetHashCode());

                    CinemachineCameraHighlight.Instance.HighlightInBounds(target);
                    if (_container != null) {
                        Destroy(_container.gameObject);
                    }

                    if (gameObject.FindInChildren<ItemInteractTag>(out var tag)) {
                        Destroy(tag.gameObject);
                    }
                }
            }
        }
    }
}