using Base;
using UnityEngine;
using BerserkPixel.Health;
using System.Collections.Generic;

namespace Weapons.DesignEx {
    /// <summary>
    /// Configuration for a delayed execution melee weapon. 
    /// Melee hits apply "Shatter Points" instead of dealing immediate damage.
    /// Mastery: Waiting 1.5s after the last hit triggers a massive detonation.
    /// </summary>
    [CreateAssetMenu(fileName = "ShatterPointRapier", menuName = "Aurora/Weapons/Expanded/Shatter-Point Rapier")]
    public class ShatterPointRapier : MeleeWeapon {
        
        [Header("Shatter Mechanics")]
        [Tooltip("Seconds of inactivity required to trigger the explosion.")]
        [SerializeField] private float _detonationDelay = 1.2f;

        [Tooltip("Multiplier applied to the total damage per stack.")]
        [SerializeField] private float _damagePerStackMult = 1.2f;

        private float _lastHitTime = 0f;
        private bool _isPendingDetonation = false;

        // Tracks all enemies currently marked for shatter in the scene
        private static HashSet<ShatterStackComponent> _markedEnemies = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ResetStaticData() {
            _markedEnemies = new();
        }

        /// <summary>
        /// Registers a hit from the rapier. Instead of damage, it adds a stack.
        /// </summary>
        public void ApplyShatterHit(GameObject enemy) {
            if (enemy == null) return;
            
            // Try to find the stack component on the enemy
            if (!enemy.TryGetComponent<ShatterStackComponent>(out var stacks)) {
                stacks = enemy.AddComponent<ShatterStackComponent>();
                stacks.Initialize(this);
                _markedEnemies.Add(stacks);
            }

            // Add a stack (base damage of the weapon is used as the weight)
            stacks.AddStack(GetDamage(), _damagePerStackMult);
            
            _lastHitTime = Time.time;
            _isPendingDetonation = true;
            
            // Reward sound/FX for the mark
            PlayImpactSound(enemy.transform.position, "shatter_mark");
        }

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;
            
            animations?.Play(AttackAnimation);
            // The Hit Detection in your system calls weapon methods on overlap.
            // We need to ensure the Attack collider is active.
            
            StartCooldown();
        }

        // --- TICK PROCESSOR ---
        /// <summary>
        /// Monitoring for the "Sheathe" (inaction) moment to trigger the POP.
        /// </summary>
        public void UpdateStacks() {
            if (!_isPendingDetonation) return;

            if (Time.time >= _lastHitTime + _detonationDelay) {
                ExecuteAllShatters();
                _isPendingDetonation = false;
            }
        }

        private void ExecuteAllShatters() {
            foreach (var enemy in _markedEnemies) {
                if (enemy != null) {
                    enemy.Detonate();
                }
            }
            _markedEnemies.Clear();
            
            // Potential global FX for the multi-pop
        }
    }

    /// <summary>
    /// Temporary component added to enemies hit by the rapier.
    /// Handles the damage calculation and explosion logic.
    /// </summary>
    public class ShatterStackComponent : MonoBehaviour {
        private int _totalAccumulatedDamage = 0;
        private int _stackCount = 0;
        private float _multiplier = 1.0f;
        private Weapons.Weapon _sourceWeapon;

        public void Initialize(Weapons.Weapon weapon) {
            _sourceWeapon = weapon;
        }

        public void AddStack(int baseDamage, float multPerStack) {
            _totalAccumulatedDamage += baseDamage;
            _stackCount++;
            _multiplier = multPerStack;
        }

        public void Detonate() {
            int finalDamage = Mathf.CeilToInt(_totalAccumulatedDamage * Mathf.Pow(_multiplier, _stackCount - 1));
            
            if (TryGetComponent<CharacterHealth>(out var health)) {
                var hitData = new HitDataBuilder()
                    .WithWeapon(_sourceWeapon)
                    .WithDamage(finalDamage)
                    .WithDirection(Vector3.zero) // Detonation is omni-directional locally
                    .Build(null, transform); // Attacker TBD, can be weapon owner
                    
                health.PerformDamage(hitData);
            }
            
            Debug.Log($"Shatter Detonated on {gameObject.name}: {finalDamage} total damage from {_stackCount} stacks!");

            Destroy(this);
        }
    }
}
