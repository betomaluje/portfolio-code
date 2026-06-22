using Base;
using UnityEngine;

namespace Weapons {
    /// <summary>
    /// A high-powered shooting weapon that applies a reactive force to the shooter upon firing.
    /// This "recoil" push can be used for mobility, repositioning, or just as a drawback.
    /// </summary>
    [CreateAssetMenu(fileName = "RecoilWeapon", menuName = "Aurora/Weapons/Recoil Weapon")]
    public class RecoilWeapon : BaseShootingWeapon {
        [Tooltip("Force of the recoil push in the opposite direction of the shot.")]
        [SerializeField] private float _recoilForce = 15f;

        [Tooltip("The mode of the recoil force. Impulse is recommended for immediate feedback.")]
        [SerializeField] private ForceMode2D _recoilMode = ForceMode2D.Impulse;

        /// <summary>
        /// Fires a projectile and applies a knockback effect to the owner's Rigidbody2D.
        /// </summary>
        /// <param name="animations">Character animation controller.</param>
        /// <param name="direction">Fire direction.</param>
        /// <param name="position">Projectile starting position.</param>
        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            if (HasAmmo()) {
                animations?.Play(AttackAnimation);
                
                // Fire the projectile
                ShootBullet(position, direction, animations?.Transform);
                
                // Apply the recoil push
                ApplyRecoil(animations?.Transform.root, direction);
                
                StartCooldown();
            } else {
                Reload(animations);
            }
        }

        /// <summary>
        /// Pushes the shooter transform in the opposite direction of the shot.
        /// Requires a Rigidbody2D component on the owner's transform or its children.
        /// </summary>
        /// <param name="owner">Attacking character transform.</param>
        /// <param name="direction">The direction the shot was fired toward.</param>
        private void ApplyRecoil(Transform owner, Vector2 direction) {
            if (owner == null) return;

            // Attempt to find Rigidbody2D
            if (owner.TryGetComponent<Rigidbody2D>(out var rb)) {
                // We add force in the opposite direction of the shot
                Vector2 recoilDir = -direction.normalized;
                rb.AddForce(recoilDir * _recoilForce, _recoilMode);
                
                // Optionally flip the sprite to look slightly toward the recoil direction 
                // but usually the character keeps facing the fire direction for consistency.
            }
        }
    }
}
