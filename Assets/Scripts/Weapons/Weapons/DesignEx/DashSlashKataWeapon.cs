using Base;
using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// Uniquely implements an internal "Stamina/Charge" model inside a Melee Weapon.
    /// Provides 3 rapid dashes that slowly recharge, completely distinct from standard DashMeleeWeapon cooldowns.
    /// </summary>
    [CreateAssetMenu(fileName = "DashSlashKataWeapon", menuName = "Aurora/Weapons/Expanded/Dash Slash Kata")]
    public class DashSlashKataWeapon : MeleeWeapon {
        
        [Header("Kata Dash Mechanics")]
        [Tooltip("How fast the player dashes.")]
        [SerializeField] private float _kataDashSpeed = 25f;

        [Tooltip("Maximum stamina charges before the weapon runs out of dashes.")]
        [SerializeField] [Min(1)] private int _maxDashCharges = 3;

        [Tooltip("How long it takes for a SINGLE dash charge to refill (ignores global AttackCooldown).")]
        [SerializeField] private float _chargeRefillTime = 2.0f;

        // Runtime Tracking
        private int _currentCharges;
        private float _lastChargeTime;

        private void OnEnable() {
            _currentCharges = _maxDashCharges;
            _lastChargeTime = 0f;
        }

        /// <summary>
        /// Overrides IsCoolingDown to use our custom stamina system instead of the global `_nextFireTime`.
        /// </summary>
        public new bool IsCoolingDown() {
            // First, process passive refill
            if (_currentCharges < _maxDashCharges && Time.time >= _lastChargeTime + _chargeRefillTime) {
                _currentCharges++;
                _lastChargeTime = Time.time;
            }

            // Weapon is cooling down if we are completely empty
            return _currentCharges <= 0;
        }

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return; // Fails out if no charges

            // Consume a charge
            _currentCharges--;
            
            // If we just consumed the very first charge from full, start the refill timer
            if (_currentCharges == _maxDashCharges - 1) {
                _lastChargeTime = Time.time;
            }

            animations?.Play(AttackAnimation);
            
            // Execute the sharp dash
            Transform owner = animations?.Transform.root;
            if (owner != null && owner.TryGetComponent<Rigidbody2D>(out var rb)) {
                // Kata dash should be sharp and instantaneous
                rb.linearVelocity = Vector2.zero; // Clear inertia
                rb.AddForce(direction.normalized * _kataDashSpeed, ForceMode2D.Impulse);
            }
        }

        /// <summary>
        /// Mastery Hook: Designed to be called by your Hit Detection script or Enemy OnDeath event
        /// If the player kills an enemy with this weapon, instantly refund a charge to let them keep chaining!
        /// </summary>
        public void RefundCharge() {
            _currentCharges = Mathf.Clamp(_currentCharges + 1, 0, _maxDashCharges);
            if (_currentCharges == _maxDashCharges) {
                _lastChargeTime = 0f; // Done recharging
            }
        }
    }
}
