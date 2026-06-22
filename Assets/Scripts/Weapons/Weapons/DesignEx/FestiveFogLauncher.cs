using UnityEngine;
using Weapons;
using Base;

namespace Weapons.DesignEx {
    /// <summary>
    /// A lobbing weapon that creates a "Festive Fog" area.
    /// Enemies inside the fog are intoxicated (slowed) and take damage over time.
    /// Lobs a slow-moving, arcing bottle that shatters on impact. It releases a lingering toxin 
    /// fog that persists for 5 seconds. Every half-second, it deals tick damage to all enemies 
    /// within its 4-meter radius
    /// </summary>
    [RequiredBullet(typeof(FestiveFogProjectile))]
    [CreateAssetMenu(fileName = "FestiveFogLauncher", menuName = "Aurora/Weapons/Expanded/Festive Fog Launcher")]
    public class FestiveFogLauncher : BaseShootingWeapon {
        
        [Header("Fog Configuration")]
        [Tooltip("The radius of the fog cloud created on impact.")]
        [SerializeField] private float _fogRadius = 4.0f;
        
        [Tooltip("How long the fog lingers.")]
        [SerializeField] private float _fogDuration = 5.0f;

        [Tooltip("Tick rate in seconds (how often it deals damage).")]
        [SerializeField] private float _tickRate = 0.5f;

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            if (HasAmmo()) {
                animations?.Play(AttackAnimation);
                
                if (BulletPrefab != null) {
                    var obj = Instantiate(BulletPrefab, position, Quaternion.identity);
                    if (obj.TryGetComponent<FestiveFogProjectile>(out var fog)) {
                        fog.SetWeapon(this);
                        fog.InitializeFog(_fogRadius, _fogDuration, _tickRate);
                        fog.Fire(direction);
                    }
                }

                StartCooldown();
            } else {
                Reload(animations);
            }
        }
    }
}
