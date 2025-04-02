using UnityEngine;

namespace Base.MovementPrediction {
    [CreateAssetMenu(menuName = "Aurora/Config/PredictionConfig")]
    public class PredictionConfig : ScriptableObject {
        [Tooltip("Percentage chance of predicting something (ex. Direction).")]
        [Range(0f, 1f)]
        public float Chance = .85f;
    }
}