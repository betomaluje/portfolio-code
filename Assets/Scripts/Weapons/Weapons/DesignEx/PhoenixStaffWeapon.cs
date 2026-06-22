using UnityEngine;
using Weapons;
using Base;

namespace Weapons.DesignEx {
    /// <summary>
    /// A phoenix staff that fires fireballs and a giant returning phoenix.
    /// Mechanics: Fires small fireballs (BulletPrefab). If you hold the fire button 
    /// (Charge, ICharge), it fires the Phoenix that travels through enemies and 
    /// returns, healing the player on return.
    /// </summary>
    [RequiredBullet(typeof(PhoenixProjectile))]
    [CreateAssetMenu(fileName = "PhoenixStaff", menuName = "Aurora/Weapons/Expanded/Phoenix Staff")]
    public class PhoenixStaffWeapon : BaseShootingWeapon, ICharge {
        
        [Header("Phoenix Charge Properties")]
        [Tooltip("How much charge is needed to unleash the phoenix.")]
        [SerializeField] private float _minPhoenixCharge = 0.8f;
        
        [Tooltip("The phoenix projectile (high damage, returns).")]
        [SerializeField] private GameObject _phoenixPrefab;
        
        [Tooltip("Healing amount received when the phoenix returns to you.")]
        [SerializeField] private int _returnHealing = 10;

        [Tooltip("Time in seconds to reach maximum charge output.")]
        [SerializeField] private float _chargeTime = 2.0f;

        private float _chargeValue;

        public float Charge {
            set => _chargeValue = Mathf.Clamp01(value);
        }

        public float ChargeTime => _chargeTime;

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            if (HasAmmo()) {
                animations?.Play(AttackAnimation);
                
                if (_chargeValue >= _minPhoenixCharge && _phoenixPrefab != null) {
                    FirePhoenix(animations?.Transform.root, direction, position);
                    _chargeValue = 0f;
                } else {
                    ShootBullet(position, direction, animations?.Transform.root);
                }

                StartCooldown();
            } else {
                Reload(animations);
            }
        }

        private void FirePhoenix(Transform player, Vector2 direction, Vector3 position) {
            var obj = Instantiate(_phoenixPrefab, position, Quaternion.identity);
            if (obj.TryGetComponent<PhoenixProjectile>(out var phoenix)) {
                phoenix.SetWeapon(this);
                phoenix.SetOwner(player);
                phoenix.InitializePhoenix(_returnHealing);
                phoenix.Fire(direction);
            }
            
            PlayImpactSound(position, "phoenix_scream");
        }

        public void HandleReturnHealing(Transform player) {
            if (player.TryGetComponent<BerserkPixel.Health.CharacterHealth>(out var health)) {
                // heal it
                // health.Heal(_returnHealing);
                // PlayHealingVFX(player.position);
            }
            PlayImpactSound(player.position, "phoenix_heal_resonance");
        }
    }
}
