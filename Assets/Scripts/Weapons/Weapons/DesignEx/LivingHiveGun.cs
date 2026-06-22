using Base;
using UnityEngine;
using System.Collections.Generic;

namespace Weapons.DesignEx {
    /// <summary>
    /// Configuration for a Living Swarm weapon. 
    /// Fires Stinger Bits that aren't linear bullets; they are small flocking entities.
    /// Mastery: Holding ICharge concentration pheromones on the aim point.
    /// Releasing ICharge returns them to a "Scatter-Shield" cloud around the player.
    /// </summary>
    [RequiredBullet(typeof(StingerBitComponent))]
    [CreateAssetMenu(fileName = "LivingHiveGun", menuName = "Aurora/Weapons/Expanded/Living Hive Gun")]
    public class LivingHiveGun : BaseShootingWeapon, ICharge {
        
        [Header("Swarm Properties")]
        [Tooltip("Maximum amount of active stingers in the swarm.")]
        [SerializeField] private int _maxSwarmSize = 10;
        
        [Tooltip("The speed at which stingers move towards a pheromone target.")]
        [SerializeField] private float _swarmSpeed = 12f;

        [Tooltip("Time in seconds to reach maximum charge output.")]
        [SerializeField] private float _chargeTime = 2.0f;

        private List<StingerBitComponent> _activeSwarm = new();
        private float _chargeValue;
        private Vector2 _aimDirection;

        /// <summary>
        /// Pass current charge (concentration) to the swarmers.
        /// </summary>
        public float Charge {
            set {
                _chargeValue = value;
                UpdateSwarmBehavior();
            }
        }

        public float ChargeTime => _chargeTime;

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            _aimDirection = direction;
            
            // Firing logic: Only replenish if we under the limit
            if (IsCoolingDown()) return;

            if (_activeSwarm.Count < _maxSwarmSize) {
                animations?.Play(AttackAnimation);
                
                if (BulletPrefab != null) {
                    var bullet = Instantiate(BulletPrefab, position, Quaternion.identity);
                    if (bullet.TryGetComponent<StingerBitComponent>(out var bit)) {
                        _activeSwarm.Add(bit);
                        bit.SetWeapon(this);
                        bit.SetOwner(animations?.Transform.root);
                        bit.InitializeSwarm(_swarmSpeed, this);
                    }
                }
                
                StartCooldown();
            }
        }

        private void UpdateSwarmBehavior() {
            foreach (var bit in _activeSwarm) {
                if (bit == null) continue;
                
                if (_chargeValue > 0.5f) {
                    // CONCENTRATED ATTACK at aim point
                    bit.SetTargetState(true, _aimDirection);
                } else {
                    // DEFENSIVE SCATTER / DRIFT around player
                    bit.SetTargetState(false, _aimDirection);
                }
            }
            
            // Cleanup dead stingers
            _activeSwarm.RemoveAll(x => x == null);
        }

    }
}
