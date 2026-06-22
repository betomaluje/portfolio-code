using System.Collections.Generic;
using Modifiers.Skills;
using UnityEngine;

namespace BerserkPixel.Health {
    /// <summary>
    /// Performs any WeaponModifier on this gameObject. Usually used in enemies.
    /// (Added automatically at EnemyStateMachine Awake method)
    /// </summary>
    public class CharacterHealthModifiers : MonoBehaviour, IHealth {
        public readonly List<WeaponModifier> _activeModifiers = new();

        public bool CanGiveHealth() => false;

        public void GiveHealth(int health) { }

        private void Update() {
            if (_activeModifiers.Count == 0) {
                return;
            }

            List<WeaponModifier> modifiersToRemove = new();
            foreach (var modifier in _activeModifiers) {
                modifier.Tick(Time.deltaTime);

                if (!modifier.MarkedForRemoval) {
                    continue;
                }

                modifier.Deactivate();
                modifiersToRemove.Add(modifier);
            }

            foreach (var modifier in modifiersToRemove) {
                _activeModifiers.Remove(modifier);
            }
        }

        private void OnDestroy() {
            List<WeaponModifier> modifiersToRemove = new();
            foreach (var modifier in _activeModifiers) {
                if (!modifier.MarkedForRemoval) {
                    continue;
                }

                modifier.Deactivate();
                modifiersToRemove.Add(modifier);
            }

            foreach (var modifier in modifiersToRemove) {
                _activeModifiers.Remove(modifier);
            }
        }

        public void PerformDamage(HitData hitData) {
            if (hitData.weapon == null) {
                return;
            }

            var configs = hitData.weapon.Modifiers;
            if (configs == null || configs.Count == 0) {
                return;
            }

            foreach (var config in configs) {
                if (!_activeModifiers.Contains(config)) {
                    config.Setup(hitData.victim);
                    config.Activate(hitData.victim);
                    _activeModifiers.Add(config);
                }
            }
        }
    }
}