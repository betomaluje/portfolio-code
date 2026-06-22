using UnityEngine;
using Base;
using Weapons;

namespace Weapons.DesignEx {
    /// <summary>
    /// A magical grimoire that fires binding seals.
    /// Mechanics: Projectile pins a target. While pinned, a field is created 
    /// that slows and potentially binds other nearby enemies.
    /// </summary>
    [RequiredBullet(typeof(BindingSealBullet))]
    [CreateAssetMenu(fileName = "BindingGrimoire", menuName = "Aurora/Weapons/Expanded/Binding Seal Grimoire")]
    public class BindingGrimoire : BaseShootingWeapon {
        
        [Header("Seal Properties")]
        [Tooltip("How long the binding seal remains active.")]
        public float SealDuration = 4.0f;

        [Tooltip("The radius of the secondary binding field.")]
        public float SpreadRadius = 3.5f;

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            if (HasAmmo()) {
                animations?.Play(AttackAnimation);
                
                if (BulletPrefab != null) {
                    var bulletObj = Instantiate(BulletPrefab, position, Quaternion.identity);
                    if (bulletObj.TryGetComponent<BindingSealBullet>(out var seal)) {
                        seal.SetWeapon(this);
                        seal.SetOwner(animations?.Transform.root);
                        seal.InitializeSeal(SealDuration, SpreadRadius);
                        seal.Fire(direction);
                    }
                }

                StartCooldown();
            } else {
                Reload(animations);
            }
        }
    }
}
