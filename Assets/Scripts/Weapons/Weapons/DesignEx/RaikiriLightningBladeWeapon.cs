using Base;
using BerserkPixel.Health;
using Extensions;
using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// Concentrates lightning into the palm for a high-speed dashing strike.
    /// 
    /// Charged Attack (ICharge): Hold to charge. On release, the player dashes
    ///     forward through enemies at extreme speed. The first target hit is
    ///     electrocuted and lightning chains to adjacent enemies via LightningChainBullet.
    /// 
    /// Standard Tap: A fast palm-strike at close range.
    ///
    /// Inherits dash logic from DashMeleeWeapon.
    /// Inspired by Kakashi's Raikiri / Sasuke's Chidori (Naruto).
    /// </summary>
    [RequiredBullet(typeof(LightningChainBullet))]
    [CreateAssetMenu(fileName = "RaikiriLightningBlade", menuName = "Aurora/Weapons/Expanded/Raikiri Lightning Blade")]
    public class RaikiriLightningBladeWeapon : DashMeleeWeapon, ICharge {

        [Header("Lightning Configuration")]
        [Tooltip("The lightning chain prefab spawned on the first target hit. Must have a LightningChainBullet component.")]
        [SerializeField] public GameObject BulletPrefab;

        [Tooltip("Obstacle layer mask to stop the dash (walls, etc.).")]
        [SerializeField] private LayerMask _obstaclesMask;

        [Tooltip("Target layer mask for enemies the dash strikes.")]
        [SerializeField] private LayerMask _targetMask;

        [Tooltip("Time in seconds to reach maximum charge output.")]
        [SerializeField] private float _chargeTime = 1.8f;

        [Tooltip("Maximum distance the dash can cover at full charge.")]
        [SerializeField] private float _maxDashDistance = 10f;

        [Tooltip("Width of the strike path during the dash.")]
        [SerializeField] private float _strikePathWidth = 1f;

        [Tooltip("Damage multiplier applied at full charge.")]
        [SerializeField] private float _maxDamageMult = 2.5f;

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
                // Full Raikiri dash — teleport player forward, hit all in path
                ExecuteRaikiriDash(owner, direction.normalized);
            } else {
                // Quick palm strike — use base class dash physics for a tiny lunge
                if (owner.TryGetComponent<Rigidbody2D>(out var rb)) {
                    float quickLunge = Range * 5f;
                    rb.AddForce(direction.normalized * quickLunge, ForceMode2D.Impulse);
                }
            }

            _chargeValue = 0f;
            StartCooldown();
        }

        private void ExecuteRaikiriDash(Transform owner, Vector2 direction) {
            Vector2 startPos = owner.position;
            float dashDist = Mathf.Lerp(_maxDashDistance * 0.3f, _maxDashDistance, _chargeValue);

            // Stop at wall
            RaycastHit2D wall = Physics2D.Raycast(startPos, direction, dashDist, _obstaclesMask);
            Vector2 endPos = wall.collider != null
                ? wall.point - direction * 0.5f
                : startPos + direction * dashDist;

            // Teleport player
            owner.position = endPos;

            // Detect all enemies along the dash path
            float pathLen = Vector2.Distance(startPos, endPos);
            Vector2 pathCenter = (startPos + endPos) * 0.5f;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Collider2D[] struck = Physics2D.OverlapBoxAll(
                pathCenter,
                new Vector2(pathLen, _strikePathWidth),
                angle,
                _targetMask
            );

            bool chainSpawned = false;
            int dashDamage = Mathf.CeilToInt(GetDamage() * Mathf.Lerp(1f, _maxDamageMult, _chargeValue));

            foreach (var col in struck) {
                if (col.TryGetComponent<CharacterHealth>(out var health) && !health.IsDead) {
                    var hitData = new HitDataBuilder()
                        .WithWeapon(this)
                        .WithDamage(dashDamage)
                        .WithDirection(direction)
                        .Build(owner, col.transform);

                    health.PerformDamage(hitData);

                    // Spawn the lightning chain on the first hit target only
                    if (!chainSpawned && BulletPrefab != null) {
                        SpawnLightningChain(col.transform.position, owner);
                        chainSpawned = true;
                    }
                }
            }

            PlayImpactSound(endPos, "lightning_strike");
        }

        private void SpawnLightningChain(Vector3 atPosition, Transform owner) {
            var chainObj = Instantiate(BulletPrefab, atPosition, Quaternion.identity);
            var chain = chainObj.GetOrAdd<LightningChainBullet>();

            chain.SetWeapon(this);
            chain.SetOwner(owner);
            chain.SetDamage(Mathf.CeilToInt(GetDamage() * 0.6f));
            chain.Fire(Vector2.zero); // direction unused — chain is stationary
        }
    }
}
