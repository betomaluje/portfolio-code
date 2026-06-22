using UnityEngine;
using Weapons;
using Base;

namespace Weapons.DesignEx {
    /// <summary>
    /// A throwing hammer that returns to the owner.
    /// Mechanics: Thrown at high speed. While returning, it connects a lightning chain 
    /// to the owner, damaging anything caught in the return path.
    /// </summary>
    [RequiredBullet(typeof(MjolnirHammerBullet))]
    [CreateAssetMenu(fileName = "MjolnirHammer", menuName = "Aurora/Weapons/Expanded/Mjolnir Hammer")]
    public class MjolnirHammerWeapon : BaseShootingWeapon {
        
        [Header("Lightning Properties")]
        [Tooltip("Damage dealt by the electricity chain during return.")]
        [SerializeField] private float _chainDamageMultiplier = 0.5f;

        [Tooltip("The max distance the hammer can travel before auto-returning.")]
        [SerializeField] private float _maxTravelDistance = 15f;

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            if (HasAmmo()) {
                animations?.Play(AttackAnimation);
                
                if (BulletPrefab != null) {
                    var obj = Instantiate(BulletPrefab, position, Quaternion.identity);
                    if (obj.TryGetComponent<MjolnirHammerBullet>(out var hammer)) {
                        hammer.SetWeapon(this);
                        hammer.SetOwner(animations?.Transform.root);
                        hammer.InitializeHammer(_chainDamageMultiplier, _maxTravelDistance);
                        hammer.Fire(direction);
                    }
                }

                StartCooldown();
            } else {
                Reload(animations);
            }
        }
    }
}
