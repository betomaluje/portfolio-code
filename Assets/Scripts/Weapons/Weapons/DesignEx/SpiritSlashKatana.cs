using Base;
using Extensions;
using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// A hybrid katana that launches crescent-shaped spirit waves on every swing.
    /// Mastery: "True Strike" bonus—hitting an enemy with both the physical blade 
    /// AND the projectile at close range deals massive combined damage.
    /// </summary>
    [RequiredBullet(typeof(CrescentWaveBullet))]
    [CreateAssetMenu(fileName = "SpiritSlashKatana", menuName = "Aurora/Weapons/Expanded/Spirit-Slash Katana")]
    public class SpiritSlashKatana : MeleeWeapon {
        
        [Header("Spirit Wave Configuration")]
        [Tooltip("The projectile to launch on every swing.")]
        [SerializeField] public GameObject BulletPrefab;
        
        [Tooltip("Damage multiplier for the wave itself (relative to base damage).")]
        [SerializeField] private float _waveDamageMult = 0.5f;

        [Tooltip("Speed of the crescent wave.")]
        [SerializeField] private float _waveSpeed = 15f;

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            // 1. Play the melee animation
            animations?.Play(AttackAnimation);

            // 2. Launch the Crescent Projectile
            if (BulletPrefab != null) {
                var wave = Instantiate(BulletPrefab, position, Quaternion.identity);
                CrescentWaveBullet waveBullet = wave.GetOrAdd<CrescentWaveBullet>();
                
                waveBullet.SetWeapon(this);
                waveBullet.SetOwner(animations?.Transform.root);
                waveBullet.SetDamage(Mathf.CeilToInt(GetDamage() * _waveDamageMult));
                waveBullet.SetSpeed(_waveSpeed);
                waveBullet.Fire(direction.normalized);
            }

            StartCooldown();
        }
    }
}
