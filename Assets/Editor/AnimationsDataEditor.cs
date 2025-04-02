using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;
using Base;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;
using DebugTools;

namespace Editor {
    public class AnimationsDataEditor : OdinMenuEditorWindow {

        private List<string> _tabs = new();
        private bool _areAnimationsVisible = true;

        [MenuItem(Shortcuts.ToolsAnimationData, false, -100)]
        private static void OpenWindow() {
            var window = GetWindow<AnimationsDataEditor>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 400);
        }

        protected override OdinMenuTree BuildMenuTree() {
            var tree = new OdinMenuTree();

            tree.AddAllAssetsAtPath("Animations", "Assets/Data", typeof(AnimationConfig), true, true);

            return tree;
        }

        protected override void OnBeginDrawEditors() {
            if (_tabs == null || _tabs.Count == 0) {
                _tabs = MenuTree.MenuItems[0].ChildMenuItems
                    .Select(x => x.Name.Replace("Animations", string.Empty).Trim())
                    .ToList();
            }

            var selected = MenuTree.Selection.FirstOrDefault();
            if (selected != null) {
                var index = _tabs.IndexOf(selected.Name.Replace("Animations", string.Empty).Trim());
                var name = _tabs[index];

                string[] guids2 = AssetDatabase.FindAssets($"{name} Controller", new[] { "Assets/Animations" });
                var path = AssetDatabase.GUIDToAssetPath(guids2.First());
                var animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
                var availableAnimations = animatorController.animationClips.Select(x => x.name).ToList();

                if (availableAnimations.Count <= 0) {
                    return;
                }

                _areAnimationsVisible = EditorGUILayout.BeginFoldoutHeaderGroup(_areAnimationsVisible, "Available Animations");
                if (_areAnimationsVisible) {
                    foreach (var animation in availableAnimations) {
                        if (EditorGUILayout.LinkButton(animation)) {
                            GUIUtility.systemCopyBuffer = animation;
                            DebugLog.Log("Copied to clipboard: " + animation);
                        }
                    }
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }
    }
}