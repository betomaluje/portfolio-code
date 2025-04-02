using System;
using System.Collections.Generic;
using System.Linq;
using BerserkPixel.Health;
using BerserkPixel.Utils;
using Enemies;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor {
    public class SwarmDataEditor : OdinMenuEditorWindow {
        private const string SwarmsPath = "Assets/Resources/Swarms/";
        private const string PrefabsPath = "Assets/Prefabs/Enemies";

        private readonly List<CharacterHealth> _allAvailablePrefabs = new();
        private readonly Dictionary<string, Texture2D> _previewTextures = new();
        private List<CharacterHealth> _prefabsPerSelection = new();

        private string _newSwarmName = "New Enemy Swarm";
        private CreateClass<EnemySwarmConfig> createSwarmConfig;

        private EnemySwarmConfig _lastSelection;

        private bool _isUsedPrefabsFoldout = true;
        private bool _isAvailablePrefabsFoldout = true;
        private SerializedProperty _prefabsProperty;

        [MenuItem(Shortcuts.ToolsEnemySwarmData, false, -100)]
        private static void OpenWindow() {
            var window = GetWindow<SwarmDataEditor>("Enemy Swarms");
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 500);
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            Undo.ClearUndo(_lastSelection);

            if (createSwarmConfig != null) {
                DestroyImmediate(createSwarmConfig.NewObject);
            }
        }

        protected override void OnEnable() {
            base.OnEnable();

            _allAvailablePrefabs.Clear();
            _previewTextures.Clear();

            var prefabsGuids = AssetDatabase.FindAssets("t:Prefab", new[] { PrefabsPath });
            foreach (var guid in prefabsGuids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null && prefab.TryGetComponent<CharacterHealth>(out var health)) {
                    _allAvailablePrefabs.Add(health);

                    _previewTextures[prefab.name] = GetPreviewTexture(prefab);
                }
            }
        }

        private void UpdatePrefabs() {
            if (_lastSelection == null) return;

            var serializedObject = new SerializedObject(_lastSelection);
            _prefabsProperty = serializedObject.FindProperty("Prefabs");

            var currentEnemies = _lastSelection.Prefabs.Select(x => x._item).ToList();
            _prefabsPerSelection = _allAvailablePrefabs.Except(currentEnemies).ToList();
        }

        protected override OdinMenuTree BuildMenuTree() {
            OdinMenuTree tree = new();

            tree.AddAllAssetsAtPath("Swarms", SwarmsPath, typeof(EnemySwarmConfig), true);

            tree.SortMenuItemsByName();

            return tree;
        }

        private Texture2D GetPreviewTexture(GameObject prefab) {
            var spriteRenderer = prefab.GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null) {
                return AssetPreview.GetAssetPreview(spriteRenderer.sprite);
            }
            return null;
        }

        private void AddCreateButton() {
            SirenixEditorGUI.BeginIndentedHorizontal();
            {
                _newSwarmName = EditorGUILayout.TextField("Swarm Name", _newSwarmName);
            }

            SirenixEditorGUI.EndIndentedHorizontal();

            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                GUILayout.FlexibleSpace();

                if (!string.IsNullOrEmpty(_newSwarmName) && SirenixEditorGUI.ToolbarButton(new GUIContent($"Create {_newSwarmName}"))) {
                    createSwarmConfig = new CreateClass<EnemySwarmConfig>(SwarmsPath, "Enemy Swarm Config");
                    createSwarmConfig.CreateNewObject(_newSwarmName);
                }
            }

            SirenixEditorGUI.EndHorizontalToolbar();
        }

        protected override void OnBeginDrawEditors() {
            OdinMenuTreeSelection selected = MenuTree.Selection;

            if (selected.SelectedValue == null) {
                AddCreateButton();
                return;
            }

            if (_lastSelection == null && selected.SelectedValue is EnemySwarmConfig a) {
                _lastSelection = a;
            }

            if (selected.SelectedValue is EnemySwarmConfig enemySwarmConfig && enemySwarmConfig.GetHashCode() != _lastSelection.GetHashCode()) {
                _lastSelection = enemySwarmConfig;

                UpdatePrefabs();
            }

            var titleStyle = new GUIStyle(EditorStyles.boldLabel) {
                fontSize = 14,
                alignment = TextAnchor.MiddleLeft,
                margin = new RectOffset(6, 0, 10, 10)
            };
            var title = new GUIContent("Available Enemies");
            GUILayout.Label(title, titleStyle);

            SirenixEditorGUI.InfoMessageBox("Here you can see all enemies that are not in the current swarm configuration. To add an enemy to the swarm, simply click on it.", true);

            SirenixEditorGUI.BeginBox();

            DrawPrefabsGrid(
                GetGameObjectsFromCharacterHealth(_prefabsPerSelection),
                ref _isAvailablePrefabsFoldout,
                "Available Enemies",
                onItemClick: (index, prefab) => {
                    AddPrefabToConfig(prefab);
                },
                onItemHover: (prefab) => {
                    OnMouseOverPrefab(prefab);
                },
                maxItemsPerRow: 10
            );

            SirenixEditorGUI.EndBox();

            SirenixEditorGUI.BeginBox();

            DrawPrefabsGrid(
                GetGameObjectsFromSerializedProperty(_prefabsProperty),
                ref _isUsedPrefabsFoldout,
                "Prefabs in this Configuration",
                onItemClick: (index, prefab) => {
                    SerializedProperty element = _prefabsProperty.GetArrayElementAtIndex(index);

                    if (element.boxedValue is WeightedListItem<CharacterHealth> item) {
                        DeleteConfirmDialog(prefab.name, item);
                    }
                },
                maxItemsPerRow: 10
            );

            SirenixEditorGUI.EndBox();
        }

        private IList<GameObject> GetGameObjectsFromCharacterHealth(List<CharacterHealth> characterHealthList) => characterHealthList
                                .Select(character => character.gameObject) // Assuming `CharacterHealth` has a reference to a `GameObject`
                                .ToList();

        private IList<GameObject> GetGameObjectsFromSerializedProperty(SerializedProperty property) {
            List<GameObject> gameObjects = new();
            if (property != null && property.isArray) {
                for (int i = 0; i < property.arraySize; i++) {
                    SerializedProperty element = property.GetArrayElementAtIndex(i);

                    if (element.boxedValue is WeightedListItem<CharacterHealth> item) {
                        GameObject prefab = item._item.gameObject;
                        if (prefab != null) {
                            gameObjects.Add(prefab);
                        }
                    }
                }
            }
            return gameObjects;
        }

        private void DrawPrefabsGrid(IList<GameObject> prefabList,
        ref bool foldout,
        string title,
        Action<int, GameObject> onItemClick = null,
        Action<GameObject> onItemHover = null,
        int maxItemsPerRow = 5) {
            if (prefabList == null) return;

            var titleStyle = new GUIStyle(EditorStyles.foldoutHeader) {
                fontSize = 14,
                alignment = TextAnchor.MiddleLeft,
                margin = new RectOffset(6, 0, 4, 4)
            };

            int totalItems = prefabList.Count;

            foldout = EditorGUILayout.Foldout(foldout, $"{title} ({totalItems})", true, titleStyle);

            if (!foldout) {
                return;
            }

            // Calculate total number of rows based on number of elements and columns
            int numRows = Mathf.CeilToInt((float)totalItems / maxItemsPerRow);

            float cellSize = 80f;
            float padding = 10f;  // Define padding around the texture

            GameObject mouseOverPrefab = null;

            for (int row = 0; row < numRows; row++) {
                EditorGUILayout.BeginHorizontal();

                GUIStyle centeredStyle = new(GUI.skin.label) {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 14
                };

                for (int col = 0; col < maxItemsPerRow; col++) {
                    int index = row * maxItemsPerRow + col;

                    if (index >= totalItems) break;

                    var prefab = prefabList[index];
                    var prefabName = prefab.name;

                    // Retrieve or generate the preview texture
                    if (_previewTextures.TryGetValue(prefabName, out Texture2D texture)) {
                        if (texture == null) {
                            texture = GetPreviewTexture(prefab);
                            _previewTextures[prefabName] = texture;
                        }
                    }

                    // Draw each item in a fixed-size box to form a grid cell
                    GUILayoutOption cellWidth = GUILayout.Width(cellSize);
                    GUILayoutOption cellHeight = GUILayout.Height(cellSize);

                    EditorGUILayout.BeginVertical(cellWidth, cellHeight);
                    if (texture != null) {

                        Rect buttonRect = GUILayoutUtility.GetRect(cellSize, cellSize);

                        // Draw an invisible button (or any placeholder content) to make this area clickable
                        var buttonSelected = GUI.Button(buttonRect, GUIContent.none);

                        // Now draw the texture over the button area
                        Rect paddedRect = new(
                            buttonRect.x + padding,
                            buttonRect.y + padding,
                            buttonRect.width - 2 * padding,
                            buttonRect.height - 2 * padding
                        );
                        GUI.DrawTexture(paddedRect, texture, ScaleMode.ScaleAndCrop);

                        var rect = GUILayoutUtility.GetLastRect();
                        var pos = Event.current.mousePosition;

                        if (rect.Contains(pos)) {
                            mouseOverPrefab = prefab;
                        }

                        if (buttonSelected) {
                            onItemClick?.Invoke(index, prefab);
                        }
                    }
                    else {
                        GUILayoutUtility.GetRect(cellSize, cellSize);
                    }

                    EditorGUILayout.LabelField(prefabName, centeredStyle, GUILayout.Width(cellSize));

                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndHorizontal();
            }

            if (mouseOverPrefab != null) {
                onItemHover?.Invoke(mouseOverPrefab);
            }
        }

        private void OnMouseOverPrefab(GameObject prefab) {
            if (_previewTextures == null || _previewTextures.Count == 0) {
                return;
            }

            if (Event.current.type == EventType.Repaint) {
                var name = prefab.name;

                if (_previewTextures.TryGetValue(name, out var texture)) {
                    if (texture == null) {
                        texture = GetPreviewTexture(prefab);
                        _previewTextures[name] = texture;
                    }

                    if (texture == null) {
                        return;
                    }

                    var pos = Event.current.mousePosition;

                    var maxSize = 100;
                    var textureSize = new Vector2(Mathf.Min(texture.width, maxSize), Mathf.Min(texture.height, maxSize));

                    var rect = new Rect(pos - textureSize, textureSize);
                    // move it so it's not in the mouse pointer
                    rect.y += textureSize.y / 2;

                    // Optional background for better visibility
                    EditorGUI.DrawRect(rect, new Color(0, 0, 0, 0.5f));

                    GUI.color = new Color(1, 1, 1, 1f);
                    GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit, true);
                    GUI.color = Color.white; // Reset color
                }
            }
        }

        private void AddPrefabToConfig(GameObject prefab) {
            if (prefab.TryGetComponent<CharacterHealth>(out var healthScript) == false) {
                return;
            }

            Undo.RecordObject(_lastSelection, "Added Enemy");

            // var prefab = healthScript.gameObject;

            // we remove from the list of available prefabs
            _prefabsPerSelection.Remove(healthScript);

            // we add to the list of selected prefabs
            var currentEnemies = _lastSelection.Prefabs.ToList();
            var averageWeight = currentEnemies.Count > 0 ? (int)currentEnemies.Average(x => x._weight) : 1;

            currentEnemies.Insert(0, new WeightedListItem<CharacterHealth>(healthScript, averageWeight));

            _lastSelection.Prefabs = currentEnemies.ToArray();

            UpdatePrefabs();

            // we save the changes
            EditorUtility.SetDirty(_lastSelection);

            // finally we redraw
            ForceMenuTreeRebuild();
        }

        private void DeleteConfirmDialog(string name, WeightedListItem<CharacterHealth> item) {
            if (EditorUtility.DisplayDialog($"Delete {name}?",
                $"Are you sure you want to delete {name}?", "Delete", "Do Not Delete")) {

                RemovePrefabFromConfig(item);
            }
        }

        private void RemovePrefabFromConfig(WeightedListItem<CharacterHealth> item) {
            Undo.RecordObject(_lastSelection, "Remove Enemy");

            var currentEnemies = _lastSelection.Prefabs.ToList();
            currentEnemies.Remove(item);
            _lastSelection.Prefabs = currentEnemies.ToArray();

            UpdatePrefabs();

            // we save the changes
            EditorUtility.SetDirty(_lastSelection);

            // finally we redraw
            ForceMenuTreeRebuild();
        }

        internal class CreateClass<T> where T : ScriptableObject {
            [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
            public T NewObject;

            private readonly string _type;
            private readonly string _path;

            public CreateClass(string path, string type) {
                _path = path;
                _type = type;
                NewObject = CreateInstance<T>();
                NewObject.name = $"New {_type}";
            }

            public void CreateNewObject(string objectName) {
                // create new instance of the SO
                NewObject = CreateInstance<T>();
                NewObject.name = $"{objectName} {_type}";

                Undo.RegisterCreatedObjectUndo(NewObject, $"Create {objectName}");

                AssetDatabase.CreateAsset(NewObject, $"{_path}/{objectName} {_type}.asset");
                AssetDatabase.SaveAssets();

                Selection.activeObject = NewObject;
                EditorUtility.SetDirty(NewObject);
                EditorGUIUtility.PingObject(NewObject);
                SceneView.FrameLastActiveSceneView();
            }
        }
    }
}