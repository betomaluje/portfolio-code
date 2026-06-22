using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Weapons.Editor {
    /// <summary>
    /// Automatically updates the Used_Weapons.md documentation file
    /// whenever weapon assets under Assets/Resources/Weapons are created, deleted, or moved.
    /// Also provides a manual menu item at "Aurora/Generate Used Weapons List".
    /// </summary>
    public class WeaponReferencePostprocessor : AssetPostprocessor {
        private const string WeaponsFolder = "Assets/Resources/Weapons";
        private const string OutputFileName = "Used_Weapons.md";

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
            bool anyChange = false;
            foreach (string path in importedAssets.Concat(deletedAssets).Concat(movedAssets)) {
                if (path.StartsWith(WeaponsFolder) && path.EndsWith(".asset")) {
                    anyChange = true;
                    break;
                }
            }

            if (anyChange) {
                GenerateUsedWeaponsList();
            }
        }

        [MenuItem("Tools/Aurora/Generate Used Weapons List")]
        public static void GenerateUsedWeaponsList() {
            string weaponsFullPath = Path.Combine(Application.dataPath, "Resources/Weapons");
            if (!Directory.Exists(weaponsFullPath)) {
                Debug.LogWarning($"[WeaponReferencePostprocessor] Resources folder not found at: {weaponsFullPath}");
                return;
            }

            // Find all ScriptableObjects in the folder
            string[] assetGuids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { WeaponsFolder });
            HashSet<string> paths = new HashSet<string>();
            foreach (var guid in assetGuids) {
                paths.Add(AssetDatabase.GUIDToAssetPath(guid));
            }

            // Group assets by category (subfolder relative to WeaponsFolder)
            var categorizedAssets = new SortedDictionary<string, List<AssetInfo>>();

            foreach (string path in paths) {
                if (!path.StartsWith(WeaponsFolder) || !path.EndsWith(".asset")) continue;

                var obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                if (obj == null) continue;

                string relativePath = path.Substring(WeaponsFolder.Length).TrimStart('/', '\\');
                string category = "Root";
                string dirName = Path.GetDirectoryName(relativePath);
                if (!string.IsNullOrEmpty(dirName)) {
                    category = dirName.Replace('\\', '/');
                }

                // Get associated MonoScript reference
                var ms = MonoScript.FromScriptableObject(obj);
                string scriptName = "Unknown Script";
                string scriptRelPath = "";
                if (ms != null) {
                    scriptName = ms.name;
                    string scriptPath = AssetDatabase.GetAssetPath(ms);
                    scriptRelPath = GetRelativePath(scriptPath, WeaponsFolder);
                }

                // Extract stats if the object is a Weapon class
                string damage = "N/A";
                string cooldown = "N/A";
                string knockback = "N/A";
                string range = "N/A";

                if (obj is Weapons.Weapon weapon) {
                    damage = weapon.GetDamage().ToString();
                    cooldown = weapon.AttackCooldown.ToString();
                    knockback = weapon.KnockbackForce.ToString();
                    range = weapon.Range.ToString();
                }

                string assetName = Path.GetFileNameWithoutExtension(path);
                string assetRelPath = GetRelativePath(path, WeaponsFolder);

                var info = new AssetInfo {
                    Name = assetName,
                    AssetRelPath = assetRelPath,
                    ScriptType = scriptName,
                    ScriptRelPath = scriptRelPath,
                    Damage = damage,
                    Cooldown = cooldown,
                    Knockback = knockback,
                    Range = range
                };

                if (!categorizedAssets.ContainsKey(category)) {
                    categorizedAssets[category] = new List<AssetInfo>();
                }
                categorizedAssets[category].Add(info);
            }

            // Generate Markdown Content
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("# Aurora Genesis - Used Weapons Reference");
            sb.AppendLine();
            sb.AppendLine("This document lists all the weapon configuration assets configured under `Assets/Resources/Weapons`, detailing their script type and basic stats.");
            sb.AppendLine();
            sb.AppendLine("> [!NOTE]");
            sb.AppendLine("> This file is automatically updated by the Unity Editor whenever assets are added, deleted, or moved in `Assets/Resources/Weapons`.");

            foreach (var kvp in categorizedAssets) {
                sb.AppendLine();
                sb.AppendLine($"## 📂 {kvp.Key} Weapons");
                sb.AppendLine();
                sb.AppendLine("| Asset Name | Script Type | Damage | Cooldown | Knockback | Range |");
                sb.AppendLine("| :--- | :--- | :---: | :---: | :---: | :---: |");
                
                var sortedList = kvp.Value.OrderBy(a => a.Name).ToList();
                foreach (var a in sortedList) {
                    // Escape space characters for valid markdown links
                    string assetLink = $"[{a.Name}]({a.AssetRelPath.Replace(" ", "%20")})";
                    string scriptLink = string.IsNullOrEmpty(a.ScriptRelPath) ? $"`{a.ScriptType}`" : $"[{a.ScriptType}]({a.ScriptRelPath.Replace(" ", "%20")})";
                    sb.AppendLine($"| {assetLink} | {scriptLink} | {a.Damage} | {a.Cooldown} | {a.Knockback} | {a.Range} |");
                }
            }

            string outputPath = Path.Combine(weaponsFullPath, OutputFileName);
            File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);
            
            Debug.Log($"[WeaponReferencePostprocessor] Used_Weapons.md successfully generated at {outputPath}");
        }

        private static string GetRelativePath(string filespec, string folder) {
            System.Uri pathUri = new System.Uri(Path.GetFullPath(filespec));
            string folderFullPath = Path.GetFullPath(folder);
            if (!folderFullPath.EndsWith(Path.DirectorySeparatorChar.ToString()) && !folderFullPath.EndsWith("/")) {
                folderFullPath += Path.DirectorySeparatorChar;
            }
            System.Uri folderUri = new System.Uri(folderFullPath);
            string rel = System.Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString());
            return rel.Replace('\\', '/');
        }

        private struct AssetInfo {
            public string Name;
            public string AssetRelPath;
            public string ScriptType;
            public string ScriptRelPath;
            public string Damage;
            public string Cooldown;
            public string Knockback;
            public string Range;
        }
    }
}
