using Base;
using BerserkPixel.Health;
using Extensions;
using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// Rapid hand-to-hand combat weapon that builds up a Ki Gauge on successive hits.
    /// 
    /// Passive Combo: Each successful hit increments the Ki stack (up to a configurable max).
    /// Payoff: On reaching max stacks, the next swing triggers a massive golden energy burst
    ///         (DragonBurstExplosionBullet) at the hit position, dealing heavy AOE damage.
    ///
    /// Inspired by Goku's Dragon Fist (Dragon Ball GT) and One For All Detroit Smash (MHA).
    /// </summary>
    [RequiredBullet(typeof(DragonBurstExplosionBullet))]
    [CreateAssetMenu(fileName = "DragonBurstGauntlet", menuName = "Aurora/Weapons/Expanded/Dragon Burst Gauntlet")]
    public class DragonBurstGauntletWeapon : MeleeWeapon {

        [Header("Ki Stack Configuration")]
        [Tooltip("Number of consecutive hits needed to trigger the Dragon Burst.")]
        [SerializeField] private int _maxKiStacks = 3;

        [Tooltip("The explosion prefab to spawn when the Ki reaches max stacks. Must have a DragonBurstExplosionBullet component.")]
        [SerializeField] public GameObject BulletPrefab;

        [Tooltip("Damage multiplier of the burst explosion relative to base weapon damage.")]
        [SerializeField] private float _burstDamageMult = 4.0f;

        [Tooltip("If no new hit is registered within this window, the Ki stacks reset to 0.")]
        [SerializeField] private float _comboWindowSeconds = 2.5f;

        private int _currentKiStacks = 0;
        private float _comboTimer = 0f;
        private bool _isReadyToBurst = false;

        // Track the last hit position to spawn the burst there
        private Vector3 _lastHitPosition;

        private void OnEnable() {
            CharacterHealth.OnAnyDamagePerformed += HandleAnyDamagePerformed;
        }

        private void OnDisable() {
            CharacterHealth.OnAnyDamagePerformed -= HandleAnyDamagePerformed;
        }

        private void OnDestroy() {
            CharacterHealth.OnAnyDamagePerformed -= HandleAnyDamagePerformed;
        }

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            // Tick down the combo window
            TickComboWindow();

            animations?.Play(AttackAnimation);
            StartCooldown();
        }

        /// <summary>
        /// Called each time any character in the scene takes damage.
        /// Filters to only react when THIS weapon lands a hit.
        /// </summary>
        private void HandleAnyDamagePerformed(HitData data) {
            if (data.weapon != this || data.attacker == null) return;

            _lastHitPosition = data.victim != null ? data.victim.position : data.attacker.position;
            _comboTimer = _comboWindowSeconds; // reset window on successful hit

            _currentKiStacks++;

            DebugTools.DebugLog.Log($"[DragonBurst] Ki Stack: {_currentKiStacks}/{_maxKiStacks}");

            if (_currentKiStacks >= _maxKiStacks) {
                TriggerDragonBurst(data.attacker);
            }
        }

        private void TriggerDragonBurst(Transform attacker) {
            _currentKiStacks = 0;
            _isReadyToBurst = false;

            if (BulletPrefab == null) return;

            var burstObj = Instantiate(BulletPrefab, _lastHitPosition, Quaternion.identity);
            var burst = burstObj.GetOrAdd<DragonBurstExplosionBullet>();

            burst.SetWeapon(this);
            burst.SetOwner(attacker);
            burst.SetDamage(Mathf.CeilToInt(GetDamage() * _burstDamageMult));
            burst.Fire(Vector2.zero); // stationary — direction unused

            PlayImpactSound(_lastHitPosition, "dragon_burst");
            DebugTools.DebugLog.Log($"[DragonBurst] BURST triggered at {_lastHitPosition}!");
        }

        /// <summary>
        /// Updates the combo window timer to drop stacks if hits have stalled.
        /// Called at the start of each Attack() since weapons are ScriptableObjects
        /// and cannot use MonoBehaviour Update.
        /// </summary>
        private void TickComboWindow() {
            if (_comboTimer > 0f) {
                _comboTimer -= AttackCooldown;

                if (_comboTimer <= 0f) {
                    _currentKiStacks = 0;
                    DebugTools.DebugLog.Log("[DragonBurst] Combo window expired. Ki reset.");
                }
            }
        }

        /// <summary>
        /// Public accessor so UI can display Ki stack count if needed.
        /// </summary>
        public int CurrentKiStacks => _currentKiStacks;
        public int MaxKiStacks => _maxKiStacks;
    }
}
