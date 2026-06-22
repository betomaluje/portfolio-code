using UnityEngine;
using Weapons;
using Base;
using Cysharp.Threading.Tasks;
using System;

namespace Weapons.DesignEx {
    /// <summary>
    /// A shoulder-mounted missile pod inspired by Enter the Gungeon.
    /// Mechanics: Rapidly fires 8 small homing missiles in a wider arc. 
    /// Missiles actively steer towards the nearest enemy in their detection cone.
    /// </summary>
    [RequiredBullet(typeof(HomingMissileBullet))]
    [CreateAssetMenu(fileName = "MissilePod", menuName = "Aurora/Weapons/Expanded/Micro-Missile Pod")]
    public class MissilePodLauncher : BaseShootingWeapon {
        
        [Header("Missile Volley")]
        [Tooltip("The number of missiles fired in each volley.")]
        [SerializeField] private int _missileCount = 8;
        
        [Tooltip("The spread angle (in degrees).")]
        [SerializeField] private float _volleySpread = 60.0f;

        [Tooltip("The time interval between missiles in a volley.")]
        [SerializeField] private float _burstRate = 0.05f;

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            if (HasAmmo()) {
                animations?.Play(AttackAnimation);
                
                FireVolley(direction, position, animations?.Transform.root).Forget();

                StartCooldown();
            } else {
                Reload(animations);
            }
        }

        private async UniTaskVoid FireVolley(Vector2 direction, Vector3 position, Transform player) {
            float baseAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            
            for (int i = 0; i < _missileCount; i++) {
                // Determine a spread angle within the volley
                float angleOffset = UnityEngine.Random.Range(-_volleySpread / 2f, _volleySpread / 2f);
                Quaternion rot = Quaternion.Euler(0, 0, baseAngle + angleOffset);
                Vector2 missileDir = rot * Vector2.up;

                if (BulletPrefab != null) {
                    var obj = Instantiate(BulletPrefab, position, Quaternion.identity);
                    if (obj.TryGetComponent<HomingMissileBullet>(out var missile)) {
                        missile.SetWeapon(this);
                        missile.SetOwner(player);
                        missile.Fire(missileDir);
                    }
                }
                
                PlayImpactSound(position, "missile_launch");
                
                await UniTask.Delay(TimeSpan.FromSeconds(_burstRate));
            }
            
            _ammo--; // Consume 1 ammo for the entire volley
        }
    }
}
