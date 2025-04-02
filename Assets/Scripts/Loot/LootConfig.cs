using BerserkPixel.Utils;
using UnityEngine;

namespace Loot {
    [CreateAssetMenu(fileName = "LootConfig", menuName = "Aurora/Config/Loot")]
    public class LootConfig : ScriptableObject {
        [Range(0f, 1f)]
        public float LootChance = .5f;

        public float LootSpawnRadius = 1f;

        public WeightedListItem<Transform>[] LootObjects;

        public int LootAmount = 5;
    }
}