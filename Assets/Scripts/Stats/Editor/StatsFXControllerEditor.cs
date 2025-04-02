using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using UnityEditor.Callbacks;
using System.Linq;

namespace Stats {
    [ExecuteInEditMode]
    [CustomEditor(typeof(StatsFXController))]
    public class StatsFXControllerEditor : UnityEditor.Editor {
        private static List<Type> _allFxTypes = new();

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if (GUILayout.Button("Add All FX")) {
                foreach (Type fxType in _allFxTypes) {
                    TryAddFX(fxType);
                }
            }
        }

        private void TryAddFX(Type fxType) {
            var gameObject = Selection.activeGameObject;
            if (gameObject == null) {
                return;
            }

            if (fxType == null) return;

            var currentFX = gameObject.GetComponentsInChildren<IStatFX>().ToList();

            if (currentFX.FirstOrDefault(f => f.GetType() == fxType) != null) return;

            var childFXContainer = gameObject.transform.Find("Stat FXs");

            if (childFXContainer == null) {
                childFXContainer = new GameObject("Stat FXs").transform;
                childFXContainer.SetParent(gameObject.transform);
            }

            DebugTools.DebugLog.Log($"{fxType} Added!");
            childFXContainer.gameObject.AddComponent(fxType);

            EditorUtility.SetDirty(gameObject);
        }

        [DidReloadScripts]
        private static void OnScriptsReloaded() {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies.SelectMany(a => a.GetTypes());
            var filteredTypes = types.Where(t => IsOfType(t, typeof(IStatFX)) && !t.IsAbstract && t.IsClass);

            _allFxTypes = filteredTypes.ToList();
        }

        private static bool IsOfType(Type toCheck, Type type, bool orInherited = true) {
            return type == toCheck || (orInherited && type.IsAssignableFrom(toCheck));
        }
    }
}