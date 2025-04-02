using UnityEngine;

namespace Companions {
    [CreateAssetMenu(menuName = "Aurora/Companions/CompanionStats")]
    public class CompanionStats : ScriptableObject {
        public string Name;
        public int MaxHealth = 100;
    }
}