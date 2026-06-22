using Base;
using UnityEngine;
using BerserkPixel.Health;

namespace Weapons.DesignEx {
    /// <summary>
    /// Releases an omnidirectional shockwave that simultaneously damages and repels all
    /// enemies in the area. Damage is proximity-based: enemies at the epicenter take full
    /// damage while those at the edge receive a fraction. All enemies receive a radial
    /// knockback impulse regardless of damage.
    /// Inspired by Conqueror's Haki — Haōshoku (One Piece).
    /// </summary>
    [CreateAssetMenu(fileName = "ConquerorDomainStrike", menuName = "Aurora/Weapons/Expanded/Conqueror Domain Strike")]
    public class ConquerorDomainStrikeWeapon : MeleeWeapon {

        [Header("Domain Properties")]
        [Tooltip("Radius of the Conqueror shockwave.")]
        [SerializeField] private float _domainRadius = 5f;

        [Tooltip("Outward knockback force applied to all enemies in range.")]
        [SerializeField] private float _shockwaveForce = 30f;

        [Tooltip("Enemies closer than this distance to the epicenter receive full damage.")]
        [SerializeField] private float _epicenterRadius = 1.5f;

        [Tooltip("Target layer for shockwave detection.")]
        [SerializeField] private LayerMask _targetMask;

        [Tooltip("Optional particle effect spawned at the strike position.")]
        [SerializeField] private ParticleSystem _shockwaveEffect;

        public override bool ShouldMoveAttackCollider() => false;
        public override Vector2 AttackSize => Vector2.one * (_domainRadius * 2f);
        public override Vector2 AttackOffset => Vector2.zero;

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            animations?.Play(AttackAnimation);

            Transform owner = animations?.Transform.root;
            ExecuteShockwave(position, owner);

            if (_shockwaveEffect != null) {
                var fx = Instantiate(_shockwaveEffect, position, Quaternion.identity);
                fx.transform.localScale = Vector3.one * _domainRadius;
                fx.Play();
                Destroy(fx.gameObject, fx.main.duration);
            }

            PlayImpactSound(position, "conqueror_shout");
            StartCooldown();
        }

        private void ExecuteShockwave(Vector3 center, Transform owner) {
            Collider2D[] hits = Physics2D.OverlapCircleAll(center, _domainRadius, _targetMask);

            foreach (var col in hits) {
                if (!col.TryGetComponent<CharacterHealth>(out var health) || health.IsDead) continue;

                float dist = Vector2.Distance(center, col.transform.position);

                // Proximity damage: full at epicenter, falls off linearly at the edge
                float proximityRatio = 1f - Mathf.Clamp01((dist - _epicenterRadius) / (_domainRadius - _epicenterRadius));
                int damage = Mathf.CeilToInt(GetDamage() * Mathf.Max(0.2f, proximityRatio));

                Vector2 awayDir = ((Vector2)col.transform.position - (Vector2)center).normalized;

                var hitData = new HitDataBuilder()
                    .WithWeapon(this)
                    .WithDamage(damage)
                    .WithDirection(awayDir)
                    .Build(owner, col.transform);

                health.PerformDamage(hitData);

                // Radial knockback via Rigidbody2D
                if (col.TryGetComponent<Rigidbody2D>(out var rb)) {
                    rb.AddForce(awayDir * _shockwaveForce, ForceMode2D.Impulse);
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            Gizmos.color = new Color(0.5f, 0f, 1f, 0.2f);
            Gizmos.DrawWireSphere(Vector3.zero, _domainRadius);
            Gizmos.color = new Color(0.9f, 0.2f, 1f, 0.35f);
            Gizmos.DrawWireSphere(Vector3.zero, _epicenterRadius);
        }
#endif
    }
}
