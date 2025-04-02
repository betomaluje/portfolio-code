using System.Collections.Generic;
using System.Linq;
using Base;
using BerserkPixel.Health;
using BerserkPixel.StateMachine;
using Enemies;
using Enemies.Components;
using Enemies.States;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Weapons;

namespace Editor {
    public class PerEnemyDataEditor : OdinMenuEditorWindow {
        private const string Path = "Assets/Prefabs/Enemies";

        private CreateState<EnemyAttackState> createAttackState;
        private CreateState<EnemyChaseState> createChaseState;
        private CreateState<EnemyHitState> createHitState;
        private CreateState<EnemyIdleState> createIdleState;
        private CreateState<EnemyWanderState> createWanderState;
        private CreateClass<AnimationConfig> createAnimationConfig;
        private CreateClass<HealthConfig> createHealthConfig;

        private GameObject _selectedMainMenu;

        private readonly Dictionary<MonoBehaviour, bool> _expandedCategories = new();

        private string _enemyName = "Enemy";

        [MenuItem(Shortcuts.ToolsPerEnemyData, false, -100)]
        private static void OpenWindow() {
            var window = GetWindow<PerEnemyDataEditor>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 500);
        }

        protected override void OnDestroy() {
            base.OnDestroy();

            if (createAttackState != null) {
                DestroyImmediate(createAttackState.NewState);
            }

            if (createChaseState != null) {
                DestroyImmediate(createChaseState.NewState);
            }

            if (createHitState != null) {
                DestroyImmediate(createHitState.NewState);
            }

            if (createIdleState != null) {
                DestroyImmediate(createIdleState.NewState);
            }

            if (createWanderState != null) {
                DestroyImmediate(createWanderState.NewState);
            }

            if (createAnimationConfig != null) {
                DestroyImmediate(createAnimationConfig.NewObject);
            }

            if (createHealthConfig != null) {
                DestroyImmediate(createHealthConfig.NewObject);
            }
        }

        protected override OdinMenuTree BuildMenuTree() {
            OdinMenuTree tree = new();

            tree.AddAllAssetsAtPath("Enemies", Path, typeof(GameObject));

            tree.SortMenuItemsByName();

            return tree;
        }

        protected override void OnBeginDrawEditors() {
            OdinMenuTreeSelection selected = MenuTree.Selection;

            if (selected.SelectedValue != null) {
                if (_selectedMainMenu == null) {
                    _selectedMainMenu = selected.SelectedValue as GameObject;
                }

                if (_selectedMainMenu != null && selected.SelectedValue.GetHashCode() != _selectedMainMenu.GetHashCode()) {
                    _selectedMainMenu = selected.SelectedValue as GameObject;
                }

                DrawPrefabScripts();
            }
        }

        private void DrawPrefabScripts() {
            if (_selectedMainMenu != null && _selectedMainMenu is GameObject itemDetails) {
                SirenixEditorGUI.BeginBox(itemDetails.name, true);

                MonoBehaviour[] allBehaviours = _selectedMainMenu.GetComponents<MonoBehaviour>();

                if (allBehaviours.Length > 0) {
                    var selectedBehaviour = new List<MonoBehaviour>(allBehaviours.Length);

                    foreach (MonoBehaviour behaviour in allBehaviours) {

                        if (!_expandedCategories.ContainsKey(behaviour)) {
                            _expandedCategories.Add(behaviour, true);
                        }

                        // only add allowed behaviours to the list
                        if (behaviour is EnemyStateMachine || behaviour is IAllyActionDecorator || behaviour is CharacterHealth ||
                            behaviour is EnemyWeaponManager || behaviour is ICharacterHolder) {

                            selectedBehaviour.Add(behaviour);
                        }
                    }

                    // we order the list alphabetically
                    selectedBehaviour = selectedBehaviour.OrderBy(mono => {
                        var splitted = mono.GetType().ToString().Split(".");
                        return splitted[^1]; // last one in the array
                    }).ToList();

                    foreach (MonoBehaviour behaviour in selectedBehaviour) {
                        GUILayout.BeginVertical();

                        DrawComponent(behaviour);

                        GUILayout.EndVertical();
                    }
                }

                SirenixEditorGUI.EndBox();
            }
        }

        private void DrawComponent(MonoBehaviour objectReference) {
            string[] splitted = objectReference.GetType().ToString().Split(".");
            string menuName = splitted[^1]; // last one in the array

            var style = new GUIStyle(EditorStyles.foldoutHeader) {
                font = EditorStyles.foldoutHeader.font,
                fontStyle = FontStyle.Normal,
                fontSize = 14,
                padding = new RectOffset(24, 0, 6, 6),
                normal = { background = EditorStyles.foldoutHeader.normal.background },
                onNormal = { background = EditorStyles.foldoutHeader.onNormal.background },
                hover = { background = EditorStyles.foldoutHeader.hover.background },
                onHover = { background = EditorStyles.foldoutHeader.onHover.background },
                active = { background = EditorStyles.foldoutHeader.active.background },
                onActive = { background = EditorStyles.foldoutHeader.onActive.background },
            };

            _expandedCategories[objectReference] = SirenixEditorGUI.Foldout(_expandedCategories[objectReference], menuName, style);
            bool expanded = _expandedCategories[objectReference];

            if (!expanded) {
                return;
            }

            EditorGUI.BeginChangeCheck();

            SerializedObject serializedObject = new(objectReference);
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
                        EditorGUILayout.ObjectField(iterator.objectReferenceValue, typeof(Sprite), false);
                        GUILayout.EndHorizontal();
                    }
                    else {
                        if (!iterator.displayName.StartsWith("Element")) {
                            EditorGUILayout.PropertyField(iterator);
                        }
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
        }

        private void DeleteConfirmDialog(string name, UnityEngine.Object objectToDelete) {
            if (EditorUtility.DisplayDialog($"Delete {name}?",
                $"Are you sure you want to delete {name}?", "Delete", "Do Not Delete")) {

                string path = AssetDatabase.GetAssetPath(objectToDelete);
                Undo.RecordObject(objectToDelete, "Delete Asset");

                if (AssetDatabase.DeleteAsset(path)) {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    Selection.activeObject = null;
                }
                else {
                    EditorUtility.DisplayDialog("Error", $"Failed to delete {name}.", "OK");
                }
            }
        }

        private void AddCreateButton(string menuName) {
            SirenixEditorGUI.BeginIndentedHorizontal();
            {
                _enemyName = EditorGUILayout.TextField("Enemy Name", _enemyName);
            }

            SirenixEditorGUI.EndIndentedHorizontal();

            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                GUILayout.FlexibleSpace();

                if (SirenixEditorGUI.ToolbarButton(new GUIContent($"Create New {menuName}"))) {
                    switch (menuName) {
                        case "Attack Data":
                            createAttackState = new CreateState<EnemyAttackState>("Attack");
                            createAttackState.CreateNewState(_enemyName);
                            Undo.RegisterCreatedObjectUndo(createAttackState.NewState, $"Create {menuName}");
                            break;
                        case "Chase Data":
                            createChaseState = new CreateState<EnemyChaseState>("Chase");
                            createChaseState.CreateNewState(_enemyName);
                            Undo.RegisterCreatedObjectUndo(createChaseState.NewState, $"Create {menuName}");
                            break;
                        case "Hit Data":
                            createHitState = new CreateState<EnemyHitState>("Hit");
                            createHitState.CreateNewState(_enemyName);
                            Undo.RegisterCreatedObjectUndo(createHitState.NewState, $"Create {menuName}");
                            break;
                        case "Idle Data":
                            createIdleState = new CreateState<EnemyIdleState>("Idle");
                            createIdleState.CreateNewState(_enemyName);
                            Undo.RegisterCreatedObjectUndo(createIdleState.NewState, $"Create {menuName}");
                            break;
                        case "Wander Data":
                            createWanderState = new CreateState<EnemyWanderState>("Wander");
                            createWanderState.CreateNewState(_enemyName);
                            Undo.RegisterCreatedObjectUndo(createWanderState.NewState, $"Create {menuName}");
                            break;
                        case "Animations":
                            createAnimationConfig = new CreateClass<AnimationConfig>("Animation");
                            createAnimationConfig.CreateNewObject(_enemyName);
                            Undo.RegisterCreatedObjectUndo(createAnimationConfig.NewObject, $"Create {menuName}");
                            break;
                        case "Health Data":
                            createHealthConfig = new CreateClass<HealthConfig>("Health");
                            createHealthConfig.CreateNewObject(_enemyName);
                            Undo.RegisterCreatedObjectUndo(createHealthConfig.NewObject, $"Create {menuName}");
                            break;
                        default:
                            break;
                    }
                    ;
                }
            }

            SirenixEditorGUI.EndHorizontalToolbar();
        }

        internal class MenuItemDetails {
            public string Description { get; }

            public MenuItemDetails(string description) {
                Description = description;
            }
        }

        internal class CreateState<T> where T : State<EnemyStateMachine> {
            [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
            public T NewState;

            private readonly string _type;

            public CreateState(string type) {
                NewState = CreateInstance<T>();
                _type = type;
                NewState.name = $"New {_type} State";
            }

            public void CreateNewState(string enemyName) {
                System.IO.Directory.CreateDirectory($"{Path}/{enemyName}/States");

                // create new instance of the SO
                NewState = CreateInstance<T>();
                NewState.name = $"{enemyName} {_type} State";

                AssetDatabase.CreateAsset(NewState, $"{Path}/{enemyName}/States/{enemyName} {_type} State.asset");
                AssetDatabase.SaveAssets();

                Selection.activeObject = NewState;
                EditorUtility.SetDirty(NewState);
                EditorGUIUtility.PingObject(NewState);
                SceneView.FrameLastActiveSceneView();
            }
        }

        internal class CreateClass<T> where T : ScriptableObject {
            [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
            public T NewObject;

            private readonly string _type;

            public CreateClass(string type) {
                NewObject = CreateInstance<T>();
                _type = type;
                NewObject.name = $"New {_type}";
            }

            public void CreateNewObject(string enemyName) {
                System.IO.Directory.CreateDirectory($"{Path}/{enemyName}");

                // create new instance of the SO
                NewObject = CreateInstance<T>();
                NewObject.name = $"{enemyName} {_type}";

                AssetDatabase.CreateAsset(NewObject, $"{Path}/{enemyName}/{enemyName} {_type}.asset");
                AssetDatabase.SaveAssets();

                Selection.activeObject = NewObject;
                EditorUtility.SetDirty(NewObject);
                EditorGUIUtility.PingObject(NewObject);
                SceneView.FrameLastActiveSceneView();
            }
        }
    }
}