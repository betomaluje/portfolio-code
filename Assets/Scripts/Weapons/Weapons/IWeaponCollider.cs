using UnityEngine;

namespace Weapons {
    interface IWeaponCollider {
        public Vector2 AttackSize { get; }
        public Vector2 AttackOffset { get; }
    }
}