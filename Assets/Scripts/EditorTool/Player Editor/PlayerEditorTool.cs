using System;
using System.Collections.Generic;
using System.Linq;
using EditorTool.Storage;
using BerserkPixel.Extensions;
using Extensions;
using Player.Input;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using DebugTools;

namespace EditorTool.PlayerEditor {
    [RequireComponent(typeof(PlayerEditorInput), typeof(PlayerCloudServices), typeof(CloudPlayerTools))]
    [RequireComponent(typeof(PlayerEditorFX))]
    public class PlayerEditorTool : MonoBehaviour {
        [Space]
        [Header("Tooling")]
        [SerializeField]
        private ToolsPerPlayer _toolsPerPlayer;

        [SerializeField]
        private SpriteRenderer _toolImage;

        [Space]
        [Header("Events")]
        [Space]
        [Tooltip("Event called when all tool categories are loaded")]
        public UnityEvent<Dictionary<ToolType, List<Tool>>> OnToolCategoriesLoaded;

        [Tooltip("Event called when a tool category is changed.")]
        public UnityEvent<int> OnToolCategoryChanged;

        [Tooltip("Event called when tool index is changed.")]
        public UnityEvent<int> OnToolIndexChanged;

        [Tooltip("Event called when tool's status is changed.")]
        public UnityEvent<Tool> OnToolChanged;

        // categories
        private ToolType[] _allCategories;
        private int _categoryIndex;

        private Dictionary<ToolType, List<Tool>> _toolsByCategory;

        private PlayerEditorInput _playerInput;

        // tooling
        private List<Tool> _currentTools;
        private int _toolIndex;

        private BattleGrid _battleGrid;
        private CloudPlayerTools _cloudPlayerTools;
        private ToolsPerPlayer _storageToolsPerPlayer;
        private PlayerCloudServices _playerCloudServices;
        private PlayerEditorFX _playerEditorFX;

        private void Awake() {
            _playerCloudServices = GetComponent<PlayerCloudServices>();
            _playerInput = GetComponent<PlayerEditorInput>();
            _cloudPlayerTools = GetComponent<CloudPlayerTools>();
            _playerEditorFX = GetComponent<PlayerEditorFX>();
            _battleGrid = BattleGrid.Instance;
        }

        private void OnEnable() {
            _playerInput.PlaceBuilding += HandlePlace;
            _playerInput.ChangeToolEvent += HandleChangeTool;
            _playerInput.DeleteEvent += HandleDelete;
            _playerInput.NextCategoryEvent += HandleNextCategory;
            _playerInput.PreviousCategoryEvent += HandlePreviousCategory;
        }

        private void OnDisable() {
            _playerInput.PlaceBuilding -= HandlePlace;
            _playerInput.ChangeToolEvent -= HandleChangeTool;
            _playerInput.DeleteEvent -= HandleDelete;
            _playerInput.NextCategoryEvent -= HandleNextCategory;
            _playerInput.PreviousCategoryEvent -= HandlePreviousCategory;

            // we save the current player's tools when we exit Edit Mode
            if (_storageToolsPerPlayer != null)
                _cloudPlayerTools.SavePlayerTools(_storageToolsPerPlayer);
        }

        public async void LoadToolsByCategory() {
            _allCategories = Enum.GetValues(typeof(ToolType)).Cast<ToolType>().OrderBy(category => category).ToArray();
            _toolsByCategory = new Dictionary<ToolType, List<Tool>>();

            var loadedTools = Resources.LoadAll<Tool>("Tools").ToHashSet();

            // we need to fetch the ToolsPerPlayer from storage
            var cloudTools = await _cloudPlayerTools.GetPlayerTools(loadedTools);
            if (cloudTools != null) {
                _storageToolsPerPlayer = cloudTools;
            }
            else {
                // we clone the default tools per player
                _storageToolsPerPlayer = _toolsPerPlayer.Clone();
            }

            foreach (var item in _allCategories) {
                var tools = loadedTools.Where(tool => tool.Type == item).ToList();
                foreach (var tool in tools) {
                    if (_storageToolsPerPlayer.Tools.ContainsKey(tool)) {
                        var amountAllowed = _storageToolsPerPlayer.Tools[tool];
                        tool.Setup(amountAllowed);
                    }
                }
                _toolsByCategory[item] = tools;
            }

            OnToolCategoriesLoaded?.Invoke(_toolsByCategory);
            UpdateCategory();
        }

        private void HandleNextCategory() {
            _categoryIndex = (_categoryIndex + 1) % _allCategories.Length;

            UpdateCategory();
        }

        private void HandlePreviousCategory() {
            _categoryIndex--;
            if (_categoryIndex < 0) {
                _categoryIndex = _allCategories.Length - 1;
            }

            UpdateCategory();
        }

        private void HandleChangeTool() {
            if (_currentTools.Count > 0) {
                _toolIndex = (_toolIndex + 1) % _currentTools.Count;
                OnToolIndexChanged?.Invoke(_toolIndex);
                UpdatePreview();
            }
        }

        private void HandleDelete() {
            var position = transform.position.ToInt();
            var tool = _battleGrid.RemoveObject(position);
            if (tool != null) {
                tool.IncreaseAmount(1);
                _storageToolsPerPlayer.Tools[tool] = tool.CurrentAmount;
                OnToolChanged?.Invoke(tool);

                _playerEditorFX.OnDelete(position);
            }
        }

        private void HandlePlace() {
            var newTool = _currentTools[_toolIndex];

            if (_battleGrid.CanPlaceObject(transform.position)) {
                if (newTool.OnlyCopy) {
                    // remove all previous copies of the same game object
                    var toCompare = newTool.MockPrefab.gameObject.name;

                    // we need to find any other reference of the same type as newTool.MockPrefab in the scene and remove them
                    var anyOtherOnScene = FindObjectsByType<GameObject>(FindObjectsSortMode.None).Where(x => x.name.StartsWith(toCompare)).ToList();
                    // add it to the _uniuqePrefabs dictionary so we can remove it later
                    foreach (var item in anyOtherOnScene) {
                        var tool = _battleGrid.RemoveObject(item.transform.position.ToInt());
                        if (tool != null) {
                            tool.IncreaseAmount(1);
                            _storageToolsPerPlayer.Tools[tool] = tool.CurrentAmount;
                        }
                    }
                }
                else if (!newTool.OnlyCopy && !newTool.HasAny()) {
                    DebugLog.Log("Not enough resources to place this tool.");
                    _playerEditorFX.OnNotEnoughResources(newTool);
                    return;
                }

                newTool.DecreaseAmount(1);
                _storageToolsPerPlayer.Tools[newTool] = newTool.CurrentAmount;
                _battleGrid.PlaceMockObject(transform.position, newTool);
                OnToolChanged?.Invoke(newTool);
            }
            else {
                DebugLog.Log("Can't place object here!");
                _playerEditorFX.OnNotProperPlace(_toolImage);
            }
        }

        private void UpdateCategory() {
            _toolIndex = 0;

            if (_toolsByCategory.TryGetValue(_allCategories[_categoryIndex], out _currentTools)) {
                OnToolCategoryChanged?.Invoke(_categoryIndex);

                OnToolIndexChanged?.Invoke(_toolIndex);

                UpdatePreview();
            }
            else {
                // clear preview image
                _toolImage.sprite = null;
            }
        }

        private void UpdatePreview() {
            if (_currentTools.Count > 0) {
                var newTool = _currentTools[_toolIndex];
                _toolImage.sprite = newTool.Icon();
            }
            else {
                // clear preview image
                _toolImage.sprite = null;
            }
        }

#if UNITY_EDITOR
        [Button]
        private void ResetCloudTools() {
            GetComponent<CloudPlayerTools>().SavePlayerTools(_toolsPerPlayer);
            LoadToolsByCategory();
        }
#endif
    }
}