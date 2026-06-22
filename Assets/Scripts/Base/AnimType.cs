using System;
using UnityEngine;

namespace Base {
    /// <summary>
    /// Helper class to define a mapping between a human-readable animation name and its Animator hash value.
    /// Used within AnimationConfig to allow for easy editing and runtime conversion of animation states.
    /// </summary>
    [Serializable]
    public class AnimType {
        [Tooltip("If in a blend tree, use name of clip here")]
        public string name;
        public string value;

        public AnimType(string name, string value) {
            this.name = name;
            this.value = value;
        }
    }
}