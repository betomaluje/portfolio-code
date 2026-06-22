using Base;
using UnityEngine;

namespace Weapons {
    /// <summary>
    /// A localized melee weapon that performs a quick forward "dash" when attacking.
    /// This helps the player close distances and strike enemies at once.
    /// </summary>
    [CreateAssetMenu(fileName = "DashMeleeWeapon", menuName = "Aurora/Weapons/Dash Melee Weapon")]
    public class DashMeleeWeapon : MeleeWeapon {
        [Tooltip("The amount of force to dash forward during the attack.")]
        [SerializeField] private float _dashForceSize = 25f;

        [Tooltip("Force mode for the dash impulse.")]
        [SerializeField] private ForceMode2D _dashMode = ForceMode2D.Impulse;

        /// <summary>
        /// Executes a melee strike while propelling the character in the attack direction.
        /// </summary>
        /// <param name="animations">Character animations controller.</param>
        /// <param name="direction">Aim direction to dash towards.</param>
        /// <param name="position">Starting world position of the character.</param>
        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            animations?.Play(AttackAnimation);
            
            // Execute the dash propel
            ApplyDash(animations?.Transform.root, direction);
            
            StartCooldown();
        }

        /// <summary>
        /// Pushes the character's Rigidbody2D in the directed forward vector.
        /// </summary>
        /// <param name="owner">Shooting character transform.</param>
        /// <param name="direction">The direction of the dash.</param>
        private void ApplyDash(Transform owner, Vector2 direction) {
            if (owner == null) return;

            if (owner.TryGetComponent<Rigidbody2D>(out var rb)) {
                // We add force directly in the aim direction
                rb.AddForce(direction.normalized * _dashForceSize, _dashMode);
            }
        }
    }
}
