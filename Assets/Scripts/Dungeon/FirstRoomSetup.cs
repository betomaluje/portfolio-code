using System;
using Player.Input;
using Modifiers.Powerups;
using Modifiers.Skills;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using Modifiers;
using Modifiers.Merchant;

namespace Dungeon {
    /// <summary>
    /// Sets up elements to spawn in the first room. Useful to place new temporary weapons or powerups
    /// </summary>
    [CreateAssetMenu(fileName = "FirstRoomSetup", menuName = "Dungeon/First Room Setup")]
    public class FirstRoomSetup : ScriptableObject {
        [Tooltip("Player to spawn in the first room")]
        public PlayerBattleInput Player;

        [Tooltip("The Modifiers Merchant prefab")]
        public ModifierMerchant Merchant;

        [Range(0f, 2f)]
        public float SpacingBetweenObjects = 1.5f;

        [Range(0f, 5f)]
        public float SpacingForPlayer = 3f;

        [Tooltip("Objects to spawn in the first room")]
        public Transform[] Objects;

        [Tooltip("List of powerups to spawn in the first room")]
        public FirstRoomItemConfig<PowerupConfig, PowerupHolder> Powerups;

        [Tooltip("List of skills to spawn in the first room")]
        public FirstRoomItemConfig<SkillConfig, SkillHolder> Skills;

        [Tooltip("List of weapon modifiers to spawn in the first room")]
        public FirstRoomItemConfig<WeaponModifier, WeaponModifierHolder> WeaponModifiers;

        private SkillConfig[] cachedSkillConfigs;
        private PowerupConfig[] cachedPowerupConfigs;
        private WeaponModifier[] cachedWeaponModifierConfigs;

        private void OnValidate() {
            if (WeaponModifiers != null && WeaponModifiers.Items != null) {
                WeaponModifiers.Amount = Mathf.Clamp(WeaponModifiers.Amount, 0, WeaponModifiers.Items.Length);
            }

            if (Skills != null && Skills.Items != null) {
                Skills.Amount = Mathf.Clamp(Skills.Amount, 0, Skills.Items.Length);
            }

            if (Powerups != null && Powerups.Items != null) {
                Powerups.Amount = Mathf.Clamp(Powerups.Amount, 0, Powerups.Items.Length);
            }
        }

#if UNITY_EDITOR
        private const string c_SkillsDataFolder = "Assets/Data/Modifiers/Skills";
        private const string c_PowerupsDataFolder = "Assets/Data/Modifiers/Powerups";
        private const string c_WeaponsDataFolder = "Assets/Data/Modifiers/Weapon Modifiers";

        [Button("Auto Fill Modifier Lists")]
        private void FetchModifiers() {
            cachedSkillConfigs = GetSkillsList();
            if (Skills != null && cachedSkillConfigs.Length > 0 && (Skills.Items == null || Skills.Items.Length != cachedSkillConfigs.Length)) {
                Skills.Items = cachedSkillConfigs;
            }

            cachedPowerupConfigs = GetPowerupsList();
            if (Powerups != null && cachedPowerupConfigs.Length > 0 && (Powerups.Items == null || Powerups.Items.Length != cachedPowerupConfigs.Length)) {
                Powerups.Items = cachedPowerupConfigs;
            }

            cachedWeaponModifierConfigs = GetWeaponModifierList();
            if (WeaponModifiers != null && cachedWeaponModifierConfigs.Length > 0 && (WeaponModifiers.Items == null || WeaponModifiers.Items.Length != cachedWeaponModifierConfigs.Length)) {
                WeaponModifiers.Items = cachedWeaponModifierConfigs;
            }
        }

        private SkillConfig[] GetSkillsList() {
            List<SkillConfig> cachedList = new();
            var skillConfigsGuids = AssetDatabase.FindAssets("t:SkillConfig", new[] { c_SkillsDataFolder });
            foreach (var guid in skillConfigsGuids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var skillConfig = AssetDatabase.LoadAssetAtPath<SkillConfig>(path);
                if (skillConfig != null) {
                    cachedList.Add(skillConfig);
                }
            }
            return cachedList.ToArray();
        }

        private PowerupConfig[] GetPowerupsList() {
            List<PowerupConfig> cachedList = new();
            var powerupConfigsGuids = AssetDatabase.FindAssets("t:PowerupConfig", new[] { c_PowerupsDataFolder });
            foreach (var guid in powerupConfigsGuids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var powerupConfig = AssetDatabase.LoadAssetAtPath<PowerupConfig>(path);
                if (powerupConfig != null) {
                    cachedList.Add(powerupConfig);
                }
            }
            return cachedList.ToArray();
        }

        private WeaponModifier[] GetWeaponModifierList() {
            List<WeaponModifier> cachedList = new();
            var weaponConfigsGuids = AssetDatabase.FindAssets("t:WeaponModifier", new[] { c_WeaponsDataFolder });
            foreach (var guid in weaponConfigsGuids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var weaponModifier = AssetDatabase.LoadAssetAtPath<WeaponModifier>(path);
                if (weaponModifier != null) {
                    cachedList.Add(weaponModifier);
                }
            }
            return cachedList.ToArray();
        }

#endif
    }

    [Serializable]
    public class FirstRoomItemConfig<U, T> where U : ScriptableObject, IModifier where T : MonoBehaviour {
        public int Amount = 1;

        [Searchable]
        [ListDrawerSettings(ShowFoldout = true, NumberOfItemsPerPage = 6, ShowPaging = true)]
        public U[] Items;
        public T Prefab;
    }
}