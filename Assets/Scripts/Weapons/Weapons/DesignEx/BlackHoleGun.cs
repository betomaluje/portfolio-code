using UnityEngine;
using Weapons;
using Base;

namespace Weapons.DesignEx {
    /// <summary>
    /// A high-tech black hole gun, inspired by Enter the Gungeon.
    /// Mechanics: Fires a massive, slow-moving black hole that attracts 
    /// enemies and projectiles in a radius. Great for grouping targets.
    /// </summary>
    [RequiredBullet(typeof(GravityWellBullet))]
    [CreateAssetMenu(fileName = "BlackHoleGun", menuName = "Aurora/Weapons/Expanded/Black Hole Gun")]
    public class BlackHoleGun : BaseShootingWeapon {
        
        [Header("Singularity Stats")]
        [Tooltip("The pulling force of the black hole.")]
        [SerializeField] private float _attractionForce = 8.0f;
        
        [Tooltip("The attraction radius.")]
        [SerializeField] private float _attractionRadius = 6.0f;

        [Tooltip("How long the black hole stays active before it collapses.")]
        [SerializeField] private float _lifespan = 4.0f;

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            if (HasAmmo()) {
                animations?.Play(AttackAnimation);
                
                if (BulletPrefab != null) {
                    var obj = Instantiate(BulletPrefab, position, Quaternion.identity);
                    if (obj.TryGetComponent<GravityWellBullet>(out var singularity)) {
                        singularity.SetWeapon(this);
                        singularity.InitializeSingularity(_attractionForce, _attractionRadius, _lifespan);
                        singularity.Fire(direction);
                    }
                }

                StartCooldown();
            } else {
                Reload(animations);
            }
        }
    }
}
