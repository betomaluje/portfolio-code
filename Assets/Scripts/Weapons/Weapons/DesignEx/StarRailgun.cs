using UnityEngine;
using Weapons;
using Base;
using BerserkPixel.Health;

namespace Weapons.DesignEx {
    /// <summary>
    /// A high-precision railgun that fires stellar beams.
    /// Mechanics: Instant hit along a beam path (Raycast-based).
    /// Penetrates all targets. If an enemy dies from the beam, they explode into a Supernova.
    /// </summary>
    [CreateAssetMenu(fileName = "StarRailgun", menuName = "Aurora/Weapons/Expanded/Star Railgun")]
    public class StarRailgun : ShootingWeapon {
        
        [Header("Railgun Stats")]
        [Tooltip("The thickness of the railgun beam.")]
        [SerializeField] private float _beamWidth = 0.5f;

        [Tooltip("Damage multiplier for the Supernova explosion on kill.")]
        [SerializeField] private float _supernovaDamageMult = 1.5f;

        [Tooltip("Radius of the Supernova explosion.")]
        [SerializeField] private float _supernovaRadius = 4.0f;

        [Tooltip("Target layer mask for hit detection.")]
        [SerializeField] private LayerMask _targetMask;

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            if (HasAmmo()) {
                animations?.Play(AttackAnimation);
                
                ExecuteBeamAttack(position, direction.normalized, animations?.Transform.root);

                StartCooldown();
            } else {
                Reload(animations);
            }
        }

        private void ExecuteBeamAttack(Vector2 origin, Vector2 direction, Transform player) {
            // Instant hit along a path
            RaycastHit2D[] hits = Physics2D.CircleCastAll(origin, _beamWidth / 2f, direction, Range, _targetMask);
            
            // Visual Beam
            // DrawLineVFX(origin, origin + direction * Range);

            foreach (var hit in hits) {
                if (hit.collider.TryGetComponent<BerserkPixel.Health.CharacterHealth>(out var health) && !health.IsDead) {
                    var railHit = new HitDataBuilder()
                        .WithWeapon(this)
                        .WithDamage(GetDamage())
                        .WithDirection(direction)
                        .Build(player, hit.collider.transform);
                        
                    // Before performing damage, we check if they survive
                    int dmg = railHit.damage;
                    bool willDie = health.CurrentHealth <= dmg;

                    health.PerformDamage(railHit);

                    if (willDie) {
                        TriggerSupernova(hit.point, player);
                    }
                }
            }
            
            PlayImpactSound(origin, "railgun_stellar_fire");
        }

        private void TriggerSupernova(Vector2 position, Transform player) {
            // Explosion logic for a kill
            Collider2D[] trapped = Physics2D.OverlapCircleAll(position, _supernovaRadius, _targetMask);
            
            foreach (var col in trapped) {
                if (col.TryGetComponent<BerserkPixel.Health.CharacterHealth>(out var health) && !health.IsDead) {
                    var novaHit = new HitDataBuilder()
                        .WithWeapon(this)
                        .WithDamage(Mathf.CeilToInt(GetDamage() * _supernovaDamageMult))
                        .WithDirection((col.transform.position - (Vector3)position).normalized)
                        .Build(player, col.transform);
                        
                    health.PerformDamage(novaHit);
                }
            }
            
            // VFX/SFX
            // SpawnSupernovaVFX(position);
            PlayImpactSound(position, "supernova_blast");
        }
    }
}
