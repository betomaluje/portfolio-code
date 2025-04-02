using System;
using System.IO;
using Modifiers.Skills;
using UnityEditor;
using UnityEngine;

namespace RemoteSync
{
    public class SkillRemoteLoaderEditor : BaseRemoteLoaderEditor
    {
        // URL of the Excel file
        protected override string RemoteExcelUrl => "https://docs.google.com/spreadsheets/d/1ASDtrewdf3dfg546dfg34/export?format=csv";

        // Local path to save the downloaded Excel file
        protected override string LocalFilePath => "Assets/Data/Modifiers/Skills.csv";

        // Where to save the ScriptableObjects
        protected override string OutputFolderPath => "Assets/Data/Modifiers/Skills";

        private int _newSkillsCount = 0;
        private int _updatedSkillsCount = 0;

        [MenuItem(Shortcuts.ToolsRemoteSkillLoaderData, false, 10)]
        private static void ShowWindow()
        {
            var window = GetWindow<SkillRemoteLoaderEditor>();
            window.titleContent = new GUIContent("SkillRemoteLoader");
            window.Show();
        }

        protected override void FinishSync(int elapsedSeconds)
        {
            Debug.Log($"Sync of Skills data complete after {elapsedSeconds}[s]. New: {_newSkillsCount}, Updated: {_updatedSkillsCount}");
        }

        protected override void ParseExcel(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogError($"Excel file not found at: {path}");
                return;
            }

            _newSkillsCount = 0;
            _updatedSkillsCount = 0;

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

                    if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                    {
                        continue;
                    }

                    string skillTypeString = columns[2];
                    string description = columns[3];
                    float endValue = float.Parse(columns[4]);
                    float duration = float.Parse(columns[5]);
                    string spriteName = columns[6];

                    if (Enum.TryParse(skillTypeString, out SkillType skillType))
                    {
                        // Call the method with parsed parameters
                        CreateSkillScriptableObject(id, name, skillType, description, endValue, duration, spriteName);
                    }

                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error parsing row {i}: {ex.Message}");
                }
            }
        }

        private void CreateSkillScriptableObject(int id, string name, SkillType skillType, string description, float endValue, float duration, string spriteName)
        {
            // Create a new instance of your ScriptableObject
            var specificFolder = Path.Combine(OutputFolderPath, skillType.ToString());

            string assetPath = Path.Combine(specificFolder, $"{name}.asset");
            var skillConfig = AssetDatabase.LoadAssetAtPath<SkillConfig>(assetPath);

            if (skillConfig == null)
            {
                // If the asset doesn't exist, create a new one
                Debug.Log($"Creating new Skill: {name}");
                skillConfig = SkillFactory.CreateSkill(skillType);
                AssetDatabase.CreateAsset(skillConfig, assetPath);
                _newSkillsCount++;
            }
            else
            {
                _updatedSkillsCount++;
            }

            Undo.RegisterCreatedObjectUndo(skillConfig, $"Create {name}");

            if (skillConfig == null)
            {
                Debug.LogError($"Failed to create Skill: {name}");
                return;
            }

            skillConfig.ID = id;
            skillConfig.Name = name;
            skillConfig.SkillType = skillType;
            skillConfig.Description = description;
            skillConfig.EndValue = endValue;
            skillConfig.Duration = duration;
            skillConfig.Icon = LookupSprite(skillType.ToString(), "Skills", spriteName);

            // Ensure the output folder exists
            if (!Directory.Exists(specificFolder))
            {
                Directory.CreateDirectory(specificFolder);
            }

            // Save the ScriptableObject
            EditorUtility.SetDirty(skillConfig);
            AssetDatabase.SaveAssets();
        }
    }
}