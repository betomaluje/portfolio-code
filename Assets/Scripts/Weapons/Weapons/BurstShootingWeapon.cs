using System.Collections;
using Base;
using UnityEngine;

namespace Weapons {
    /// <summary>
    /// A shooting weapon that fires multiple projectiles in quick succession with a single attack input.
    /// It balances high burst damage with a longer cooldown period.
    /// </summary>
    [CreateAssetMenu(fileName = "BurstShootingWeapon", menuName = "Aurora/Weapons/Burst Shooting Weapon")]
    public class BurstShootingWeapon : BaseShootingWeapon {
        [Tooltip("The number of bullets fired in one burst.")]
        [SerializeField] private int _burstAmount = 3;

        [Tooltip("The delay (in seconds) between individual shots within the burst.")]
        [SerializeField] private float _burstInterval = 0.1f;

        private bool _isBursting = false;

        /// <summary>
        /// Executes the burst fire logic. 
        /// Overrides the standard attack to initiate a coroutine for sequential firing.
        /// </summary>
        /// <param name="animations">The animation controller for the character.</param>
        /// <param name="direction">The aim direction for the burst.</param>
        /// <param name="position">The starting position for projectiles.</param>
        public override void Attack(CharacterAnimations animations, Vector2 direction, Vector3 position) {
            if (IsCoolingDown() || _isBursting) return;

            if (HasAmmo()) {
                animations?.Play(AttackAnimation);
                
                // We start the burst fire in a coroutine. 
                // Since this is a ScriptableObject, we need to use a MonoBehavior to run the coroutine.
                // We'll use the character's transform as the runner.
                if (animations != null) {
                    animations.Transform.GetComponent<MonoBehaviour>().StartCoroutine(ExecuteBurst(animations.Transform, direction, position));
                }
                
                StartCooldown();
            } else {
                Reload(animations);
            }
        }

        /// <summary>
        /// Handles the sequential firing within the burst.
        /// Tracks the owner transform for projectile assignment.
        /// </summary>
        /// <param name="owner">The transform of the character firing the weapon.</param>
        /// <param name="direction">Firing direction.</param>
        /// <param name="position">Starting world position for each bullet.</param>
        private IEnumerator ExecuteBurst(Transform owner, Vector2 direction, Vector3 position) {
            _isBursting = true;
            
            for (int i = 0; i < _burstAmount; i++) {
                if (!HasAmmo()) break;

                // Fire an individual projectile
                ShootBullet(position, direction, owner);

                // Wait before firing the next bullet in the burst
                yield return new WaitForSeconds(_burstInterval);
            }

            _isBursting = false;
        }

        /// <summary>
        /// Resets the internal bursting tag when the weapon is swapped or re-enabled.
        /// </summary>
        protected virtual void OnEnable() {
            _isBursting = false;
        }
    }
}
