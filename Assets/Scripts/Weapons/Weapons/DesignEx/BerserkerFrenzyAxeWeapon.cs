using Base;
using UnityEngine;
using BerserkPixel.Health;

namespace Weapons.DesignEx {
    /// <summary>
    /// A massive berserker axe that gains power with every confirmed enemy kill.
    /// Frenzy stacks accumulate up to a cap and each stack raises GetDamage().
    /// Stacks begin to decay after a configurable window of inactivity.
    /// Inspired by Guts from Berserk — the longer the fight, the more dangerous.
    /// </summary>
    [CreateAssetMenu(fileName = "BerserkerFrenzyAxe", menuName = "Aurora/Weapons/Expanded/Berserker Frenzy Axe")]
    public class BerserkerFrenzyAxeWeapon : MeleeWeapon {

        [Header("Frenzy Stacks")]
        [Tooltip("Maximum frenzy stacks.")]
        [SerializeField] private int _maxFrenzyStacks = 8;

        [Tooltip("Damage multiplier bonus per stack (additive). e.g. 0.15 = +15% per stack.")]
        [SerializeField] private float _damageBonusPerStack = 0.15f;

        [Tooltip("Seconds without a kill before stacks start decaying.")]
        [SerializeField] private float _decayWindowSeconds = 4f;

        [Tooltip("Seconds per stack lost during decay.")]
        [SerializeField] private float _decayRateSeconds = 1.5f;

        private int _frenzyStacks = 0;
        private float _lastKillTime = 0f;
        private float _lastDecayTime = 0f;
        private bool _isDecaying = false;

        private void OnEnable() {
            _frenzyStacks = 0;
            _lastKillTime = 0f;
            _lastDecayTime = 0f;
            _isDecaying = false;
            CharacterHealth.OnAnyDamagePerformed += HandleDamagePerformed;
        }

        private void OnDisable() {
            CharacterHealth.OnAnyDamagePerformed -= HandleDamagePerformed;
        }

        private void OnDestroy() {
            CharacterHealth.OnAnyDamagePerformed -= HandleDamagePerformed;
        }

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            // Tick decay while the player is still swinging
            TickDecay();

            animations?.Play(AttackAnimation);
            StartCooldown();
        }

        private void HandleDamagePerformed(HitData data) {
            if (data.weapon != this || data.victim == null) return;

            // Only grant a stack on a lethal hit
            if (data.victim.TryGetComponent<CharacterHealth>(out var health) && health.IsDead) {
                _frenzyStacks = Mathf.Min(_frenzyStacks + 1, _maxFrenzyStacks);
                _lastKillTime = Time.time;
                _isDecaying = false;

                DebugTools.DebugLog.Log($"[Berserker] FRENZY x{_frenzyStacks} — damage mult: {GetFrenzyMultiplier():F2}");
            }
        }

        private void TickDecay() {
            if (_frenzyStacks <= 0) return;

            float timeSinceKill = Time.time - _lastKillTime;

            // Begin decaying after the window
            if (timeSinceKill >= _decayWindowSeconds) {
                _isDecaying = true;
            }

            if (_isDecaying && Time.time >= _lastDecayTime + _decayRateSeconds) {
                _frenzyStacks = Mathf.Max(0, _frenzyStacks - 1);
                _lastDecayTime = Time.time;
                DebugTools.DebugLog.Log($"[Berserker] Stack decayed → x{_frenzyStacks}");
            }
        }

        public override int GetDamage() {
            return Mathf.CeilToInt(base.GetDamage() * GetFrenzyMultiplier());
        }

        private float GetFrenzyMultiplier() => 1f + (_frenzyStacks * _damageBonusPerStack);

        /// <summary>Public accessor for HUD display if needed.</summary>
        public int FrenzyStacks => _frenzyStacks;
        public int MaxFrenzyStacks => _maxFrenzyStacks;
    }
}
