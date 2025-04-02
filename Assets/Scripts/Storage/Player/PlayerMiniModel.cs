using System;
using System.Collections.Generic;
using UnityEngine;

namespace Storage.Player {
    [Serializable]
    public struct PlayerMiniModel {
        public int MaxHealth;
        public int Money;
        public int Level;
        public int Experience;

        public TempStats Stats;
        public int SelectedWeapon;
        public List<string> Weapons;
        public List<CompanionModel> Companions;

        [Serializable]
        public struct CompanionModel {
            public string Name;
            public List<string> Colors;
        }

        [Serializable]
        /// <summary>
        /// This are the temporary stats for each run
        /// </summary>
        public struct TempStats {
            // what is the player's extra health
            // Range [0 - Infinity]
            public int ExtraHealth;

            // modifies the movement speed. Use numbers higher than 1.0
            // Range [1.0 - Infinity]
            [Min(1f)]
            public float SpeedMultiplier;

            // modifies the attack damage
            // Range [1.0 - Infinity]
            [Min(1f)]
            public float AttackMultiplier;

            // modifies the defense
            [Range(0f, 1f)]
            public float DefenseMultiplier;

            // how much auto heals per second
            public int HealRate;

            // chance of performing acritical hit
            [Range(0f, 1f)]
            public float CriticalHitChance;

            // modifies the critical hit damage
            [Min(1f)]
            public float CriticalHitMultiplier;
        }
    }
}