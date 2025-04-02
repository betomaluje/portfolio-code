using UnityEngine;

namespace NPCs {
    [CreateAssetMenu(menuName = "Aurora/NPC/Characteristics")]
    public class NPCCharacteristics : ScriptableObject {
        public int MaxHealth = 100;

        [Header("Common Characteristics")]
        [Range(0f, 1f)]
        public float friendliness = 0.5f;

        [Range(0f, 1f)]
        public float scariness = 0.5f;

        [Header("Additional Characteristics")]
        public float agility = 0.5f;

        public float CalculateRescueChance() {
            // Calculate the rescue chance based on characteristics
            // You can adjust the formula based on your game's logic
            var rescueChance = friendliness - scariness;
            return Mathf.Clamp01(rescueChance); // Clamp between 0 and 1
        }
    }
}