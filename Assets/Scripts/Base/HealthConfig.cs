using Sirenix.OdinInspector;
using UnityEngine;

namespace Base {
    [CreateAssetMenu(fileName = "Health", menuName = "Aurora/Config/Health")]
    [InlineEditor]
    public class HealthConfig : ScriptableObject {
        [field: SerializeField]
        public int MaxHealth { get; private set; }

        [field: SerializeField]
        public int ExperienceToGive { get; private set; }

        private void OnValidate() {
            if (MaxHealth > 0 && ExperienceToGive < 0) {
                ExperienceToGive = Mathf.RoundToInt(MaxHealth / 2f);
            }
        }
    }
}