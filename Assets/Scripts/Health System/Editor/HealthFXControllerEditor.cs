using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using UnityEditor.Callbacks;
using System.Linq;

namespace BerserkPixel.Health.FX {
    [ExecuteInEditMode]
    [CustomEditor(typeof(HealthFXController))]
    public class HealthFXControllerEditor : UnityEditor.Editor {

        private static List<Type> _enemyFXs = new(){
            typeof(BloodParticlesFX),
            typeof(EnemyBlockFX),
            typeof(FlashFX),
            typeof(SkewFX),
            typeof(KnockbackFX),
            typeof(TimeScaleFX)
        };

        private static List<Type> _allFxTypes = new();

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            // remove this if we want all types individually
            // foreach (Type fxType in _allFxTypes) {
            //     if (GUILayout.Button(fxType.Name)) {
            //         TryAddFX(fxType);
            //     }
            // }

            if (GUILayout.Button("Add All FX")) {
                foreach (Type fxType in _enemyFXs) {
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

            var currentFX = gameObject.GetComponentsInChildren<IFX>().ToList();

            if (currentFX.FirstOrDefault(f => f.GetType() == fxType) != null) return;

            var childFXContainer = gameObject.transform.Find("FXs");

            if (childFXContainer == null) {
                childFXContainer = new GameObject("FXs").transform;
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
            var filteredTypes = types.Where(t => IsOfType(t, typeof(IFX)) && !t.IsAbstract && t.IsClass);

            _allFxTypes = filteredTypes.ToList();
        }

        private static bool IsOfType(Type toCheck, Type type, bool orInherited = true) {
            return type == toCheck || (orInherited && type.IsAssignableFrom(toCheck));
        }
    }
}