using UnityEngine;
using Weapons;

namespace Base {
    public class CharacterAnimations {
        private const float TransitionDuration = .1f;
        private readonly AnimationConfig _animationConfig;
        private readonly Animator _animator;

        public CharacterAnimations(Animator animator, AnimationConfig animationConfig) {
            _animator = animator;
            _animationConfig = animationConfig;
        }

        public void Play(string animationName) {
            TryPlayAnimation(animationName);
        }

        public void PlayIdle() {
            Play("Idle");
        }

        public void PlayInteract() {
            Play("Interact");
        }

        private string GetAttackAnimation(AttackType attackType) {
            return attackType == AttackType.Hammer ? "Heavy Weapon" : "Short Weapon";
        }

        public void PlayAttack() {
            Play("Attack");
        }

        public void PlayAttack(AttackType attackType) {
            Play(GetAttackAnimation(attackType));
        }

        public void PlayRun() {
            Play("Run");
        }

        public void PlayRoll() {
            Play("Roll");
        }

        public void PlayHurt() {
            Play("Hurt");
        }

        public void PlayDead() {
            Play("Death");
        }

        public void PlayBlock() {
            Play("Block");
        }

        private void TryPlayAnimation(string toPlay) {
            if (string.IsNullOrEmpty(toPlay)) {
                return;
            }

            if (_animationConfig == null) {
                // DebugTools.DebugLog.LogError($"Animation Config is null, cannot play animation {toPlay}");
                return;
            }

            if (_animationConfig.GetAnimation(toPlay, out var animation) && animation != -1) {
                _animator.CrossFadeInFixedTime(animation, TransitionDuration);
            }
            else {
                // DebugTools.DebugLog.LogWarning($"Can't play animation {toPlay}. Not in config.");
            }
        }

        public float GetAnimationLength(AttackType attackType) {
            return GetAnimationLength(GetAttackAnimation(attackType));
        }

        public float GetAnimationLength(string toPlay) {
            float defaultTime = 3f;

            if (_animationConfig == null) {
                return defaultTime;
            }

            if (_animationConfig.GetAnimation(toPlay, out var animation)) {

                foreach (AnimationClip clip in _animator.runtimeAnimatorController.animationClips) {
                    if (Animator.StringToHash(clip.name) == animation) {
                        return clip.length;
                    }
                }
            }

            return defaultTime;
        }
    }
}