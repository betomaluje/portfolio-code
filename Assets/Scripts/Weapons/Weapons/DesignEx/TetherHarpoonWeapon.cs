using Base;
using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// A localized weapon that shoots a harpoon. 
    /// If an active tether exists, recasting will pull the target instead of shooting.
    /// </summary>
    [RequiredBullet(typeof(TetherHarpoonBullet))]
    [CreateAssetMenu(fileName = "TetherHarpoonWeapon", menuName = "Aurora/Weapons/Expanded/Tether Harpoon Pistol")]
    public class TetherHarpoonWeapon : BaseShootingWeapon {
        
        // Tracks the currently attached hook so we can recast to pull
        private TetherHarpoonBullet _activeTether;

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            // Recast Logic: If we already have an active tether hooked on someone, pull them!
            if (_activeTether != null && _activeTether.IsAttached) {
                animations?.Play(AttackAnimation); // Could use a unique "Pull" string here
                _activeTether.ExecutePull();
                StartCooldown();
                return;
            }

            if (HasAmmo()) {
                animations?.Play(AttackAnimation);
                
                if (BulletPrefab != null) {
                    var bullet = Instantiate(BulletPrefab, position, Quaternion.identity);
                    if (bullet.TryGetComponent<IBullet>(out var ibullet)) {
                        ibullet.SetWeapon(this);
                        ibullet.SetOwner(animations?.Transform.root);
                        
                        // Save the reference to the active tether so we can hook it later
                        if (bullet.TryGetComponent<TetherHarpoonBullet>(out var tether)) {
                            _activeTether = tether;
                        }

                        ibullet.Fire(direction.normalized);
                    }
                }
                
                StartCooldown();
            } else {
                Reload(animations);
            }
        }

        // --- ENFORCING BULLET TYPES ---
        // By using Unity's OnValidate, we can drop the reference and warn the user
        // immediately if they drag the wrong prefab into the inspector!
    }
}
