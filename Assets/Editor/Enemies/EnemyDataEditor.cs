using System.IO;
using System.Linq;
using System.Text;
using Base;
using BerserkPixel.StateMachine;
using Enemies;
using Enemies.States;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class EnemyDataEditor : OdinMenuEditorWindow
    {
        private const string Path = "Assets/Data/Enemies";

        private CreateState<EnemyAttackState> createAttackState;
        private CreateState<EnemyChaseState> createChaseState;
        private CreateState<EnemyHitState> createHitState;
        private CreateState<EnemyIdleState> createIdleState;
        private CreateState<EnemyWanderState> createWanderState;
        private CreateClass<AnimationConfig> createAnimationConfig;
        private CreateClass<HealthConfig> createHealthConfig;

        private string _enemyName = "Enemy";

        [MenuItem(Shortcuts.ToolsEnemyData, false, -100)]
        private static void OpenWindow()
        {
            var window = GetWindow<EnemyDataEditor>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 500);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (createAttackState != null)
            {
                DestroyImmediate(createAttackState.NewState);
            }

            if (createChaseState != null)
            {
                DestroyImmediate(createChaseState.NewState);
            }

            if (createHitState != null)
            {
                DestroyImmediate(createHitState.NewState);
            }

            if (createIdleState != null)
            {
                DestroyImmediate(createIdleState.NewState);
            }

            if (createWanderState != null)
            {
                DestroyImmediate(createWanderState.NewState);
            }

            if (createAnimationConfig != null)
            {
                DestroyImmediate(createAnimationConfig.NewObject);
            }

            if (createHealthConfig != null)
            {
                DestroyImmediate(createHealthConfig.NewObject);
            }
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new();

            tree.AddAllAssetsAtPath("Attack Data", Path, typeof(EnemyAttackState), true, true);
            tree.AddAllAssetsAtPath("Chase Data", Path, typeof(EnemyChaseState), true, true);
            tree.AddAllAssetsAtPath("Hit Data", Path, typeof(EnemyHitState), true, true);
            tree.AddAllAssetsAtPath("Idle Data", Path, typeof(EnemyIdleState), true, true);
            tree.AddAllAssetsAtPath("Wander Data", Path, typeof(EnemyWanderState), true, true);
            tree.AddAllAssetsAtPath("Animations", Path, typeof(AnimationConfig), true, true).AddIcons(EditorIcons.Male);
            tree.AddAllAssetsAtPath("Health Data", Path, typeof(HealthConfig), true, true).AddIcons(EditorIcons.Plus);
            tree.AddAllAssetsAtPath("Weapons", Path, typeof(Weapons.Weapon), true, true).AddIcons(EditorIcons.Crosshair);

            tree.SortMenuItemsByName();

            return tree;
        }

        protected override void OnBeginDrawEditors()
        {
            OdinMenuTreeSelection selected = MenuTree.Selection;

            if (selected.SelectedValue != null)
            {
                // delete current
                if (selected.SelectedValue is State<EnemyStateMachine> stateToDelete)
                {
                    AddDeleteButton(stateToDelete);
                }
                else if (selected.SelectedValue is ScriptableObject soToDelete)
                {
                    AddDeleteButton(soToDelete);

                    if (soToDelete is AnimationConfig animationConfig)
                    {

                        StringBuilder sb = new();

                        foreach (var a in animationConfig.InspectorAnimations)
                        {
                            // check in the animations folder if animation exists
                            bool exists = CheckAnimationClipExists(a.value, System.IO.Path.Combine(Application.dataPath, "Animations/Enemies"));

                            if (!exists)
                            {
                                sb.Append($"{a.value}, ");
                            }
                        }

                        if (sb.Length > 0)
                        {
                            SirenixEditorGUI.InfoMessageBox($"Apparently the following animations do not exist:\n<b>{sb}</b>");
                        }
                    }
                }
            }
            else
            {
                // create new
                var bannedNames = new[] { "Weapons" };
                foreach (var item in MenuTree.MenuItems)
                {
                    if (!bannedNames.Contains(item.Name) && item.IsSelected)
                    {
                        AddCreateButton(item.Name);
                        break;
                    }
                }
            }
        }

        private bool CheckAnimationClipExists(string clipName, string searchDirectory)
        {
            // Ensure the search directory exists
            if (!Directory.Exists(searchDirectory))
            {
                return false;
            }

            // Define the file path to look for the animation clip with .anim extension
            string[] files = Directory.GetFiles(searchDirectory, $"{clipName}.anim", SearchOption.AllDirectories);

            // If any matching files are found, return true
            return files.Length > 0;
        }

        private void AddDeleteButton(State<EnemyStateMachine> stateToDelete)
        {
            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                GUILayout.FlexibleSpace();

                if (SirenixEditorGUI.ToolbarButton(new GUIContent($"Delete {stateToDelete.name}")))
                {
                    DeleteConfirmDialog(stateToDelete.name, stateToDelete);
                }
            }

            SirenixEditorGUI.EndHorizontalToolbar();
        }

        private void AddDeleteButton(ScriptableObject soToDelete)
        {
            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                GUILayout.FlexibleSpace();

                if (SirenixEditorGUI.ToolbarButton(new GUIContent($"Delete {soToDelete.name}")))
                {
                    DeleteConfirmDialog(soToDelete.name, soToDelete);
                }
            }

            SirenixEditorGUI.EndHorizontalToolbar();
        }

        private void DeleteConfirmDialog(string name, Object objectToDelete)
        {
            if (EditorUtility.DisplayDialog($"Delete {name}?",
                $"Are you sure you want to delete {name}?", "Delete", "Do Not Delete"))
            {

                string path = AssetDatabase.GetAssetPath(objectToDelete);
                Undo.RecordObject(objectToDelete, "Delete Asset");

                if (AssetDatabase.DeleteAsset(path))
                {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    Selection.activeObject = null;
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", $"Failed to delete {name}.", "OK");
                }
            }
        }

        private void AddCreateButton(string menuName)
        {
            SirenixEditorGUI.BeginIndentedHorizontal();
            {
                _enemyName = EditorGUILayout.TextField("Enemy Name", _enemyName);
            }

            SirenixEditorGUI.EndIndentedHorizontal();

            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                GUILayout.FlexibleSpace();

                if (SirenixEditorGUI.ToolbarButton(new GUIContent($"Create New {menuName}")))
                {
                    switch (menuName)
                    {
                        case "Attack Data":
                            createAttackState = new CreateState<EnemyAttackState>("Attack");
                            createAttackState.CreateNewState(_enemyName);
                            break;
                        case "Chase Data":
                            createChaseState = new CreateState<EnemyChaseState>("Chase");
                            createChaseState.CreateNewState(_enemyName);

                            break;
                        case "Hit Data":
                            createHitState = new CreateState<EnemyHitState>("Hit");
                            createHitState.CreateNewState(_enemyName);
                            break;
                        case "Idle Data":
                            createIdleState = new CreateState<EnemyIdleState>("Idle");
                            createIdleState.CreateNewState(_enemyName);
                            break;
                        case "Wander Data":
                            createWanderState = new CreateState<EnemyWanderState>("Wander");
                            createWanderState.CreateNewState(_enemyName);
                            break;
                        case "Animations":
                            createAnimationConfig = new CreateClass<AnimationConfig>("Animations");
                            createAnimationConfig.CreateNewObject(_enemyName);
                            break;
                        case "Health Data":
                            createHealthConfig = new CreateClass<HealthConfig>("Health");
                            createHealthConfig.CreateNewObject(_enemyName);
                            break;
                        default:
                            break;
                    }
                    ;
                }
            }

            SirenixEditorGUI.EndHorizontalToolbar();
        }

        public class CreateState<T> where T : State<EnemyStateMachine>
        {
            [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
            public T NewState;

            private readonly string _type;

            public CreateState(string type)
            {
                NewState = CreateInstance<T>();
                _type = type;
                NewState.name = $"New {_type} State";
            }

            public void CreateNewState(string enemyName)
            {
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

                Undo.RegisterCreatedObjectUndo(NewState, $"Create {enemyName}");
            }
        }

        internal class CreateClass<T> where T : ScriptableObject
        {
            [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
            public T NewObject;

            private readonly string _type;

            public CreateClass(string type)
            {
                NewObject = CreateInstance<T>();
                _type = type;
                NewObject.name = $"New {_type}";
            }

            public void CreateNewObject(string enemyName)
            {
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

                Undo.RegisterCreatedObjectUndo(NewObject, $"Create {enemyName}");
            }
        }
    }

}