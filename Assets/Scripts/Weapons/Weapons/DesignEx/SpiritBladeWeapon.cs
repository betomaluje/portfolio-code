using Base;
using Extensions;
using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// A blade of pure spirit energy formed on the player's hand.
    /// 
    /// Standard Attack: A close-range, wide melee swing with slightly extended range.
    /// Charged Attack (ICharge): On release, projects a piercing energy beam that travels
    /// across the screen, scaling in length and damage based on charge duration.
    /// 
    /// Inspired by Goku Black's Azure Slicer / Vegito's Spirit Sword (Dragon Ball Super)
    /// and Kuwabara's Spirit Sword (YuYu Hakusho).
    /// </summary>
    [RequiredBullet(typeof(SpiritBladeBullet))]
    [CreateAssetMenu(fileName = "SpiritBlade", menuName = "Aurora/Weapons/Expanded/Spirit Blade")]
    public class SpiritBladeWeapon : MeleeWeapon, ICharge {

        [Header("Beam Configuration")]
        [Tooltip("The projectile prefab for the charged spirit beam. Must have a SpiritBladeBullet component.")]
        [SerializeField] public GameObject BulletPrefab;

        [Tooltip("Speed of the spirit beam.")]
        [SerializeField] private float _beamSpeed = 22f;

        [Tooltip("Damage multiplier of the beam relative to base weapon damage. Min charge fires at 0.5x, full charge fires at this value.")]
        [SerializeField] private float _maxBeamDamageMult = 3.0f;

        [Tooltip("Time in seconds to reach maximum charge output.")]
        [SerializeField] private float _chargeTime = 1.5f;

        [Tooltip("Scale of the beam sprite at minimum charge.")]
        [SerializeField] private float _minBeamScale = 0.6f;

        [Tooltip("Scale of the beam sprite at maximum charge.")]
        [SerializeField] private float _maxBeamScale = 2.5f;

        private float _chargeValue;

        public float Charge {
            set => _chargeValue = Mathf.Clamp01(value);
        }

        public float ChargeTime => _chargeTime;

        /// <summary>
        /// On a charged release: fires a piercing energy beam scaled to charge level.
        /// On a quick tap (no charge): performs a standard melee swing.
        /// </summary>
        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            animations?.Play(AttackAnimation);

            if (_chargeValue > 0.05f && BulletPrefab != null) {
                FireSpiritBeam(animations, direction, position);
            }

            // Reset charge after attack
            _chargeValue = 0f;
            StartCooldown();
        }

        private void FireSpiritBeam(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            var beamObj = Instantiate(BulletPrefab, position, Quaternion.identity);
            var bullet = beamObj.GetOrAdd<SpiritBladeBullet>();

            // Scale beam damage and visual size with charge
            int beamDamage = Mathf.CeilToInt(GetDamage() * Mathf.Lerp(0.5f, _maxBeamDamageMult, _chargeValue));
            float beamScale = Mathf.Lerp(_minBeamScale, _maxBeamScale, _chargeValue);

            beamObj.transform.localScale = new Vector3(beamScale, beamScale, 1f);

            bullet.SetWeapon(this);
            bullet.SetOwner(animations?.Transform.root);
            bullet.SetDamage(beamDamage);
            bullet.SetSpeed(_beamSpeed);
            bullet.Fire(direction.normalized);

            PlayImpactSound(position, "spirit_slash");
        }
    }
}
