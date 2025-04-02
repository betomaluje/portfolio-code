using System.Linq;
using BerserkPixel.Health;
using BerserkPixel.StateMachine;
using Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using Weapons;

namespace Base {
    [RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(CharacterHealth))]
    [DisallowMultipleComponent]
    public class CharacterStateMachine<T> : StateMachine<T>, ICharacterHolder where T : MonoBehaviour {
        private const string SpriteTransform = "Sprite";

        [Space(8)]
        [Header("Animation Config")]
        [SerializeField]
        protected AnimationConfig animationConfig;

        [SerializeField]
        [Required]
        protected Animator animator;

        [Space(8)]
        [Header("Movement Config")]
        [SerializeField]
        [Required]
        protected Rigidbody2D _rigidbody;

        [SerializeField]
        [Tooltip("Which transform are we going to flip")]
        [Required]
        [ChildGameObjectsOnly]
        protected Transform spriteTransform;

        [Space(8)]
        [Header("Health")]
        [SerializeField]
        [Required]
        protected CharacterHealth characterHealth;

        [Space(8)]
        [Header("Attack Config")]
        [SerializeField]
        [Tooltip("Assign the collider to use for hit detection")]
        [ChildGameObjectsOnly]
        protected BoxCollider2D attackCollider;

        public CharacterAnimations Animations { get; private set; }

        // everything move related: move, apply force and flip sprites
        public IMove Movement { get; private set; }

        // for raycast a hit detection
        public BoxCollider2D AttackCollider { get; private set; }

        public IWeaponManager WeaponManager { get; private set; }

        public IHealth Health { get => characterHealth; }

        protected override void Awake() {
            base.Awake();
            Animations = new CharacterAnimations(animator, animationConfig);
            Movement = new CharacterMovement(_rigidbody, spriteTransform);
            WeaponManager = GetComponent<IWeaponManager>();
            AttackCollider = attackCollider;
        }

        protected override void OnValidate() {
            base.OnValidate();
            if (animator == null && this.FindInChildren(out animator)) { }

            if (_rigidbody == null && TryGetComponent(out _rigidbody)) { }

            if (characterHealth == null && TryGetComponent(out characterHealth)) { }

            if (spriteTransform == null && transform.Find(SpriteTransform) != null) {
                transform.Find(SpriteTransform).TryGetComponent(out spriteTransform);
            }
        }

        [BoxGroup("Debug", order: 100)]
        [Button("Arrange States")]
        private void ArrangeStates() {
            if (_states != null) {
                var currentOrder = _states.OrderBy(state => state.name);
                if (!currentOrder.SequenceEqual(_states)) {
                    _states = currentOrder.ToList();
                }
            }
        }

        public void Block() {
            characterHealth.SetImmune();
        }

        public void UnBlock() {
            characterHealth.ResetImmune();
        }
    }
}