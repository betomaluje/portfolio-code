using UnityEngine;

namespace Player {
    [CreateAssetMenu(fileName = "Movement", menuName = "Aurora/Config/Movement")]
    public class MovementConfig : ScriptableObject {
        [Range(0, 20)]
        public float Speed = 10f;

        [Header("Roll Config")]
        [Range(1, 26)]
        public float RollSpeed = 14f;

        public float RollDuration = .8f;
    }
}