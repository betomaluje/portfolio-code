using System.Collections.Generic;
using Base.Swarm;
using BerserkPixel.Health;
using BerserkPixel.Utils;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Enemies {
    [InlineEditor]
    [CreateAssetMenu(menuName = "Aurora/Level/Enemy Swarm Config")]
    public class EnemySwarmConfig : SwarmConfig<CharacterHealth> {
        [Min(0)]
        public int AmountOfWaves = 1;

        [Header("Spawn FX")]
        [Space]
        public Transform AppearFxPrefab;

        public float TimeForSpawnEnemy = 1.5f;

        public Vector3 FxRotation = new(-90, 0, 0);

#if UNITY_EDITOR
        private const string PrefabsPath = "Assets/Prefabs/Enemies";

        [BoxGroup("Debug", order: 99)]
        [Button("Add All Enemies")]
        private void AddAllEnemies() {
            List<WeightedListItem<CharacterHealth>> _allAvailablePrefabs = new();
            var prefabsGuids = AssetDatabase.FindAssets("t:Prefab", new[] { PrefabsPath });
            foreach (var guid in prefabsGuids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null && prefab.TryGetComponent<CharacterHealth>(out var health)) {
                    var item = new WeightedListItem<CharacterHealth>(health, 1);
                    _allAvailablePrefabs.Add(item);
                }
            }
            Prefabs = _allAvailablePrefabs.ToArray();
        }
#endif
    }
}