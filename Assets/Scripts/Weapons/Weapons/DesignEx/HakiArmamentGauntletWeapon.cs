using Base;
using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// A gauntlet imbued with Armament Haki, cycling between an active Haki phase
    /// (massively increased damage and slightly extended reach) and a Recharge phase.
    /// The weapon is always usable — only the damage output changes with the phase.
    /// Mastery: Learn the rhythm of the cycle to land charged swings during the Haki window.
    /// Inspired by Luffy's Gear 4 / Zoro's Armament Haki (One Piece).
    /// </summary>
    [CreateAssetMenu(fileName = "HakiArmamentGauntlet", menuName = "Aurora/Weapons/Expanded/Haki Armament Gauntlet")]
    public class HakiArmamentGauntletWeapon : MeleeWeapon {

        [Header("Haki Cycle")]
        [Tooltip("How long the Haki-ON (empowered) window lasts in seconds.")]
        [SerializeField] private float _hakiActiveDuration = 3f;

        [Tooltip("Recharge time before the next Haki window.")]
        [SerializeField] private float _hakiRechargeTime = 6f;

        [Tooltip("Damage multiplier during the Haki window.")]
        [SerializeField] private float _hakiDamageMult = 3.0f;

        [Tooltip("Attack size bonus during Haki (adds to the base attack size).")]
        [SerializeField] private Vector2 _hakiSizeBonus = new Vector2(0.6f, 0.4f);

        private bool _isHakiActive = false;
        private float _hakiTimer = 0f;

        private void OnEnable() {
            // Begin in Haki-ON phase so the player feels power immediately
            _isHakiActive = true;
            _hakiTimer = _hakiActiveDuration;
        }

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            TickHakiCycle();
            animations?.Play(AttackAnimation);
            StartCooldown();
        }

        private void TickHakiCycle() {
            _hakiTimer -= AttackCooldown;

            if (_hakiTimer <= 0f) {
                _isHakiActive = !_isHakiActive;
                _hakiTimer = _isHakiActive ? _hakiActiveDuration : _hakiRechargeTime;
                DebugTools.DebugLog.Log($"[Haki] Phase switched → {(_isHakiActive ? "HAKI ON 🔥" : "Recharging...")}");
            }
        }

        public override int GetDamage() {
            return _isHakiActive
                ? Mathf.CeilToInt(base.GetDamage() * _hakiDamageMult)
                : base.GetDamage();
        }

        public override Vector2 AttackSize => _isHakiActive
            ? base.AttackSize + _hakiSizeBonus
            : base.AttackSize;

        /// <summary>Public accessor for HUD visualization of current Haki state.</summary>
        public bool IsHakiActive => _isHakiActive;

        /// <summary>Returns 0..1 progress through the current phase (for a charge bar).</summary>
        public float HakiPhaseProgress => _isHakiActive
            ? 1f - (_hakiTimer / _hakiActiveDuration)
            : _hakiTimer / _hakiRechargeTime;
    }
}
