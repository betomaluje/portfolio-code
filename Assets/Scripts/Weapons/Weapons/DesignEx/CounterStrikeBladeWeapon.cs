using Base;
using UnityEngine;
using BerserkPixel.Health;

namespace Weapons.DesignEx {
    /// <summary>
    /// A parry-and-punish melee weapon with two distinct states:
    /// 
    ///   GUARD (first press): Player enters a brief parry window. If the player
    ///       receives an incoming hit during this window, the weapon absorbs the attack
    ///       and primes a COUNTER that multiplies the next strike's damage.
    ///
    ///   COUNTER (after a successful parry): The next swing deals massive bonus damage
    ///       (scales with how much damage was parried) and has a guaranteed critical hit.
    ///       Counter window expires after _counterExpireSeconds.
    ///
    ///   Normal swing: If no parry is active, behaves like a standard MeleeWeapon.
    ///
    /// Inspired by the counter-attack techniques of Muichiro / Genya (Demon Slayer)
    /// and Vergil's Royal Guard (Devil May Cry).
    /// </summary>
    [CreateAssetMenu(fileName = "CounterStrikeBlade", menuName = "Aurora/Weapons/Expanded/Counter-Strike Blade")]
    public class CounterStrikeBladeWeapon : MeleeWeapon {

        [Header("Guard Window")]
        [Tooltip("Duration of the parry window in seconds after the attack button is pressed.")]
        [SerializeField] private float _guardWindowSeconds = 0.4f;

        [Tooltip("Cooldown after a missed guard before the weapon can guard again.")]
        [SerializeField] private float _guardFailCooldown = 1.2f;

        [Header("Counter Strike")]
        [Tooltip("Damage multiplier applied to the counter-strike. Stacks with the parried damage.")]
        [SerializeField] private float _counterDamageMult = 4.0f;

        [Tooltip("How long the counter-strike window stays active after a successful parry.")]
        [SerializeField] private float _counterExpireSeconds = 3f;

        [Tooltip("The attack animation name played on a successful counter.")]
        [SerializeField] private string _counterAnimation = "AttackCounter";

        private enum StrikeState { Ready, Guarding, CounterCharged }
        private StrikeState _state = StrikeState.Ready;

        private float _stateTimer = 0f;
        private int _parryAbsorbedDamage = 0;
        private Transform _ownerTransform;

        private void OnEnable() {
            _state = StrikeState.Ready;
            _stateTimer = 0f;
            _parryAbsorbedDamage = 0;
            CharacterHealth.OnAnyDamagePerformed += HandleIncomingHit;
        }

        private void OnDisable() {
            CharacterHealth.OnAnyDamagePerformed -= HandleIncomingHit;
        }

        private void OnDestroy() {
            CharacterHealth.OnAnyDamagePerformed -= HandleIncomingHit;
        }

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            // Cache the owner so we can identify player hits in OnAnyDamagePerformed
            _ownerTransform = animations?.Transform.root;

            TickStateTimer();

            switch (_state) {
                case StrikeState.CounterCharged:
                    ExecuteCounterStrike(animations, direction, position);
                    break;

                case StrikeState.Ready:
                    // Enter guard window — player is committing to a parry attempt
                    _state = StrikeState.Guarding;
                    _stateTimer = _guardWindowSeconds;
                    _parryAbsorbedDamage = 0;
                    animations?.Play(AttackAnimation); // Play a guard/block animation
                    // Note: actual cooldown deferred until counter or guard expires
                    break;

                case StrikeState.Guarding:
                    // Player pressed again while guarding — cancel to normal swing
                    _state = StrikeState.Ready;
                    animations?.Play(AttackAnimation);
                    StartCooldown();
                    break;
            }
        }

        private void ExecuteCounterStrike(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            animations?.Play(_counterAnimation);

            // Counter damage: base * multiplier + absorbed damage bonus
            int counterDamage = Mathf.CeilToInt((GetDamage() + _parryAbsorbedDamage) * _counterDamageMult);

            // Detect enemies via Physics (mirroring how CaseyBatReflector does direct circle hits)
            Collider2D[] nearby = Physics2D.OverlapCircleAll(position, AttackSize.magnitude);
            foreach (var col in nearby) {
                if (col.TryGetComponent<CharacterHealth>(out var health) && !health.IsDead) {
                    Vector2 dir = ((Vector2)col.transform.position - (Vector2)position).normalized;
                    var hitData = new HitDataBuilder()
                        .WithWeapon(this)
                        .WithDamage(counterDamage)
                        .WithCriticalHitChance(1f) // Counter always crits
                        .WithDirection(dir)
                        .Build(_ownerTransform, col.transform);

                    health.PerformDamage(hitData);
                }
            }

            PlayImpactSound(position, "counter_slash");
            DebugTools.DebugLog.Log($"[Counter] COUNTER hit! {counterDamage} damage (absorbed: {_parryAbsorbedDamage})");

            _parryAbsorbedDamage = 0;
            _state = StrikeState.Ready;
            StartCooldown();
        }

        /// <summary>
        /// Listens for any damage event. If the victim is our owner and we are currently
        /// Guarding, absorb the hit and charge the counter.
        /// </summary>
        private void HandleIncomingHit(HitData data) {
            if (_state != StrikeState.Guarding) return;
            if (_ownerTransform == null || data.victim == null) return;
            if (data.victim != _ownerTransform) return;

            // Successful parry!
            _parryAbsorbedDamage = data.damage;
            _state = StrikeState.CounterCharged;
            _stateTimer = _counterExpireSeconds;

            PlayImpactSound(data.victim.position, "parry_clang");
            DebugTools.DebugLog.Log($"[Counter] PARRY! Absorbed {_parryAbsorbedDamage}. Counter charged!");
        }

        private void TickStateTimer() {
            if (_stateTimer <= 0f) return;
            _stateTimer -= AttackCooldown;

            if (_stateTimer <= 0f) {
                // Guard window expired without a parry, or counter window expired
                if (_state == StrikeState.Guarding) {
                    DebugTools.DebugLog.Log("[Counter] Guard expired — missed parry.");
                    StartCooldown(); // apply the miss penalty cooldown
                } else if (_state == StrikeState.CounterCharged) {
                    DebugTools.DebugLog.Log("[Counter] Counter window expired.");
                }
                _state = StrikeState.Ready;
            }
        }

        /// <summary>Public accessor for HUD state display.</summary>
        public bool IsGuarding => _state == StrikeState.Guarding;
        public bool IsCounterCharged => _state == StrikeState.CounterCharged;
    }
}
