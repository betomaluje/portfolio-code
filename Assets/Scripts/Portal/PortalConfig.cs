using Dungeon;
using Scene_Management;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Portal {
    [CreateAssetMenu(menuName = "Aurora/Portal/PortalConfig", order = 0)]
    public class PortalConfig : ScriptableObject {
        [BoxGroup("General")]
        [Tooltip("The Biome name")]
        [InfoBox("The Biome name", InfoMessageType.Info)]
        public string RealmName;

        [BoxGroup("General")]
        [Tooltip("Where does thie portal lead to?")]
        [InfoBox("Where does thie portal lead to?", InfoMessageType.Info)]
        [Required]
        public SceneField TargetScene;

        [BoxGroup("General")]
        [Tooltip("The minimum amount of wins required to go to Boss Scene")]
        [InfoBox("The minimum amount of wins required to go to Boss Scene", InfoMessageType.Info)]
        public int MinWins = 0;

        [BoxGroup("General")]
        [Tooltip("Boss Scene for this specific Biome")]
        [InfoBox("Boss Scene for this specific Biome", InfoMessageType.Info)]
        public SceneField BossScene;

        [BoxGroup("Requirements")]
        [Tooltip("(Optional) Required Biome/Realm to have completed")]
        [InfoBox("(Optional) Required Biome/Realm to have completed", InfoMessageType.Info)]
        [SerializeField]
        private PortalConfig _requiredBiome;

        [BoxGroup("Requirements")]
        [SerializeField]
        private bool _isDemoLocked = true;

#pragma warning disable CS0414 // suppressed but it's used at the end of this file
        [SerializeField]
        private bool _debug = false;
#pragma warning restore CS0414 // suppressed but it's used at the end of this file

        /// <summary>
        /// Checks if the player has the required number of wins for the portal to be unlocked.
        /// </summary>
        /// <returns>True if the player has the required number of wins, false otherwise.</returns>
        public bool IsThisPortalAvailable() {
            // if it is demo lock, return false
            if (_isDemoLocked) {
                return false;
            }

            // if it has no previous required biome, this one is unlocked
            if (_requiredBiome == null) {
                return true;
            }

            // if we do have a previous required biome, check if the previous biome has been defeated
            return _requiredBiome.HasBeenDefeated();
        }

        /// <summary>
        /// Checks if the Boss Scene has been unlocked by checking wins of the current biome.
        /// </summary>
        /// <returns>True if the Boss Scene has been unlocked, false otherwise.</returns>
        private bool HasBossBeenUnlocked() {
            if (!BossScene) {
                return false;
            }
            return GetCurrentWins() >= MinWins;
        }

        /// <summary>
        /// Checksif this Biome/Realm has been defeated by checking wins of the Boss Scene.
        /// </summary>
        /// <returns></returns>
        public bool HasBeenDefeated() {
            if (!BossScene) {
                DebugTools.DebugLog.Log($"Cannot check if {RealmName} has been defeated because there is no Boss Scene assigned.");
                return GetCurrentWins() >= MinWins;
            }

            var bossPrefsName = DungeonWinsUtils.GetWinsPrefsName(BossScene.SceneName);
            return PlayerPrefs.GetInt(bossPrefsName, 0) > 0;
        }

        public SceneField GetTargetScene() {
            if (HasBeenDefeated()) {
                // already defeated biome, start again :)
                return TargetScene;
            }

            SceneField targetScene;

            if (HasBossBeenUnlocked()) {
                targetScene = BossScene;
            }
            else {
                targetScene = TargetScene;
                PlayerPrefs.SetString("RealmName", RealmName);
            }

            DebugTools.DebugLog.Log($"Going to {targetScene.SceneName}!");

            return targetScene;
        }

        /// <summary>
        ///  Returns the progress of the current biome/realm.
        /// </summary>
        /// <returns>1 if it has been completed, 0 if it has not been started, and a number between 0 and 1 if it is in progress.</returns>
        public float GetProgress() {
            if (HasBeenDefeated()) {
                // Completed
                return 1;
            }

            if (IsThisPortalAvailable()) {
                if (HasBossBeenUnlocked()) {
                    // To Boss Fight!
                    return .9f;
                }
                else {
                    // In progress to boss fight
                    return (float)GetCurrentWins() / MinWins;
                }
            }
            else {
                // Locked
                return 0;
            }
        }

        public int GetCurrentWins() {
            string prefsName = DungeonWinsUtils.GetWinsPrefsName(TargetScene.SceneName);
            return PlayerPrefs.GetInt(prefsName, 0);
        }

        #region Debug
        [ShowIf("_debug")]
        [Button]
        private void ResetWins() {
            PlayerPrefs.SetInt(DungeonWinsUtils.GetWinsPrefsName(TargetScene.SceneName), 0);
        }

        [ShowIf("_debug")]
        [Button]
        private void ResetLosses() {
            PlayerPrefs.SetInt(DungeonWinsUtils.GetLossesPrefsName(TargetScene.SceneName), 0);
        }

        [ShowIf("_debug")]
        [Button]
        private void ResetBossWins() {
            if (BossScene == null) {
                return;
            }
            PlayerPrefs.SetInt(DungeonWinsUtils.GetWinsPrefsName(BossScene.SceneName), 0);
        }

        [ShowIf("_debug")]
        [Button]
        private void ResetBossLosses() {
            if (BossScene == null) {
                return;
            }
            PlayerPrefs.SetInt(DungeonWinsUtils.GetLossesPrefsName(BossScene.SceneName), 0);
        }
        #endregion
    }
}