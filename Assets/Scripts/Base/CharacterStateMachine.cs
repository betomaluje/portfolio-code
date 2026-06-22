using System.Linq;
using BerserkPixel.Health;
using BerserkPixel.StateMachine;
using Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using Weapons;

namespace Base {
    /// <summary>
    /// Serves as the fundamental anchor linking core character components (Animations, Movement, Health,
    /// Weaponry) with the underlying StateMachine pattern, creating a unified access facade.
    /// </summary>
    [RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(CharacterHealth))]
    [DisallowMultipleComponent]
    public class CharacterStateMachine<T> : StateMachine<T>, ICharacterHolder where T : MonoBehaviour {
        private const string SpriteTransform = "Sprite";

        [Space(8)]
        [Header("Animation Config")]
        [SerializeField]
        protected AnimationConfig animationConfig;

        [SerializeField, Required]
        protected Animator animator;

        [Space(8)]
        [Header("Movement Config")]
        [SerializeField, Required]
        protected Rigidbody2D _rigidbody;

        [SerializeField, Required, ChildGameObjectsOnly]
        [Tooltip("Target transform designated for visual orientation/flipping.")]
        protected Transform spriteTransform;

        [Space(8)]
        [Header("Health")]
        [SerializeField, Required]
        protected CharacterHealth characterHealth;

        [Space(8)]
        [Header("Attack Config")]
        [SerializeField, ChildGameObjectsOnly]
        [Tooltip("Designated collider used exclusively for offensive overlap hit detection.")]
        protected BoxCollider2D attackCollider;

        /// <summary>Centralized manager for parsing clip lengths and triggering parameter updates.</summary>
        public CharacterAnimations Animations { get; private set; }

        /// <summary>Interface governing physical Rigidbody forces, velocity, and visual scaling.</summary>
        public IMove Movement { get; private set; }

        /// <summary>Specialized bounds designated for collision checks during combat states.</summary>
        public BoxCollider2D AttackCollider { get; private set; }

        /// <summary>Interface maintaining the active inventory and combat execution capabilities.</summary>
        public IWeaponManager WeaponManager { get; private set; }

        public IHealth Health => characterHealth;
        public IHealthSetup HealthSetup => characterHealth;

        protected override void Awake() {
            base.Awake();
            Animations = new CharacterAnimations(animator, animationConfig);
            Movement = CreateMovement(_rigidbody, spriteTransform);
            WeaponManager = GetComponent<IWeaponManager>();
            AttackCollider = attackCollider;
        }

        protected void OnDestroy() {
            if (Movement is CharacterMovement characterMovement) {
                characterMovement.Dispose();
            }
        }

        protected override void OnValidate() {
            base.OnValidate();

            // Self-repair logic to auto-hook components from the hierarchy in Editor
            if (animator == null) this.FindInChildren(out animator);
            if (_rigidbody == null) TryGetComponent(out _rigidbody);
            if (characterHealth == null) TryGetComponent(out characterHealth);

            if (spriteTransform == null) {
                Transform foundSprite = transform.Find(SpriteTransform);
                if (foundSprite != null) {
                    spriteTransform = foundSprite;
                }
            }
        }

        protected virtual IMove CreateMovement(Rigidbody2D rigidbody, Transform spriteTransform) {
            return new CharacterMovement(rigidbody, spriteTransform);
        }

        [BoxGroup("Debug", order: 100)]
        [Button("Arrange States")]
        private void ArrangeStates() {
            if (_states != null) {
                var currentOrder = _states.OrderBy(state => state.name).ToList();
                if (!currentOrder.SequenceEqual(_states)) {
                    _states = currentOrder;
                }
            }
        }

        /// <summary>
        /// Grants transient health immunity. Used to simulate parrying or shield guarding behavior.
        /// </summary>
        public void Block() {
            characterHealth.SetImmune();
        }

        /// <summary>
        /// Disables health immunity, rendering the character vulnerable to incoming damage data.
        /// </summary>
        public void UnBlock() {
            characterHealth.ResetImmune();
        }
    }
}