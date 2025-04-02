using Base;
using BerserkPixel.StateMachine;
using Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Enemies {
	public class EnemyAnimationStateMachine : StateMachine<EnemyAnimationStateMachine> {
        [SerializeField]
        protected AnimationConfig animationConfig;
        [SerializeField]
        [Required]
        protected Animator animator;

        public CharacterAnimations Animations { get; private set; }

        protected override void Awake() {
            base.Awake();
            Animations = new CharacterAnimations(animator, animationConfig);
        }

        protected override void OnValidate() {
            base.OnValidate();
            if (animator == null && this.FindInChildren(out animator)) { }
        }
    }
}