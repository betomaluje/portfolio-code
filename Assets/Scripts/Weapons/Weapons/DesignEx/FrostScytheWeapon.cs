using UnityEngine;
using Weapons;
using Base;
using BerserkPixel.Health;

namespace Weapons.DesignEx {
    /// <summary>
    /// A heavy scythe inspired by Ember Knights.
    /// Mechanics: Slow, wide swings. Every 3rd hit triggers a Frost Nova 
    /// that slows enemies and makes them vulnerable.
    /// </summary>
    [CreateAssetMenu(fileName = "FrostScythe", menuName = "Aurora/Weapons/Expanded/Frost-Bite Scythe")]
    public class FrostScytheWeapon : MeleeWeapon {
        
        [Header("Frost Properties")]
        [Tooltip("Number of hits required to trigger the Frost Nova.")]
        [SerializeField] private int _hitsToTriggerNova = 3;

        [Tooltip("The radius of the Frost Nova effect.")]
        [SerializeField] private float _novaRadius = 4.5f;

        [Tooltip("Target layer for the nova.")]
        [SerializeField] private LayerMask _targetMask;

        private int _currentHitCount = 0;

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

            animations?.Play(AttackAnimation);
            StartCooldown();
        }

        private void HandleAnyDamagePerformed(HitData data) {
            // Check if THIS specific weapon instance caused the damage
            if (data.weapon == this && data.attacker != null) {
                _currentHitCount++;

                if (_currentHitCount >= _hitsToTriggerNova) {
                    TriggerFrostNova(data.victim.position, data.attacker);
                    _currentHitCount = 0;
                }
            }
        }

        private void TriggerFrostNova(Vector2 position, Transform player) {
            Collider2D[] trapped = Physics2D.OverlapCircleAll(position, _novaRadius, _targetMask);
            
            foreach (var col in trapped) {
                if (col.TryGetComponent<CharacterHealth>(out var health) && !health.IsDead) {
                    var novaHit = new HitDataBuilder()
                        .WithWeapon(this)
                        .WithDamage(Mathf.CeilToInt(GetDamage() * 0.5f))
                        .WithDirection((col.transform.position - (Vector3)position).normalized)
                        .Build(player, col.transform);
                        
                    health.PerformDamage(novaHit);
                    
                    // Status Effect: Slow (can be handled via health system modifiers or separate script)
                    // ApplyFrostStatus(col.gameObject);
                }
            }
            
            PlayImpactSound(position, "frost_nova_shatter");
            // SpawnNovaVFX(position);
        }
    }
}
