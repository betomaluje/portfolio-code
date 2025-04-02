using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Modifiers.Conditions {
    public class ConditionFactory {
        private const string c_ConditionsDataFolder = "Assets/Data/Modifiers/Conditions";

        private static readonly Dictionary<ConditionType, BaseCondition> conditionNames = new();

        public ConditionFactory() {
            GetConditionsInDisk();
        }

        private static void GetConditionsInDisk() {
#if UNITY_EDITOR
            // Load all BaseCondition ScriptableObjects from a specific folder
            string[] guids = AssetDatabase.FindAssets("t:BaseCondition", new[] { c_ConditionsDataFolder });
            foreach (string guid in guids) {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                BaseCondition condition = AssetDatabase.LoadAssetAtPath<BaseCondition>(assetPath);

                if (condition != null && Enum.TryParse(condition.GetType().Name.Replace("Condition", ""), out ConditionType conditionType)) {
                    conditionNames[conditionType] = condition;
                }
                else {
                    Debug.LogWarning($"Could not map {assetPath} to a ConditionType enum.");
                }
            }
#else
        Debug.LogError("ConditionFactory only works in the editor to initialize the dictionary because it relies on AssetDatabase.");
#endif
        }

        public BaseCondition GetOrCreateCondition(ConditionType type) {
            if (conditionNames.TryGetValue(type, out BaseCondition condition)) {
                return condition;
            }

            return type switch {
                ConditionType.CollectMoney => ScriptableObject.CreateInstance<CollectMoneyCondition>(),
                ConditionType.DoesNotMove => ScriptableObject.CreateInstance<DoesNotMoveCondition>(),
                ConditionType.HitCounter => ScriptableObject.CreateInstance<HitCounterCondition>(),
                ConditionType.LosePercentOnHit => ScriptableObject.CreateInstance<LosePercentOnHitCondition>(),
                ConditionType.MaxHealth => ScriptableObject.CreateInstance<MaxHealthCondition>(),
                ConditionType.MinHealth => ScriptableObject.CreateInstance<MinHealthCondition>(),
                ConditionType.OnCriticalHit => ScriptableObject.CreateInstance<OnCriticalHitCondition>(),
                ConditionType.OnPlayerHeal => ScriptableObject.CreateInstance<OnPlayerHealCondition>(),
                ConditionType.PlayerRoll => ScriptableObject.CreateInstance<PlayerRollCondition>(),
                ConditionType.ReceiveDamage => ScriptableObject.CreateInstance<ReceiveDamageCondition>(),
                ConditionType.SurroundedBy => ScriptableObject.CreateInstance<SurroundedByCondition>(),
                ConditionType.TargetsQuantity => ScriptableObject.CreateInstance<TargetsQuantityCondition>(),
                _ => null,
            };
        }
    }

    [Serializable]
    public enum ConditionType {
        CollectMoney,
        DoesNotMove,
        HitCounter,
        LosePercentOnHit,
        MaxHealth,
        MinHealth,
        OnCriticalHit,
        OnPlayerHeal,
        PlayerRoll,
        ReceiveDamage,
        SurroundedBy,
        TargetsQuantity
    }
}