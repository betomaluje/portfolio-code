using System;
using System.Collections.Generic;
using System.Linq;
using Modifiers;
using Modifiers.Conditions;
using Modifiers.Powerups;
using Modifiers.Skills;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor {
    public class ModifiersDataEditor : OdinMenuEditorWindow {

        private const string c_ConditionsDataFolder = "Assets/Data/Modifiers/Conditions";
        private const string c_SkillsDataFolder = "Assets/Data/Modifiers/Skills";
        private const string c_PowerupsDataFolder = "Assets/Data/Modifiers/Powerups";
        private const string c_WeaponsDataFolder = "Assets/Data/Modifiers/Weapon Modifiers";

        private CreateModifier<SkillConfig> createSkill;
        private CreateModifier<PowerupConfig> createPowerup;
        private CreateModifier<WeaponModifier> createWeaponModifier;

        private string _modifierName = "Modifier";

        private object _selectedMainMenu; // The currently selected menu

        private ScriptableObject _selectedItem; // The currently selected item

        private OdinMenuTree _cachedMenuTree;

        private List<BaseCondition> cachedConditions;
        private List<SkillConfig> cachedSkillConfigs;
        private List<PowerupConfig> cachedPowerupConfigs;
        private List<WeaponModifier> cachedWeaponModifierConfigs;

        private readonly Dictionary<string, bool> _expandedCategories = new();

        private readonly List<ModifierItem> _items = new();

        [MenuItem(Shortcuts.ToolsModifiersData, false, -100)]
        private static void OpenWindow() {
            var window = GetWindow<ModifiersDataEditor>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 400);
        }

        protected override void OnEnable() {
            base.OnEnable();
            cachedConditions = new List<BaseCondition>();
            cachedSkillConfigs = new List<SkillConfig>();
            cachedPowerupConfigs = new List<PowerupConfig>();
            cachedWeaponModifierConfigs = new List<WeaponModifier>();

            RefreshConditions();
            RefreshSkillConfigs();
            RefreshPowerupConfigs();
            RefreshWeaponModifierConfigs();
        }

        private void RefreshConditions() {
            cachedConditions.Clear();
            var conditionConfigsGuids = AssetDatabase.FindAssets("t:BaseCondition", new[] { c_ConditionsDataFolder });
            foreach (var guid in conditionConfigsGuids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var conditionConfig = AssetDatabase.LoadAssetAtPath<BaseCondition>(path);
                if (conditionConfig != null) {
                    cachedConditions.Add(conditionConfig);
                }
            }

            cachedConditions = cachedConditions.GroupBy(x => x.GetType().Name).OrderBy(x => x.Key).SelectMany(x => x).ToList();
        }

        private void RefreshSkillConfigs() {
            cachedSkillConfigs.Clear();
            var skillConfigsGuids = AssetDatabase.FindAssets("t:SkillConfig", new[] { c_SkillsDataFolder });
            foreach (var guid in skillConfigsGuids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var skillConfig = AssetDatabase.LoadAssetAtPath<SkillConfig>(path);
                if (skillConfig != null) {
                    cachedSkillConfigs.Add(skillConfig);
                }
            }

            cachedSkillConfigs = cachedSkillConfigs.GroupBy(x => x.GetType().Name).OrderBy(x => x.Key).SelectMany(x => x).ToList();
        }

        private void RefreshPowerupConfigs() {
            cachedPowerupConfigs.Clear();
            var powerupConfigsGuids = AssetDatabase.FindAssets("t:PowerupConfig", new[] { c_PowerupsDataFolder });
            foreach (var guid in powerupConfigsGuids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var skillConfig = AssetDatabase.LoadAssetAtPath<PowerupConfig>(path);
                if (skillConfig != null) {
                    cachedPowerupConfigs.Add(skillConfig);
                }
            }

            cachedPowerupConfigs = cachedPowerupConfigs.GroupBy(x => x.GetType().Name).OrderBy(x => x.Key).SelectMany(x => x).ToList();
        }

        private void RefreshWeaponModifierConfigs() {
            cachedWeaponModifierConfigs.Clear();
            var skillConfigsGuids = AssetDatabase.FindAssets("t:WeaponModifier", new[] { c_WeaponsDataFolder });
            foreach (var guid in skillConfigsGuids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var skillConfig = AssetDatabase.LoadAssetAtPath<WeaponModifier>(path);
                if (skillConfig != null) {
                    cachedWeaponModifierConfigs.Add(skillConfig);
                }
            }

            cachedWeaponModifierConfigs = cachedWeaponModifierConfigs.GroupBy(x => x.GetType().Name).OrderBy(x => x.Key).SelectMany(x => x).ToList();
        }

        protected override void OnDestroy() {
            base.OnDestroy();

            if (createSkill != null) {
                DestroyImmediate(createSkill.NewModifier);
            }

            if (createPowerup != null) {
                DestroyImmediate(createPowerup.NewModifier);
            }

            if (createWeaponModifier != null) {
                DestroyImmediate(createWeaponModifier.NewModifier);
            }
        }

        protected override OdinMenuTree BuildMenuTree() {
            if (_cachedMenuTree != null) {
                return _cachedMenuTree;
            }

            _cachedMenuTree = new OdinMenuTree {
                { "Conditions", new MenuItemDetails("Conditions") },
                { "Skills", new MenuItemDetails("Skills") },
                { "Powerups", new MenuItemDetails("Powerups") },
                { "Weapon Modifiers", new MenuItemDetails("Weapon Modifiers") }
            };

            return _cachedMenuTree;
        }

        protected override void OnBeginDrawEditors() {
            OdinMenuTreeSelection selected = MenuTree.Selection;

            if (selected.SelectedValue != null) {
                if (selected.SelectedValue != _selectedMainMenu) {
                    _items.Clear();
                    _selectedItem = null;
                }

                _selectedMainMenu = selected.SelectedValue;

                DrawCategory();
            }
        }

        private void DrawCategory() {
            if (_selectedMainMenu is MenuItemDetails itemDetails) {

                SirenixEditorGUI.BeginBox(itemDetails.Description, true, GUILayout.Width(position.width * .8f));

                SirenixEditorGUI.BeginHorizontalToolbar();
                {
                    // TODO: create a new panel for creation since we CANNOT create new modifiers form abstract classes
                    // AddCreateButton(itemDetails.Description);
                    AddRefreshButton(itemDetails.Description);
                }

                SirenixEditorGUI.EndHorizontalToolbar();

                // Begin horizontal layout for two panels
                GUILayout.BeginHorizontal();
                float leftPanelPercentageSize = .2f;

                #region Left Panel
                // Left panel
                GUILayout.BeginVertical(GUILayout.Width(position.width * leftPanelPercentageSize)); // Adjust width as needed

                AddItems(itemDetails);

                // Add any other content specific to this panel here
                GUILayout.EndVertical();
                #endregion

                #region Right Panel

                // Right panel
                if (_selectedItem != null) {
                    // Draw the details of clicked item
                    DrawItemDetails(_selectedItem);
                }

                #endregion

                GUILayout.EndHorizontal(); // End the horizontal layout

                SirenixEditorGUI.EndBox();
            }
        }

        private void AddItems(MenuItemDetails itemDetails) {
            if (_items == null || _items.Count == 0) {
                List<ScriptableObject> itemsToDisplay = new();

                switch (itemDetails.Description) {
                    case "Conditions":
                        itemsToDisplay = cachedConditions.Cast<ScriptableObject>().ToList();
                        break;
                    case "Skills":
                        itemsToDisplay = cachedSkillConfigs.Cast<ScriptableObject>().ToList();
                        break;
                    case "Powerups":
                        itemsToDisplay = cachedPowerupConfigs.Cast<ScriptableObject>().ToList();
                        break;
                    case "Weapon Modifiers":
                        itemsToDisplay = cachedWeaponModifierConfigs.Cast<ScriptableObject>().ToList();
                        break;
                }

                if (itemsToDisplay.Count == 0) {
                    return;
                }

                string prevCategory = itemsToDisplay[0].GetType().Name;
                var amountCategory = itemsToDisplay.Count(x => x.GetType().Name == prevCategory);

                _items.Add(new ModifierItem(null, prevCategory, false, amountCategory));

                if (!_expandedCategories.ContainsKey(prevCategory)) {
                    _expandedCategories.Add(prevCategory, true);
                }

                for (int i = 0; i < itemsToDisplay.Count; i++) {
                    var itemToDisplay = itemsToDisplay[i];

                    if (itemToDisplay.GetType().Name != prevCategory) {
                        prevCategory = itemToDisplay.GetType().Name;

                        amountCategory = itemsToDisplay.Count(x => x.GetType().Name == prevCategory);

                        _items.Add(new ModifierItem(null, prevCategory, false, amountCategory));

                        if (!_expandedCategories.ContainsKey(prevCategory)) {
                            _expandedCategories[prevCategory] = true;
                        }
                    }

                    _items.Add(new ModifierItem(itemToDisplay, itemToDisplay.name));
                }
            }

            // Display the items
            var firstItem = _items.First();
            string prevCategory2 = firstItem.Name;

            _expandedCategories[prevCategory2] = SirenixEditorGUI.Foldout(_expandedCategories[prevCategory2], SplitCamelCase(prevCategory2) + $" ({firstItem.Amount})");

            foreach (var item in _items) {
                if (item.IsButton) {
                    bool expanded = _expandedCategories[prevCategory2];

                    if (!expanded) {
                        continue;
                    }

                    if (item.Modifier != null && SirenixEditorGUI.Button(item.Name, ButtonSizes.Medium) && _selectedItem != item.Modifier) {
                        _selectedItem = item.Modifier;
                    }
                }
                else {
                    if (prevCategory2 != item.Name) {
                        prevCategory2 = item.Name;
                        _expandedCategories[prevCategory2] = SirenixEditorGUI.Foldout(_expandedCategories[prevCategory2], SplitCamelCase(prevCategory2) + $" ({item.Amount})");
                    }
                }
            }
        }

        private string SplitCamelCase(string input) {
            return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        }

        private void DrawItemDetails(ScriptableObject item) {
            if (item != null) {
                GUILayout.BeginVertical();

                AddDeleteButton(item);
                EditorGUI.BeginChangeCheck();

                SerializedObject serializedObject = new(item);
                SerializedProperty iterator = serializedObject.GetIterator();
                serializedObject.Update();

                // Loop through all serialized properties
                while (iterator.NextVisible(true)) {
                    if (iterator.propertyType == SerializedPropertyType.ObjectReference && iterator.objectReferenceValue != null) {
                        Texture2D texture = AssetPreview.GetAssetPreview(iterator.objectReferenceValue);
                        if (texture != null) {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label(texture, GUILayout.MaxWidth(100), GUILayout.MaxHeight(100));
                            GUILayout.FlexibleSpace();

                            var objectReferenceValue = iterator.objectReferenceValue;

                            EditorGUI.BeginChangeCheck();

                            objectReferenceValue = EditorGUILayout.ObjectField(objectReferenceValue, typeof(Sprite), false);

                            if (EditorGUI.EndChangeCheck()) {
                                serializedObject.FindProperty(iterator.name).objectReferenceValue = objectReferenceValue;                                
                            }

                            GUILayout.EndHorizontal();
                        }
                    }
                    else {
                        if (iterator.depth == 0) {
                            EditorGUILayout.PropertyField(iterator);
                        }
                    }
                }

                if (EditorGUI.EndChangeCheck()) {
                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(serializedObject.targetObject);
                }

                GUILayout.EndVertical();
            }
        }

        private void AddDeleteButton(ScriptableObject modifierToDelete) {
            if (modifierToDelete == null) {
                return;
            }

            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                GUILayout.FlexibleSpace();

                string modifierName = modifierToDelete.name;

                if (SirenixEditorGUI.ToolbarButton(new GUIContent($"Delete {modifierName}"))) {

                    if (EditorUtility.DisplayDialog($"Delete {modifierName}?",
                $"Are you sure you want to delete {modifierName}?", "Delete", "Do Not Delete")) {

                        string path = AssetDatabase.GetAssetPath(modifierToDelete);
                        Undo.RecordObject(modifierToDelete, "Delete Modifier");

                        if (AssetDatabase.DeleteAsset(path)) {
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();

                            _selectedItem = null;

                            RefreshSkillConfigs();
                            RefreshPowerupConfigs();
                            RefreshWeaponModifierConfigs();

                            _items.Clear();
                        }
                        else {
                            EditorUtility.DisplayDialog("Error", $"Failed to delete {modifierName}.", "OK");
                        }
                    }
                }
            }

            SirenixEditorGUI.EndHorizontalToolbar();
        }

        private void AddRefreshButton(string menuName) {
            if (SirenixEditorGUI.ToolbarButton(new GUIContent("Refresh"))) {
                _items.Clear();

                switch (menuName) {
                    case "Skills":
                        RefreshSkillConfigs();
                        break;
                    case "Powerups":
                        RefreshPowerupConfigs();
                        break;
                    case "Weapon Modifiers":
                        RefreshWeaponModifierConfigs();
                        break;
                    default:
                        break;
                };
            }
        }

        private void AddCreateButton(string menuName) {
            // GUILayout.FlexibleSpace();
            SirenixEditorGUI.BeginIndentedHorizontal();
            _modifierName = EditorGUILayout.TextField("Modifier Name", _modifierName);

            if (SirenixEditorGUI.ToolbarButton(new GUIContent($"Create New {menuName}"))) {
                switch (menuName) {
                    case "Skills":
                        createSkill = new CreateModifier<SkillConfig>("Skill");
                        createSkill.CreateNewModifier(c_SkillsDataFolder, _modifierName);
                        Undo.RegisterCreatedObjectUndo(createSkill.NewModifier, $"Create {_modifierName}");
                        break;
                    case "Powerups":
                        createPowerup = new CreateModifier<PowerupConfig>("Powerup");
                        createPowerup.CreateNewModifier(c_PowerupsDataFolder, _modifierName);
                        Undo.RegisterCreatedObjectUndo(createPowerup.NewModifier, $"Create {_modifierName}");
                        break;
                    case "Weapon Modifiers":
                        createWeaponModifier = new CreateModifier<WeaponModifier>("Weapon Modifier");
                        createWeaponModifier.CreateNewModifier(c_WeaponsDataFolder, _modifierName);
                        Undo.RegisterCreatedObjectUndo(createWeaponModifier.NewModifier, $"Create {_modifierName}");
                        break;
                    default:
                        break;
                };
            }
            SirenixEditorGUI.EndIndentedHorizontal();
        }
    }

    internal class ModifierItem {
        public ScriptableObject Modifier { get; }
        public string Name { get; }
        public bool IsButton { get; }
        public int Amount { get; set; }

        public ModifierItem(ScriptableObject modifier, string name, bool isButton = true, int amount = 0) {
            Modifier = modifier;
            Name = name;
            IsButton = isButton;
            Amount = amount;
        }
    }

    internal class MenuItemDetails {
        public string Description { get; }

        public MenuItemDetails(string description) {
            Description = description;
        }
    }

    internal class CreateModifier<T> where T : ScriptableObject, IModifier {
        [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
        public T NewModifier;

        private readonly string _type;

        public CreateModifier(string type) {
            _type = type;
        }

        public void CreateNewModifier(string path, string modifierName) {
            // create new instance of the SO
            NewModifier = ScriptableObject.CreateInstance<T>();
            NewModifier.name = $"{modifierName}";

            AssetDatabase.CreateAsset(NewModifier, $"{path}/{modifierName}.asset");
            AssetDatabase.SaveAssets();

            Undo.RecordObject(NewModifier, "Create New Modifier");
        }
    }
}