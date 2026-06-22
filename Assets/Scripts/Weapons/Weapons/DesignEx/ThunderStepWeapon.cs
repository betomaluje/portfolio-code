using Base;
using UnityEngine;
using BerserkPixel.Health;

namespace Weapons.DesignEx {
    /// <summary>
    /// A single-target lightning strike that teleports the player directly onto the enemy.
    /// Hold ICharge for commitment — on release, Raycast finds the first enemy in aim direction,
    /// snaps the player to their position, and applies a massive damage hit with a crit bonus
    /// that scales with charge duration. Quick-tap fires a basic close-range lunge.
    /// Inspired by Zenitsu's Thunder Breathing — Thunderclap and Flash (Demon Slayer).
    /// </summary>
    [CreateAssetMenu(fileName = "ThunderStepWeapon", menuName = "Aurora/Weapons/Expanded/Zenitsu Thunder Step")]
    public class ThunderStepWeapon : DashMeleeWeapon, ICharge {

        [Header("Thunder Step Properties")]
        [Tooltip("Layer mask for the raycast target — enemies only.")]
        [SerializeField] private LayerMask _targetMask;

        [Tooltip("Layer mask for obstacles that block the step.")]
        [SerializeField] private LayerMask _obstacleMask;

        [Tooltip("Maximum step distance.")]
        [SerializeField] private float _maxStepDistance = 14f;

        [Tooltip("Damage multiplier at full charge. The step always crits at full charge.")]
        [SerializeField] private float _maxDamageMult = 5f;

        [Tooltip("Base critical hit chance even without charge.")]
        [SerializeField, Range(0f, 1f)] private float _baseCritChance = 0.3f;

        [Tooltip("Time in seconds to reach maximum charge.")]
        [SerializeField] private float _chargeTime = 1.8f;

        private float _chargeValue;

        public float Charge {
            set => _chargeValue = Mathf.Clamp01(value);
        }

        public float ChargeTime => _chargeTime;

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            animations?.Play(AttackAnimation);

            Transform owner = animations?.Transform.root;
            if (owner == null) {
                StartCooldown();
                return;
            }

            if (_chargeValue > 0.05f) {
                ExecuteThunderStep(owner, direction.normalized);
            } else {
                // Quick-tap: basic close-range impulse (base class handles the dash physics)
                if (owner.TryGetComponent<Rigidbody2D>(out var rb)) {
                    rb.AddForce(direction.normalized * Range * 8f, ForceMode2D.Impulse);
                }
            }

            _chargeValue = 0f;
            StartCooldown();
        }

        private void ExecuteThunderStep(Transform owner, Vector2 direction) {
            Vector2 origin = owner.position;
            float dist = Mathf.Lerp(_maxStepDistance * 0.25f, _maxStepDistance, _chargeValue);

            // Stop at walls
            RaycastHit2D wall = Physics2D.Raycast(origin, direction, dist, _obstacleMask);
            float effectiveDist = wall.collider != null ? wall.distance - 0.4f : dist;

            // Find first enemy along the path
            RaycastHit2D enemyHit = Physics2D.Raycast(origin, direction, effectiveDist, _targetMask);

            if (enemyHit.collider != null && enemyHit.collider.TryGetComponent<CharacterHealth>(out var health) && !health.IsDead) {
                // Snap player right next to the target
                Vector2 snapPos = enemyHit.point - direction * 0.5f;
                owner.position = snapPos;

                // Damage with crit scaling
                float critChance = Mathf.Lerp(_baseCritChance, 1f, _chargeValue);
                int damage = Mathf.CeilToInt(GetDamage() * Mathf.Lerp(1f, _maxDamageMult, _chargeValue));

                var hitData = new HitDataBuilder()
                    .WithWeapon(this)
                    .WithDamage(damage)
                    .WithCriticalHitChance(critChance)
                    .WithDirection(direction)
                    .Build(owner, enemyHit.collider.transform);

                health.PerformDamage(hitData);
                PlayImpactSound(snapPos, "thunder_strike");
            } else {
                // No target — still dash to the end point
                owner.position = origin + direction * effectiveDist;
            }
        }
    }
}
