using System;
using Base;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Weapons {
    /// <summary>
    /// A melee weapon that performs an area-of-effect (AOE) circular attack centered on the player.
    /// It hits all enemies in every direction within a specific radius.
    /// Perfect for heavy "Ground Slam" or "Whirlwind" animations.
    /// </summary>
    [CreateAssetMenu(fileName = "CircleMeleeWeapon", menuName = "Aurora/Weapons/Circle Melee Weapon")]
    public class CircleMeleeWeapon : MeleeWeapon {
        [Tooltip("The side length of the square detection area (simulating a radius).")]
        [SerializeField] private float _attackRadius = 3f;

        [SerializeField] private ParticleSystem _circleEffect;
        [SerializeField] [Min(0)] private float _delayBeforeParticles = 0.15f;

        /// <summary>
        /// This weapon shouldn't move the collider based on aiming, 
        /// since it hits all around the character equally.
        /// </summary>
        /// <returns>Always returns false for radial attacks.</returns>
        public override bool ShouldMoveAttackCollider() => false;

        /// <summary>
        /// Returns a squared size to let the standard BoxCollider2D detect 
        /// an area that encompasses the given radius.
        /// </summary>
        public override Vector2 AttackSize => new(_attackRadius * 2, _attackRadius * 2);

        /// <summary>
        /// Centered at origin (0,0) relative to the owner character.
        /// </summary>
        public override Vector2 AttackOffset => Vector2.zero;

        /// <summary>
        /// Standard melee attack trigger.
        /// </summary>
        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown()) return;            

            // Trigger the "Radial" or generic attack animation
            animations?.Play(AttackAnimation);

            if (_circleEffect != null) {
                SpawnParticlesAsync(position).Forget();
            }
            
            StartCooldown();
        }

        private async UniTaskVoid SpawnParticlesAsync(Vector3 position) {
            if (_delayBeforeParticles > 0) {
                await UniTask.Delay(TimeSpan.FromSeconds(_delayBeforeParticles));
            }

            if (_circleEffect != null) {
                ParticleSystem temp = Instantiate(_circleEffect);
                temp.transform.position = position;
                temp.transform.localScale = Vector3.one * _attackRadius;
                temp.Play();
                Destroy(temp.gameObject, temp.main.duration);
            }
        }
    }
}
