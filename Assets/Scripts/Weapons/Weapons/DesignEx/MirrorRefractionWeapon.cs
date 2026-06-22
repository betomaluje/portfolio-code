using UnityEngine;
using Base;
using Weapons;

namespace Weapons.DesignEx {
    /// <summary>
    /// A sword that creates ghost-like mirror images in a fan.
    /// Mechanics: Attacking with a full charge (ICharge) creates mirror copies 
    /// that mirror the player's position and strike.
    /// </summary>
    [RequiredBullet(typeof(MirrorRefractionBullet))]
    [CreateAssetMenu(fileName = "MirrorShatterBlade", menuName = "Aurora/Weapons/Expanded/Mirror Shatter Blade")]
    public class MirrorRefractionWeapon : MeleeWeapon, ICharge {
        
        [Header("Mirror Prefabs")]
        [Tooltip("The ghost slash prefab for mirror images.")]
        [SerializeField] public GameObject BulletPrefab;
        
        [Header("Mirror Properties")]
        [Tooltip("The number of mirror reflections generated.")]
        [SerializeField] private int _reflectionCount = 3;

        [Tooltip("The spread angle of the mirror fan.")]
        [SerializeField] private float _spreadFanAngle = 45f;

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
            
            // Only spawn mirrors if charge is above 0.5f
            if (_chargeValue > 0.5f) {
                SpawnMirrorFan(animations?.Transform.root, direction, position);
            }

            StartCooldown();
            _chargeValue = 0f;
        }

        private void SpawnMirrorFan(Transform owner, Vector2 forwardDir, Vector3 spawnPoint) {
            float startAngle = -_spreadFanAngle / 2f;
            float step = _spreadFanAngle / (_reflectionCount - 1);
            
            for (int i = 0; i < _reflectionCount; i++) {
                float currentAngle = startAngle + (step * i);
                Vector2 reflectDir = Quaternion.Euler(0, 0, currentAngle) * forwardDir;
                
                if (BulletPrefab != null) {
                    var mirror = Instantiate(BulletPrefab, spawnPoint, Quaternion.identity);
                    if (mirror.TryGetComponent<MirrorRefractionBullet>(out var refComponent)) {
                        refComponent.SetWeapon(this);
                        refComponent.SetOwner(owner);
                        refComponent.Fire(reflectDir.normalized);
                    }
                }
            }
            
            PlayImpactSound(spawnPoint, "mirror_glass_shink");
        }
    }
}
