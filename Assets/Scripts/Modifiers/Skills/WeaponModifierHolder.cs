using System;
using Camera;
using Extensions;
using Interactable;
using Player;
using Sounds;
using UI;
using UnityEngine;
using Weapons;

namespace Modifiers.Skills {
    public class WeaponModifierHolder : ModifierHolder<WeaponModifier> {
        public override event Action<int> OnModifierSelected = delegate { };

        public override void CancelInteraction() { }

        public override void DoInteract() {
            var player = FindFirstObjectByType<PlayerStateMachine>();
            if (player != null) {
                HandleTargetDetected(player.transform);
            }
        }

        protected override void HandleTargetDetected(Transform target) {
            // only for Player
            if (target.TryGetComponent<WeaponManager>(out var weaponManager)) {
                weaponManager.EquipModifiers(_itemsConfigs);

                PlayerEquippedModifiers.Instance.EquipModifier(_itemsConfigs);

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