using System.Collections.Generic;
using UnityEngine;
using Weapons;

namespace Base {
    /// <summary>
    /// Manages character animations, acting as a bridge between the Animator 
    /// and the game's state machines to cleanly trigger animation clips and query their durations.
    /// </summary>
    public class CharacterAnimations {
        private const float TransitionDuration = 0.1f;
        
        private readonly AnimationConfig _animationConfig;
        private readonly Animator _animator;

        public Transform Transform => _animator.gameObject.transform;

        /// <summary>
        /// Fallback durations utilized primarily for blend tree animations where explicit 
        /// clip lengths cannot be easily queried from the runtime controller.
        /// </summary>
        private readonly Dictionary<string, float> _fallbackDurations = new() {
            { "Charge", 0.85f },
            { "Idle", 1f },
        };

        public CharacterAnimations(Animator animator, AnimationConfig animationConfig) {
            _animator = animator;
            _animationConfig = animationConfig;
        }

        /// <summary>
        /// Attempts to play an animation by its string name if the animator is active.
        /// </summary>
        public void Play(string animationName) {
            if (_animator.gameObject.activeInHierarchy && _animator.enabled) {
                TryPlayAnimation(animationName);
            }
        }

        /// <summary>
        /// Sets a float parameter on the underlying Animator, useful for blend trees or locomotion speeds.
        /// </summary>
        public void SetFloatParameter(int parameterName, float value) {
            if (_animator.gameObject.activeInHierarchy && _animator.enabled) {
                _animator.SetFloat(parameterName, value);
            }
        }

        public void PlayIdle() => Play("Idle");
        public void PlayInteract() => Play("Interact");
        public void PlayAttack() => Play("Attack");
        public void PlayAttack(AttackType attackType) => Play(GetAttackAnimation(attackType));
        public void PlayRun() => Play("Run");
        public void PlayRoll() => Play("Roll");
        public void PlayHurt() => Play("Hurt");
        public void PlayDead() => Play("Death");
        public void PlayBlock() => Play("Block");

        public void PlayCharge() {
            if (_animationConfig.Animations.ContainsKey("Charge")) {
                Play("Charge");
            } else {
                PlayRun();
            }
        }

        private string GetAttackAnimation(AttackType attackType) {
            return attackType == AttackType.Hammer ? "Heavy Weapon" : "Short Weapon";
        }

        private void TryPlayAnimation(string toPlay) {
            if (string.IsNullOrEmpty(toPlay) || _animationConfig == null) return;

            if (_animationConfig.GetAnimation(toPlay, out var animationHash) && animationHash != -1) {
                _animator.CrossFadeInFixedTime(animationHash, TransitionDuration);
            }
        }

        /// <summary>
        /// Retrieves the duration of the attack animation specific to the given weapon type.
        /// </summary>
        public float GetAnimationLength(AttackType attackType) {
            return GetAnimationLength(GetAttackAnimation(attackType));
        }

        /// <summary>
        /// Resolves the length of an animation clip in seconds. 
        /// Falls back to predefined dictionaries or a generic default if the clip is untraceable.
        /// </summary>
        public float GetAnimationLength(string toPlay) {
            const float defaultTime = 3f;

            if (_animationConfig == null) return defaultTime;

            if (_animationConfig.GetAnimation(toPlay, out var animationHash)) {
                foreach (AnimationClip clip in _animator.runtimeAnimatorController.animationClips) {
                    if (Animator.StringToHash(clip.name) == animationHash || clip.name == toPlay || clip.name.Contains(toPlay)) {
                        return clip.length;
                    }
                }
            }

            if (_fallbackDurations.TryGetValue(toPlay, out var duration)) {
                return duration;
            }

            return defaultTime;
        }
    }
}