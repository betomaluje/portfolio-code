using Base;
using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// A cannon that fires metal shrapnel. 
    /// Mastery: Using ICharge (Holding Attack) activates the magnetic field.
    /// All active shards in the scene will rapidly return to the player, 
    /// ripping through enemies along the way.
    /// </summary>
    [CreateAssetMenu(fileName = "MagneticShrapnelCannon", menuName = "Aurora/Weapons/Expanded/Magnetic Shrapnel Cannon")]
    public class MagneticShrapnelCannon : BaseShootingWeapon, ICharge {
        
        [Header("Shrapnel Config")]
        [Tooltip("The number of shards fired in a single burst.")]
        [SerializeField] private int _shardsPerBurst = 5;

        [Tooltip("Time in seconds to reach maximum charge output.")]
        [SerializeField] private float _chargeTime = 2.0f;

        private float _chargeValue = 0f;

        /// <summary>
        /// Property for tracking magnetization state.
        /// Recalls shards if Charge is held above a threshold.
        /// </summary>
        public float Charge {
            set {
                _chargeValue = value;
                if (_chargeValue > 0.5f) {
                    RecallAllShards();
                }
            }
        }

        public float ChargeTime => _chargeTime;

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            // Attack fires shards normally
            if (HasAmmo()) {
                animations?.Play(AttackAnimation);

                for (int i = 0; i < _shardsPerBurst; i++) {
                    // Random spread for the shrapnel burst
                    Vector2 spreadDir = direction + Random.insideUnitCircle * 0.2f;
                    
                    if (BulletPrefab != null) {
                        var bullet = Instantiate(BulletPrefab, position, Quaternion.identity);
                        if (bullet.TryGetComponent<MagnetShrapnel>(out var shard)) {
                            shard.SetWeapon(this);
                            shard.SetOwner(animations?.Transform.root);
                            shard.Fire(spreadDir.normalized);
                        }
                    }
                }

                StartCooldown();
            } else {
                Reload(animations);
            }
        }

        private void RecallAllShards() {
            // Find all active shrapnel in the scene and trigger their recall
            // Efficiency note: Could track shards locally instead of FindObjectsOfType
            MagnetShrapnel[] shards = Object.FindObjectsByType<MagnetShrapnel>(FindObjectsSortMode.None);
            foreach (var shard in shards) {
                shard.ReturnToOwner();
            }
        }

    }
}
