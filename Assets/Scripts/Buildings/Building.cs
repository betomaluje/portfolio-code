using System;
using UnityEngine;

namespace Buildings {
    [CreateAssetMenu(fileName = "Building", menuName = "Aurora/Building", order = 0)]
    public class Building : ScriptableObject {
        [field: SerializeField]
        public string Name { get; private set; }

        [field: SerializeField]
        public Sprite Sprite { get; private set; }

        [field: SerializeField]
        public string Description { get; private set; }

        [field: SerializeField]
        public int Cost { get; private set; }

        public override bool Equals(object building) {
            var other = building as Building;

            return Equals(other);
        }

        public override int GetHashCode() {
            return HashCode.Combine(base.GetHashCode(), Name, Sprite, Description, Cost);
        }

        private bool Equals(Building other) {
            return other != null &&
                   string.Compare(Name, other.Name, StringComparison.CurrentCulture) == 0 &&
                   Cost == other.Cost;
        }
    }
}