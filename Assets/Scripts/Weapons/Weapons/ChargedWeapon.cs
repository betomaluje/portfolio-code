using Base;
using UnityEngine;

namespace Weapons {
    /// <summary>
    /// A shooting weapon that modifies its attack characteristics based on a charge duration.
    /// Implements ICharge to receive the player's charging input.
    /// High charge can mean more damage, faster projectiles, or larger bullets.
    /// </summary>
    [CreateAssetMenu(fileName = "ChargedWeapon", menuName = "Aurora/Weapons/Charged Weapon")]
    public class ChargedWeapon : BaseShootingWeapon, ICharge {
        [Tooltip("Damage multiplier at maximum charge (1.0 = normal damage).")]
        [SerializeField] private float _maxChargeDamageMult = 2.0f;

        [Tooltip("Speed multiplier at maximum charge.")]
        [SerializeField] private float _maxChargeSpeedMult = 1.5f;

        [Tooltip("The visual scale of the projectile at maximum charge.")]
        [SerializeField] private float _maxChargeSizeScale = 2.0f;

        [Tooltip("Time in seconds to reach maximum charge output.")]
        [SerializeField] private float _chargeTime = 2.0f;

        private float _chargeValue;
        
        /// <summary>
        /// Property required by the ICharge interface. 
        /// Injected by the WeaponManager during the charging sequence.
        /// </summary>
        public float Charge {
            set => _chargeValue = Mathf.Clamp01(value);
        }

        public float ChargeTime => _chargeTime;

        /// <summary>
        /// Executes the attack with parameters modulated by the current charge level.
        /// </summary>
        /// <param name="animations">The character's animations.</param>
        /// <param name="direction">The aim direction.</param>
        /// <param name="position">Starting world position of the bullet.</param>
        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            if (HasAmmo()) {
                animations?.Play(AttackAnimation);
                
                ExecuteChargedShoot(position, direction, animations?.Transform);
                StartCooldown();
                
                _chargeValue = 0f; // Reset charge after firing
            } else {
                Reload(animations);
            }
        }

        /// <summary>
        /// Custom bullet spawning that modifies the resulting projectile components.
        /// </summary>
        /// <param name="position">Firing point.</param>
        /// <param name="direction">Direction of flight.</param>
        /// <param name="owner">Shooting character.</param>
        private void ExecuteChargedShoot(Vector3 position, Vector3 direction, Transform owner) {
            if (BulletPrefab == null) return;

            GameObject bullet = Instantiate(BulletPrefab, position, Quaternion.identity);
            
            // Adjust scale based on charge
            float scaleMod = Mathf.Lerp(1.0f, _maxChargeSizeScale, _chargeValue);
            bullet.transform.localScale *= scaleMod;

            if (bullet.TryGetComponent<IBullet>(out var ibullet)) {
                ibullet.SetWeapon(this);
                ibullet.SetOwner(owner);

                // Modulate damage
                int baseDamage = GetDamage();
                int modulatedDamage = Mathf.CeilToInt(baseDamage * Mathf.Lerp(1.0f, _maxChargeDamageMult, _chargeValue));
                ibullet.SetDamage(modulatedDamage);

                // Modulate speed
                float modulatedSpeed = ibullet.GetSpeed() * Mathf.Lerp(1.0f, _maxChargeSpeedMult, _chargeValue);
                ibullet.SetSpeed(modulatedSpeed);

                ibullet.Fire(direction.normalized);
            }
            
            // Note: We don't decrement ammo here because the weapon might require 1 ammo per shot regardless of charge.
            // Or if you want Max Charge to consume more ammo, you can do it here.
        }
    }
}
