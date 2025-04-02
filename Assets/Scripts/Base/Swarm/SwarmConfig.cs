using System.Linq;
using BerserkPixel.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Base.Swarm {
    public class SwarmConfig<T> : ScriptableObject {
        [Min(0)]
        public int AmountToSpawn = 4;

        public WeightedListItem<T>[] Prefabs;

        [BoxGroup("Debug", order: 100)]
        [Button("Arrange Prefabs")]
        private void ArrangePrefabs() {
            if (Prefabs != null) {
                var currentOrder = Prefabs.OrderByDescending(prefab => prefab._weight).ThenBy(prefab => prefab._item.ToString());
                if (!currentOrder.SequenceEqual(Prefabs)) {
                    Prefabs = currentOrder.ToArray();
                }
            }
        }
    }
}