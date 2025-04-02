using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using static Storage.Player.PlayerMiniModel;

namespace Storage.Player {
    public static class PlayerMiniModelExt {
        private static readonly TempStats DefaultStats = new() {
            ExtraHealth = 0,
            SpeedMultiplier = 1f,
            AttackMultiplier = 1f,
            DefenseMultiplier = 0f,
            HealRate = 0,
            CriticalHitChance = 0.1f,
            CriticalHitMultiplier = 1f
        };

        private static readonly int DefaultHealth = 100;
        private static readonly int DefaultMoney = 100;
        private static readonly int DefaultLevel = 1;
        private static readonly int DefaultExperience = 0;

        private static readonly int DefaultSelectedWeapon = 0;
        private static readonly List<string> DefaultWeapons = new(1) { "Punch" };
        private static readonly List<CompanionModel> DefaultCompanions = new() { };

        private static float Round2Decimals(float value) {
            return Mathf.Round(value * 10.0f) * 0.1f;
        }

        private static TempStats ClampStats(PlayerMiniModel playerMiniModel) {
            var playerStats = playerMiniModel.Stats;
            if (playerStats.Equals(default(TempStats))) {
                playerStats = DefaultStats;
            }

            playerStats.SpeedMultiplier = Round2Decimals(playerStats.SpeedMultiplier);
            playerStats.AttackMultiplier = Round2Decimals(playerStats.AttackMultiplier);
            playerStats.DefenseMultiplier = Round2Decimals(playerStats.DefenseMultiplier);
            playerStats.CriticalHitChance = Round2Decimals(playerStats.CriticalHitChance);
            playerStats.CriticalHitMultiplier = Round2Decimals(playerStats.CriticalHitMultiplier);

            return playerStats;
        }

        public static PlayerMiniModel GetDefault() {
            var miniModel = new PlayerMiniModel {
                MaxHealth = DefaultHealth,
                Money = DefaultMoney,
                Level = DefaultLevel,
                Experience = DefaultExperience,
                Stats = DefaultStats,
                SelectedWeapon = DefaultSelectedWeapon,
                Weapons = DefaultWeapons,
                Companions = DefaultCompanions
            };
            return miniModel;
        }

        public static string ToJson(PlayerMiniModel playerMiniModel) {
            playerMiniModel.Stats = ClampStats(playerMiniModel);

            var savedObjects = JsonConvert.SerializeObject(playerMiniModel);
            return savedObjects;
        }

        public static PlayerMiniModel FromJson(string json) {
            var playerMiniModel = JsonConvert.DeserializeObject<PlayerMiniModel>(json);
            playerMiniModel.Stats = ClampStats(playerMiniModel);
            return Check(playerMiniModel);
        }

        public static PlayerMiniModel Check(PlayerMiniModel miniModel) {
            if (miniModel.MaxHealth <= 0) {
                miniModel.MaxHealth = DefaultHealth;
            }

            if (miniModel.Money <= 0) {
                miniModel.Money = DefaultMoney;
            }

            if (miniModel.Level <= 0) {
                miniModel.Level = DefaultLevel;
            }

            if (miniModel.Experience < 0) {
                miniModel.Experience = DefaultExperience;
            }

            if (miniModel.Weapons == null || miniModel.Weapons.Count == 0) {
                miniModel.Weapons = DefaultWeapons;
            }

            if (miniModel.Stats.Equals(default(TempStats))) {
                miniModel.Stats = DefaultStats;
            }

            if (miniModel.SelectedWeapon < 0 || miniModel.SelectedWeapon > miniModel.Weapons.Count - 1) {
                miniModel.SelectedWeapon = DefaultSelectedWeapon;
            }

            if (miniModel.Companions == null || miniModel.Companions.Count == 0) {
                miniModel.Companions = DefaultCompanions;
            }

            return miniModel;
        }
    }
}