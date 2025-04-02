using System;
using System.Collections.Generic;
using Base;
using BerserkPixel.Health;
using BerserkPixel.StateMachine;
using Interactable;
using NPCs.Expressions;
using Shop.Keeper.States;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Shop.Keeper {
    [RequireComponent(typeof(ShopKeeperRequirements))]
    public class ShopKeeperStateMachine : StateMachine<ShopKeeperStateMachine>, IInteract {
        private const string SpriteTransform = "Sprite";

        [Space(8)]
        [Header("Animation Config")]
        public AnimationConfig animationConfig;

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

        [SerializeField]
        protected CharacterHealth _characterHealth;

        private NPCExpressionManager _expressionManager;

        public CharacterAnimations Animations { get; private set; }

        // everything move related: move, apply force and flip sprites
        public IMove Movement { get; private set; }

        public event Action OnInteract = delegate { };
        public event Action OnCancelInteract = delegate { };

        protected override void Awake() {
            base.Awake();
            _expressionManager = GetComponent<NPCExpressionManager>();
            Animations = new CharacterAnimations(animator, animationConfig);
            Movement = new CharacterMovement(_rigidbody, spriteTransform);

            var idle = ScriptableObject.CreateInstance<ShopKeeperIdle>();
            var interact = ScriptableObject.CreateInstance<ShopKeeperInteract>();
            var runAway = ScriptableObject.CreateInstance<ShopKeeperRun>();
            var dead = ScriptableObject.CreateInstance<ShopKeeperDead>();
            var states = new List<State<ShopKeeperStateMachine>>() {
                idle, interact, runAway, dead
            };
            SetStates(states);
        }

        protected override void OnValidate() {
            base.OnValidate();

            if (animator == null && TryGetComponent(out animator)) { }

            if (_rigidbody == null && TryGetComponent(out _rigidbody)) { }

            if (spriteTransform == null && transform.Find(SpriteTransform) != null) {
                transform.Find(SpriteTransform).TryGetComponent(out spriteTransform);
            }

            if (_characterHealth == null && TryGetComponent(out _characterHealth)) { }
        }

        private void OnEnable() {
            _characterHealth.OnDamagePerformed += HandleHurt;
            _characterHealth.OnDie += HandleDie;
        }

        private void OnDisable() {
            _characterHealth.OnDamagePerformed -= HandleHurt;
            _characterHealth.OnDie -= HandleDie;
        }

        private void HandleHurt(HitData hitData) {
            SetState(typeof(ShopKeeperRun));
        }

        private void HandleDie() {
            SetState(typeof(ShopKeeperDead));
        }

        public void DoInteract() {
            SetState(typeof(ShopKeeperInteract));
            OnInteract?.Invoke();
        }

        public void CancelInteraction() {
            SetState(typeof(ShopKeeperWander));
            OnCancelInteract?.Invoke();
        }

        public void MakeBodyKinematic() {
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        }

        public void MakeBodyDynamic() {
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }

        /// <summary>
        ///     Gets a NPCExpressionState from a predetermined ExpressionType
        /// </summary>
        /// <param name="type"></param>
        public void SetExpression(ExpressionType type) {
            _expressionManager.SetExpression(type);
        }
    }
}