using System;
using System.IO;
using Modifiers.Conditions;
using Modifiers.Powerups;
using Stats;
using UnityEditor;
using UnityEngine;

namespace RemoteSync
{
    public class PowerupRemoteLoaderEditor : BaseRemoteLoaderEditor
    {
        // URL of the Excel file
        protected override string RemoteExcelUrl => "https://docs.google.com/spreadsheets/d/1ASDtrewdf3dfg546dfg34/export?format=csv";

        // Local path to save the downloaded Excel file
        protected override string LocalFilePath => "Assets/Data/Modifiers/Powerups.csv";

        // Where to save the ScriptableObjects
        protected override string OutputFolderPath => "Assets/Data/Modifiers/Powerups";

        private ConditionFactory _conditionFactory;

        private int _newPowerupCount = 0;
        private int _updatedPowerupCount = 0;

        [MenuItem(Shortcuts.ToolsRemotePowerupLoaderData, false, 10)]
        private static void ShowWindow()
        {
            var window = GetWindow<PowerupRemoteLoaderEditor>();
            window.titleContent = new GUIContent("PowerupRemoteLoader");
            window.Show();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _conditionFactory = new ConditionFactory();
        }

        protected override void FinishSync(int elapsedSeconds)
        {
            Debug.Log($"Sync of Powerups data complete after {elapsedSeconds}[s]. New: {_newPowerupCount}, Updated: {_updatedPowerupCount}");
        }

        protected override void ParseExcel(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogError($"Excel file not found at: {path}");
                return;
            }

            DebugTools.DebugLog.Log($"Parsing Powerups data from {path}");

            _newPowerupCount = 0;
            _updatedPowerupCount = 0;

            // Open the Excel CSV file
            string[] lines = File.ReadAllLines(path);

            // Skip the header row and process data rows
            for (int i = 1; i < lines.Length; i++) // Start at 1 to skip the header
            {
                string line = lines[i];
                string[] columns = ParseCsvLine(line);
                if (columns.Length < 7)
                {
                    Debug.LogWarning($"Row {i} skipped due to missing data.");
                    continue;
                }

                // Parse columns to extract parameters
                try
                {
                    int id = int.Parse(columns[0]);
                    string name = columns[1];
                    string statTypeString = columns[2];
                    string description = columns[3];
                    float endValue = float.Parse(columns[4]);
                    float duration = float.Parse(columns[5]);
                    float cooldown = float.Parse(columns[6]);
                    string spriteName = columns[7];
                    string conditions = columns[8];

                    if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description))
                    {
                        continue;
                    }

                    if (Enum.TryParse(statTypeString, out StatType statType))
                    {
                        // Call the method with parsed parameters
                        CreatePowerupScriptableObject(id, name, statType, description, endValue, duration, cooldown, spriteName, conditions);
                    }

                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error parsing row {i}: {ex.Message}");
                }
            }
        }

        private void CreatePowerupScriptableObject(
            int id,
            string name,
            StatType statType,
            string description,
            float endValue,
            float duration,
            float cooldown,
            string spriteName,
            string conditions
        )
        {
            // Create a new instance of your ScriptableObject
            var specificFolder = Path.Combine(OutputFolderPath, statType.ToString());

            string assetPath = Path.Combine(specificFolder, $"{name}.asset");
            var powerupConfig = AssetDatabase.LoadAssetAtPath<PowerupConfig>(assetPath);

            if (powerupConfig == null)
            {
                // If the asset doesn't exist, create a new one
                Debug.Log($"Creating new Powerup: {name}");
                powerupConfig = PowerupFactory.CreatePowerup(statType);
                AssetDatabase.CreateAsset(powerupConfig, assetPath);
                _newPowerupCount++;
            }
            else
            {
                _updatedPowerupCount++;
            }

            Undo.RegisterCreatedObjectUndo(powerupConfig, $"Create {name}");

            if (powerupConfig == null)
            {
                Debug.LogError($"Failed to create Powerup: {name}");
                return;
            }

            powerupConfig.ID = id;
            powerupConfig.Name = name;
            powerupConfig._statType = statType;
            powerupConfig.Description = description;
            powerupConfig.EndValue = endValue;

            if (duration < 0)
            {
                powerupConfig.Duration = 60 * 10; // 10 minutes by default
            }
            else
            {
                powerupConfig.Duration = duration;
            }

            powerupConfig.Cooldown = cooldown;
            powerupConfig.Icon = LookupSprite(statType.ToString(), "Powerups", spriteName);

            if (powerupConfig.NeedNewConditions() && !string.IsNullOrWhiteSpace(conditions))
            {
                var extractedConditions = ExtractConditions(conditions);
                if (extractedConditions.Length > 0)
                {
                    powerupConfig.SetConditions(extractedConditions);
                }
            }

            // Ensure the output folder exists
            if (!Directory.Exists(specificFolder))
            {
                Directory.CreateDirectory(specificFolder);
            }

            // Save the ScriptableObject
            EditorUtility.SetDirty(powerupConfig);
            AssetDatabase.SaveAssets();
        }

        private BaseCondition[] ExtractConditions(string conditions)
        {
            if (string.IsNullOrWhiteSpace(conditions))
            {
                return new BaseCondition[0];
            }

            string[] conditionStrings = conditions.Split('-');
            var conditionArray = new BaseCondition[conditionStrings.Length];

            for (int i = 0; i < conditionStrings.Length; i++)
            {
                string conditionString = conditionStrings[i].Trim();
                if (string.IsNullOrWhiteSpace(conditionString))
                {
                    continue;
                }

                if (Enum.TryParse(conditionString, out ConditionType conditionType))
                {
                    // Call the method with parsed parameters
                    conditionArray[i] = _conditionFactory.GetOrCreateCondition(conditionType);
                }
            }

            return conditionArray;
        }

    }
}