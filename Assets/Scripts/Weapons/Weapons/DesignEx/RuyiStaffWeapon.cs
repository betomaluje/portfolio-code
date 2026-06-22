using BerserkPixel.Health;
using Base;
using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// An extending magical staff that grows in reach based on charge time.
    /// 
    /// Standard Tap: Wide sweeping strikes at normal staff range.
    /// Charged Attack (ICharge): Holding extends the staff further. On release,
    ///     the collider is already scaled, and enemies at the tip of the extension
    ///     are struck by a critical bonus (damage spike).
    ///     The larger the charge, the farther the reach and the bigger the crit bonus.
    ///
    /// Implements IWeaponCollider so the WeaponManager dynamically resizes 
    /// the attack BoxCollider2D at the moment of weapon equip / charge change.
    ///
    /// Inspired by Goku's Nyoi-bō / Sun Wukong's Ruyi Jingu Bang (Dragon Ball / Journey to the West).
    /// </summary>
    [CreateAssetMenu(fileName = "RuyiStaff", menuName = "Aurora/Weapons/Expanded/Ruyi Jingu Bang Staff")]
    public class RuyiStaffWeapon : MeleeWeapon, ICharge, IWeaponCollider {

        [Header("Extension Properties")]
        [Tooltip("Minimum attack size (x = length, y = width) at zero charge.")]
        [SerializeField] private Vector2 _minAttackSize = new Vector2(1.5f, 0.8f);

        [Tooltip("Maximum attack size at full charge.")]
        [SerializeField] private Vector2 _maxAttackSize = new Vector2(6f, 0.8f);

        [Tooltip("Minimum attack offset (where the collider is centered) at zero charge.")]
        [SerializeField] private Vector2 _minAttackOffset = new Vector2(0.8f, 0f);

        [Tooltip("Maximum attack offset at full charge — pushes the strike zone further out.")]
        [SerializeField] private Vector2 _maxAttackOffset = new Vector2(3.5f, 0f);

        [Tooltip("Time in seconds to reach maximum charge output.")]
        [SerializeField] private float _chargeTime = 2.0f;

        [Tooltip("Damage multiplier applied when a hit occurs at > 60% extension (tip strike).")]
        [SerializeField] private float _tipDamageMult = 2.5f;

        [Tooltip("Target layer to detect enemies at the tip of the staff.")]
        [SerializeField] private LayerMask _targetMask;

        private float _chargeValue;

        public float Charge {
            set => _chargeValue = Mathf.Clamp01(value);
        }

        public float ChargeTime => _chargeTime;

        // IWeaponCollider — these are polled by WeaponManager.UpdateWeaponCollider() 
        // every time the weapon is selected (and again when Attack is called if extended).
        public override Vector2 AttackSize => Vector2.Lerp(_minAttackSize, _maxAttackSize, _chargeValue);
        public override Vector2 AttackOffset => Vector2.Lerp(_minAttackOffset, _maxAttackOffset, _chargeValue);

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            animations?.Play(AttackAnimation);

            // At high charge, scan for enemies at the tip zone and mark them for a crit bonus
            if (_chargeValue > 0.6f) {
                ApplyTipBonus(animations?.Transform.root, direction);
            }

            _chargeValue = 0f;
            StartCooldown();
        }

        /// <summary>
        /// Performs an additional scan at the far tip of the staff extension.
        /// Any enemy found there receives the tip damage multiplier on top of the
        /// standard hit that the AttackState's DetectAttack() will already process.
        /// </summary>
        private void ApplyTipBonus(Transform owner, Vector2 direction) {
            if (owner == null) return;

            // The tip zone center is at the far end of the current attack offset + half the size
            Vector2 tipCenter = (Vector2)owner.position + direction.normalized *
                                (AttackOffset.x + AttackSize.x * 0.5f);

            Collider2D[] atTip = Physics2D.OverlapCircleAll(tipCenter, 0.6f, _targetMask);

            foreach (var col in atTip) {
                if (col.TryGetComponent<BerserkPixel.Health.CharacterHealth>(out var health) && !health.IsDead) {
                    var tipHit = new HitDataBuilder()
                        .WithWeapon(this)
                        .WithDamage(Mathf.CeilToInt(GetDamage() * _tipDamageMult))
                        .WithDirection((col.transform.position - owner.position).normalized)
                        .Build(owner, col.transform);

                    health.PerformDamage(tipHit);

                    PlayImpactSound(col.transform.position, "staff_crack");
                    DebugTools.DebugLog.Log($"[RuyiStaff] Tip bonus hit: {col.name}");
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            Gizmos.color = new Color(1f, 0.4f, 0f, 0.3f);
            // Draw a rough visual of the maximum attack zone
            Gizmos.DrawWireCube((Vector3)(Vector2.right * _maxAttackOffset.x), _maxAttackSize);
        }
#endif
    }
}
