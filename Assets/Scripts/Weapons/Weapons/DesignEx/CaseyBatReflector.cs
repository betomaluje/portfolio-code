using UnityEngine;
using Weapons;
using Base;
using BerserkPixel.Health;

namespace Weapons.DesignEx {
    /// <summary>
    /// A heavy metal bat inspired by Casey from Enter the Gungeon.
    /// Mechanics: Slow swing, massive knockback. 
    /// If it hits an enemy projectile (IBullet), it reflects it back towards the aim direction.
    /// </summary>
    [CreateAssetMenu(fileName = "CaseyBat", menuName = "Aurora/Weapons/Expanded/Casey Reflector Bat")]
    public class CaseyBatReflector : MeleeWeapon {
        
        [Header("Reflector Stats")]
        [Tooltip("The targets that can be hit.")]
        [SerializeField] private LayerMask _targetMask;

        [Tooltip("The speed multiplier applied to reflected bullets.")]
        [SerializeField] private float _reflectSpeedMult = 2.0f;
        
        [Tooltip("Extra damage multiplier for reflected bullets.")]
        [SerializeField] private float _reflectDamageMult = 1.5f;

        [Tooltip("The radius for detecting projectiles during a swing.")]
        [SerializeField] private float _reflectRadius = 2.5f;

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            animations?.Play(AttackAnimation);
            
            Transform owner = animations?.Transform.root;
            if (owner != null) {
                // 1. Physical Hit (handled via MeleeWeapon logic usually, but here we add the reflector)
                HandleReflections(position, direction.normalized, owner);
            }

            StartCooldown();
        }

        private void HandleReflections(Vector3 origin, Vector2 direction, Transform player) {
            // Scan for projectiles (IBullet) in the swing arc
            Collider2D[] projectiles = Physics2D.OverlapCircleAll(origin, _reflectRadius);
            
            foreach (var col in projectiles) {
                // Try to get a bullet that isn't ours
                if (col.TryGetComponent<IBullet>(out var bullet)) {
                    // Reflection logic
                    bullet.SetOwner(player);
                    bullet.SetWeapon(this);
                    bullet.SetDamage(Mathf.CeilToInt(bullet.GetDamage() * _reflectDamageMult));
                    bullet.SetSpeed(bullet.GetSpeed() * _reflectSpeedMult);
                    
                    // Send it back in the direction we are aiming!
                    bullet.Fire(direction);
                    
                    PlayImpactSound(col.transform.position, "bat_reflect_ping");
                    Debug.Log($"[Casey] Reflected {col.name} with {bullet.GetDamage()} damage!");
                    
                    // Don't let it collide with us anymore locally
                    // This depends on the bullet's own internal logic, but usually Fire() handles the direction.
                }
            }
            
            // Standard melee hit detection is handled by the MeleeWeapon subclass when the animation triggers or via overlaps.
            // We'll add some extra 'oomph' to the standard melee hit.
            Collider2D[] enemies = Physics2D.OverlapCircleAll(origin, AttackSize.magnitude, _targetMask);
            foreach (var enemy in enemies) {
                if (enemy.TryGetComponent<CharacterHealth>(out var health) && !health.IsDead) {
                    var batHit = new HitDataBuilder()
                        .WithWeapon(this)
                        .WithDamage(GetDamage())
                        .WithDirection(direction)
                        .Build(player, enemy.transform);
                        
                    health.PerformDamage(batHit);
                }
            }
        }
    }
}
