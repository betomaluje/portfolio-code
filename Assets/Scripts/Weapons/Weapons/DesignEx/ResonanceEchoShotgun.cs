using Base;
using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// A shotgun-style weapon that fires slow sonic waves.
    /// Mastery: Waves reflect off walls, triggering a "Resonance Ripple" 
    /// that deals area damage at the bounce point, rewarding players for using terrain geometry.
    [RequiredBullet(typeof(SonicEchoBullet))]
    [CreateAssetMenu(fileName = "ResonanceEchoShotgun", menuName = "Aurora/Weapons/Expanded/Resonance Echo-Shotgun")]
    public class ResonanceEchoShotgun : BaseShootingWeapon {

        [Header("Resonance Properties")]
        [Tooltip("The radius of the sound ripple triggered on wall bounce.")]
        [SerializeField] private float _rippleRadius = 2.5f;

        [Tooltip("Number of times the wave can echo off walls.")]
        [SerializeField] private int _maxEchoes = 2;

        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;

            if (HasAmmo()) {
                animations?.Play(AttackAnimation);

                if (BulletPrefab != null) {
                    var bullet = Instantiate(BulletPrefab, position, Quaternion.identity);
                    if (bullet.TryGetComponent<SonicEchoBullet>(out var sonic)) {
                        sonic.SetWeapon(this);
                        sonic.SetOwner(animations?.Transform.root);
                        sonic.InitializeResonance(_rippleRadius, _maxEchoes);
                        sonic.Fire(direction.normalized);
                    }
                    else if (bullet.TryGetComponent<IBullet>(out var ibullet)) {
                        ibullet.SetWeapon(this);
                        ibullet.Fire(direction.normalized);
                    }
                }

                StartCooldown();
            } else {
                Reload(animations);
            }
        }

    }
}
