using Base;
using Extensions;
using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// An evolving katana that fires crescent waves on swings, inspired by Ichigo's Zangetsu.
    ///
    /// Standard Combo (3-hit cycle):
    ///   - Swings 1 & 2: Quick physical slashes (standard MeleeWeapon attack). 
    ///   - Swing 3:       Fires a standard CrescentWaveBullet alongside the slash.
    ///
    /// Charged Attack (ICharge — Getsuga Tenshō):
    ///   Holding charge then releasing fires a massive, dark GetsugaHeavyWaveBullet
    ///   that is significantly larger, slower, and pierces many more targets.
    ///   The heavy wave deals greatly amplified damage.
    ///
    /// Inspired by Ichigo's Zangetsu / Getsuga Tenshō (Bleach).
    /// </summary>
    [RequiredBullet(typeof(GetsugaHeavyWaveBullet))]
    [CreateAssetMenu(fileName = "GetsugaKatana", menuName = "Aurora/Weapons/Expanded/Getsuga Zangetsu Katana")]
    public class GetsugaKatanaWeapon : MeleeWeapon, ICharge {

        [Header("Standard Wave")]
        [Tooltip("Standard crescent wave prefab. Fires on every 3rd swing. Must have a CrescentWaveBullet component.")]
        [SerializeField] public GameObject StandardWavePrefab;

        [Tooltip("Speed of the standard crescent wave.")]
        [SerializeField] private float _standardWaveSpeed = 14f;

        [Tooltip("Damage multiplier for the standard crescent wave.")]
        [SerializeField] private float _standardWaveDamageMult = 0.6f;

        [Header("Getsuga Tenshō — Charged Wave")]
        [Tooltip("The heavy Getsuga wave prefab. Fires on charged release. Must have a GetsugaHeavyWaveBullet component.")]
        [SerializeField] public GameObject HeavyWavePrefab;

        [Tooltip("Speed of the heavy Getsuga wave (slower for dramatic effect).")]
        [SerializeField] private float _heavyWaveSpeed = 7f;

        [Tooltip("Maximum damage multiplier for the heavy Getsuga wave at full charge.")]
        [SerializeField] private float _maxHeavyWaveDamageMult = 6.0f;

        [Tooltip("Scale multiplier of the heavy wave sprite at full charge.")]
        [SerializeField] private float _maxHeavyWaveScale = 3.0f;

        [Tooltip("Time in seconds to reach maximum charge output.")]
        [SerializeField] private float _chargeTime = 2.0f;

        private float _chargeValue;
        private int _swingCount = 0;

        public float Charge {
            set => _chargeValue = Mathf.Clamp01(value);
        }

        public float ChargeTime => _chargeTime;

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            animations?.Play(AttackAnimation);

            if (_chargeValue > 0.1f) {
                // -- GETSUGA TENSHŌ: charged release fires the heavy wave --
                FireHeavyGetsugaWave(animations, direction, position);
                // Charged attack resets the combo counter
                _swingCount = 0;
            } else {
                // -- STANDARD SWING --
                _swingCount++;

                if (_swingCount >= 3 && StandardWavePrefab != null) {
                    // Every 3rd swing, fire a standard crescent wave
                    FireStandardCrescentWave(animations, direction, position);
                    _swingCount = 0;
                }
            }

            _chargeValue = 0f;
            StartCooldown();
        }

        private void FireStandardCrescentWave(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            var waveObj = Instantiate(StandardWavePrefab, position, Quaternion.identity);
            var bullet = waveObj.GetOrAdd<CrescentWaveBullet>();

            bullet.SetWeapon(this);
            bullet.SetOwner(animations?.Transform.root);
            bullet.SetDamage(Mathf.CeilToInt(GetDamage() * _standardWaveDamageMult));
            bullet.SetSpeed(_standardWaveSpeed);
            bullet.Fire(direction.normalized);

            PlayImpactSound(position, "spirit_slash");
            DebugTools.DebugLog.Log("[GetsugaKatana] Standard crescent wave fired (3rd swing).");
        }

        private void FireHeavyGetsugaWave(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (HeavyWavePrefab == null) return;

            var waveObj = Instantiate(HeavyWavePrefab, position, Quaternion.identity);
            var bullet = waveObj.GetOrAdd<GetsugaHeavyWaveBullet>();

            // Scale size and damage with charge
            float scale = Mathf.Lerp(1.5f, _maxHeavyWaveScale, _chargeValue);
            waveObj.transform.localScale = new Vector3(scale, scale, 1f);

            int heavyDamage = Mathf.CeilToInt(GetDamage() * Mathf.Lerp(1.5f, _maxHeavyWaveDamageMult, _chargeValue));

            bullet.SetWeapon(this);
            bullet.SetOwner(animations?.Transform.root);
            bullet.SetDamage(heavyDamage);
            bullet.SetSpeed(_heavyWaveSpeed);
            bullet.Fire(direction.normalized);

            PlayImpactSound(position, "getsuga_release");
            DebugTools.DebugLog.Log($"[GetsugaKatana] GETSUGA TENSHO! Charge: {_chargeValue:P0} | Damage: {heavyDamage}");
        }
    }
}
