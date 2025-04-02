using System;
using System.Collections.Generic;
using System.Linq;
using Stats;
using UnityEngine;
using UnityEngine.Pool;

namespace Modifiers.Powerups {
    /// <summary>
    /// Makes any Character/Entity be able to receive a powerup. Attach this script to make something be able to receive powerups.
    /// It usually affects an Entities stats
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(IStatsModifier))]
    public class CharacterPowerup : MonoBehaviour, ICharacterPowerup {
        // this is to avoid having the same powerup multiple times
        public readonly HashSet<PowerupConfig> _equippedPowerups = new();

        public Queue<PowerupConfig> _queuedPowerups = new();

        public event Action<PowerupConfig> OnPowerupEquipped;
        public event Action<PowerupConfig> OnPowerupActivated;
        public event Action<PowerupConfig> OnPowerupDeactivated;

        private void Update() {
            CheckForQueuedPowerups();
            CheckForActivePowerups();
        }

        /// <summary>
        /// Checks any powerup in the queue and activates it if it meets the conditions
        /// </summary>
        private void CheckForQueuedPowerups() {
            if (_queuedPowerups.Count == 0) {
                return;
            }

            var toRemove = GenericPool<List<PowerupConfig>>.Get();
            foreach (var powerup in _queuedPowerups) {
                if (!powerup.IsCoolingDown() && powerup.CheckConditions()) {
                    // since this script is on the Player, we need to pass the transform of the Player
                    // DebugTools.DebugLog.Log($"Activating from Queue: {powerup.Name}");
                    powerup.Activate(transform);
                    OnPowerupActivated?.Invoke(powerup);
                    toRemove.Add(powerup);
                }

                if (powerup.IsCoolingDown()) {
                    // reset only once
                    powerup.ResetPowerup();
                }
            }

            if (toRemove.Count > 0)
                _queuedPowerups = new Queue<PowerupConfig>(_queuedPowerups.Where(x => !toRemove.Contains(x)));
        }

        private void CheckForActivePowerups() {
            if (_equippedPowerups.Count == 0) {
                return;
            }

            // we need to check all the currently active powerups
            // if there's any Marked for Removal, we deactivate it, then check if it's temporary and remove it from the active list

            var toRemove = new List<PowerupConfig>();
            foreach (var powerup in _equippedPowerups) {
                if (!powerup.IsCurrentlyActive) {
                    continue;
                }

                powerup.Tick(Time.deltaTime);

                if (powerup.MarkedAsDone) {
                    powerup.Deactivate();
                    OnPowerupDeactivated?.Invoke(powerup);
                    if (powerup.ShouldBeRemoved()) {
                        toRemove.Add(powerup);
                    }
                    else {
                        TryEnqueue(powerup);
                    }
                }
            }

            foreach (var powerup in toRemove) {
                RemovePowerup(powerup);
            }
        }

        private void TryEnqueue(PowerupConfig powerup) {
            if (!_queuedPowerups.Contains(powerup)) {
                // DebugTools.DebugLog.Log($"Try Enqueue: {powerup.Name}");
                powerup.ResetPowerup();
                _queuedPowerups.Enqueue(powerup);
            }
        }

        /// <summary>
        /// Entry point for a Character/Entity to use a powerup. It checks if the powerup is allowed and if it is, it activates it
        /// </summary>
        /// <param name="powerup">The PowerupConfig to use</param>
        /// <param name="target">The transform target to apply the poweup to</param>
        /// <returns>True if the powerup was successfully activated</returns>
        public bool DoPowerup(PowerupConfig powerup, Transform target) {
            if (!_equippedPowerups.Contains(powerup)) {
                powerup.Setup(transform);
                _equippedPowerups.Add(powerup);
                OnPowerupEquipped?.Invoke(powerup);

                if (powerup.CheckConditions()) {
                    // DebugTools.DebugLog.Log($"Automatically activating: {powerup.Name}");
                    powerup.Activate(target);
                    OnPowerupActivated?.Invoke(powerup);
                }
                else {
                    TryEnqueue(powerup);
                }

                return true;
            }

            return false;
        }

        public void RemovePowerup(PowerupConfig powerup) {
            if (_equippedPowerups.Contains(powerup)) {
                // DebugTools.DebugLog.Log($"Removing: {powerup.Name}");
                // OnPowerupDeactivated?.Invoke(powerup);
                powerup.Cleanup();
                _equippedPowerups.Remove(powerup);
            }
        }

        private void OnDestroy() {
            foreach (var powerup in _equippedPowerups) {
                powerup.Cleanup();
            }
            _equippedPowerups.Clear();

            foreach (var powerup in _queuedPowerups) {
                powerup.Cleanup();
            }
            _queuedPowerups.Clear();
        }
    }
}