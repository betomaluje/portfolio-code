using Base;
using UnityEngine;

namespace Weapons {
    [CreateAssetMenu(fileName = "MeleeWeapon", menuName = "Aurora/Weapons/Melee Weapon")]
    public class MeleeWeapon : Weapon, IWeaponCollider {
        [SerializeField]
        private Vector2 _attackSize = Vector2.one;
        [SerializeField]
        private Vector2 _attackOffset = Vector2.zero;

        public Vector2 AttackSize { get => _attackSize; }
        public Vector2 AttackOffset { get => _attackOffset; }

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) {
                return;
            }
            animations?.Play(AttackAnimation);
            StartCooldown();
        }
    }
}