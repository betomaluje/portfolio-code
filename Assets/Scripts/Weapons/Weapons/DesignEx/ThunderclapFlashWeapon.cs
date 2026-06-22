using UnityEngine;
using Base;
using Weapons;
using BerserkPixel.Health;

namespace Weapons.DesignEx {
    /// <summary>
    /// A high-speed dash katana inspired by "Thunderclap and Flash".
    /// Mechanics: Charge attack (ICharge). On release, player instantly 
    /// teleports/dashes to max range. All enemies in the trail take crit damage.
    /// </summary>
    [CreateAssetMenu(fileName = "ThunderclapFlash", menuName = "Aurora/Weapons/Expanded/Thunderclap & Flash Katana")]
    public class ThunderclapFlashWeapon : MeleeWeapon, ICharge {
        
        [Tooltip("The targets that can be hit.")]
        [SerializeField] private LayerMask _targetMask;

        [Tooltip("The layers that block the dash (walls, etc).")]
        [SerializeField] private LayerMask _obstaclesMask;

        [Header("Godspeed Properties")]
        [Tooltip("Maximum dash distance when fully charged.")]
        public float MaxDashDistance = 12.0f;
        
        [Tooltip("The width of the path strike.")]
        public float StrikeWidth = 1.5f;

        [Tooltip("Time in seconds to reach maximum charge output.")]
        [SerializeField] private float _chargeTime = 2.0f;

        private float _chargeValue;

        public float Charge {
            set => _chargeValue = Mathf.Clamp01(value);
        }

        public float ChargeTime => _chargeTime;

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            animations?.Play(AttackAnimation);
            
            float currentDistance = Mathf.Lerp(MaxDashDistance * 0.2f, MaxDashDistance, _chargeValue);
            
            Transform owner = animations?.Transform.root;
            if (owner != null) {
                ExecutePathStrike(owner, direction.normalized, currentDistance);
            }

            StartCooldown();
            
            // Should reset the charge after the teleport
            _chargeValue = 0f;
        }

        private void ExecutePathStrike(Transform player, Vector2 direction, float distance) {
            Vector2 startPos = player.position;
            Vector2 endPos = startPos + direction * distance;
            
            // 1. Raycast for walls to prevent teleporting through geometry
            RaycastHit2D wallHit = Physics2D.Raycast(startPos, direction, distance, _obstaclesMask);
            if (wallHit.collider != null) {
                endPos = wallHit.point - direction * 0.5f; // Snap to wall with buffer
            }

            // 2. Teleport
            if (player.TryGetComponent<ICharacterHolder>(out var holder)) {
                holder.Movement.MoveToPoint(endPos, 0.25f);
            } else {
                player.position = endPos;
            }

            // 3. Area of Effect damage in the path rectangle
            // Width and length are the striking zone
            float strikeLength = Vector2.Distance(startPos, endPos);
            Vector2 pathCenter = (startPos + endPos) / 2f;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            Collider2D[] trappedRect = Physics2D.OverlapBoxAll(pathCenter, new Vector2(strikeLength, StrikeWidth), angle, _targetMask);
            
            foreach (var col in trappedRect) {
                if (col.TryGetComponent<CharacterHealth>(out var health) && !health.IsDead) {
                    var flashHit = new HitDataBuilder()
                        .WithWeapon(this)
                        .WithDamage(Mathf.CeilToInt(GetDamage() * (1.5f + _chargeValue))) // Scales with charge
                        .WithCriticalHitChance(0.5f + _chargeValue * 0.5f) // High crit chance on dash
                        .WithDirection(direction)
                        .Build(player, col.transform);
                    
                    health.PerformDamage(flashHit);
                }
            }

            // VFX: Should spawn a "Thunder Path" trail from start to end
            PlayImpactSound(startPos, "thunder_flash");
            // SpawnCollisionParticles(endPos);
        }
    }
}
