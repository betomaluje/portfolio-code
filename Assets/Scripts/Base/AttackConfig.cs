using Sirenix.OdinInspector;
using UnityEngine;

namespace Base {
    /// <summary>
    /// ScriptableObject that holds attack configuration for an entity.
    /// Defines attack properties such as target mask, critical hit chance and multipliers, and hit pause durations.
    /// </summary>
    [CreateAssetMenu(fileName = "Attack", menuName = "Aurora/Config/Attack")]
    [InlineEditor]
    public class AttackConfig : ScriptableObject {
        [field: SerializeField]
        public LayerMask TargetMask { get; private set; }

        public float CritChance = .2f;
        public float CritMultiplier = 1.5f;

        [Tooltip("Duration in seconds for a critical hit time scale change")]
        public float CritHitTime = .4f;

        [Tooltip("Duration in seconds for a normal hit time scale change")]
        public float NormalHitTime = .05f;
    }
}