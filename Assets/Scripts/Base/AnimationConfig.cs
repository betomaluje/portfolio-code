using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Base {
    [CreateAssetMenu(fileName = "Animations", menuName = "Aurora/Config/Animations")]
    [InlineEditor]
    public class AnimationConfig : ScriptableObject {
        [SerializeField]
        private List<AnimType> inspectorAnimations = new()
        {
            new AnimType("Idle", string.Empty),
            new AnimType("Run", string.Empty),
            new AnimType("Attack", string.Empty),
            new AnimType("Death", string.Empty)
        };

        public Dictionary<string, int> Animations = new();

        public List<AnimType> InspectorAnimations => inspectorAnimations;

        private void OnEnable() {
            Animations.Clear();
            foreach (var inspectorAnimation in inspectorAnimations) {
                if (!string.IsNullOrEmpty(inspectorAnimation.value)) {
                    Animations.Add(inspectorAnimation.name, Animator.StringToHash(inspectorAnimation.value));
                }
            }
        }

        public bool GetAnimation(string toPlay, out int animation) {
            return Animations.TryGetValue(toPlay, out animation);
        }
    }
}