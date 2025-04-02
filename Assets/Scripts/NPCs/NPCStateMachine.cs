using System;
using Base;
using BerserkPixel.Health;
using BerserkPixel.Prata;
using BerserkPixel.StateMachine;
using Detection;
using Interactable;
using JetBrains.Annotations;
using NPCs.Expressions;
using NPCs.States;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace NPCs {
    public class NPCStateMachine : StateMachine<NPCStateMachine>, IInteract {
        private const string SpriteTransform = "Sprite";

        [Header("Characteristics")]
        [SerializeField]
        private NPCCharacteristics _characteristics;

        [Space(8)]
        [Header("Animation Config")]
        [Required]
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
        private TargetDetection _playerDetection;

        [SerializeField]
        [Tooltip("Which transform are we going to flip")]
        [ChildGameObjectsOnly]
        [Required]
        protected Transform spriteTransform;

        [SerializeField]
        protected CharacterHealth _characterHealth;

        [SerializeField]
        private Transform[] _hitParticles;

        [Header("To Destroy Dead")]
        [SerializeField]
        private Object[] _toDestroyWhenDead;

        private NPCExpressionManager _expressionManager;

        // Prata
        [CanBeNull]
        private Interaction _interaction;

        private IInteractionHolder _interactionHolder;

        public Action<bool> OnRescued = delegate { };

        public CharacterAnimations Animations { get; private set; }

        public Transform PlayerTransform { get; private set; }

        // everything move related: move, apply force and flip sprites
        public IMove Movement { get; private set; }

        public bool HasBeenRescued { get; private set; }

        public CharacterHealth Health => _characterHealth;

        public NPCCharacteristics Characteristics {
            get => _characteristics;
            set {
                _characteristics = value;
                _characterHealth.SetupHealth(_characteristics.MaxHealth);
            }
        }

        protected override void Awake() {
            base.Awake();
            CloneStates();
            _expressionManager = GetComponent<NPCExpressionManager>();
            Animations = new CharacterAnimations(animator, animationConfig);
            Movement = new CharacterMovement(_rigidbody, spriteTransform);

            _interactionHolder = GetComponentInChildren<IInteractionHolder>();

            if (_interactionHolder != null) {
                _interaction = _interactionHolder.GetInteraction();
            }
        }

        protected override void OnValidate() {
            base.OnValidate();

            if (animator == null && TryGetComponent(out animator)) { }

            if (_rigidbody == null && TryGetComponent(out _rigidbody)) { }

            if (spriteTransform == null && transform.Find(SpriteTransform) != null) {
                transform.Find(SpriteTransform).TryGetComponent(out spriteTransform);
            }
        }

        protected override void Start() {
            base.Start();
            _characterHealth.SetupHealth(_characteristics.MaxHealth);
            HasBeenRescued = false;
        }

        private void OnEnable() {
            _characterHealth.OnDamagePerformed += HandleHurt;
            _characterHealth.OnDie += HandleDie;

            if (_playerDetection != null) {
                _playerDetection.OnPlayerDetected.AddListener(HandlePlayerDetected);
            }
        }

        private void OnDisable() {
            _characterHealth.OnDamagePerformed -= HandleHurt;
            _characterHealth.OnDie -= HandleDie;

            if (_playerDetection != null) {
                _playerDetection.OnPlayerDetected.RemoveListener(HandlePlayerDetected);
            }
        }

        private void HandlePlayerDetected(Transform player) {
            PlayerTransform = player;
        }

        public void CancelInteraction() {
            _interaction?.Cancel();
            DialogManager.Instance.HideDialog();
        }

        public void DoInteract() {
            if (HasBeenRescued) {
                SetExpression(ExpressionType.Happy);
                return;
            }

            if (_interaction == null || !_interaction.HasAnyDialogLeft()) {
                if (!HasBeenRescued) {
                    HasBeenRescued = true;
                    OnRescued?.Invoke(true);
                    DialogManager.Instance.HideDialog();
                    _interactionHolder?.Reset();
                }

                return;
            }

            // just to know if it's the first time talking to the NPC
            if (CurrentState != typeof(NPCDialogState)) {
                var dialogState = GetState(typeof(NPCDialogState)) as NPCDialogState;
                dialogState.SetInteraction(_interaction);

                SetState(dialogState);
            }
            else {
                var dialogState = GetState(typeof(NPCDialogState)) as NPCDialogState;
                dialogState.ContinueDialog();
            }

        }

        private void HandleHurt(HitData hitData) {
            if (Health.CurrentHealth <= 0) {
                return;
            }

            SpawnHitParticles();

            if (HasBeenRescued) {
                return;
            }

            // we run if hurt
            OnRescued?.Invoke(false);
            SetState(typeof(NPCWanderState));
        }

        private void SpawnHitParticles() {
            if (_hitParticles is not { Length: > 0 }) {
                return;
            }

            var index = Random.Range(0, _hitParticles.Length);
            Instantiate(_hitParticles[index], transform.position, Quaternion.identity);
        }

        private void HandleDie() {
            SetState(typeof(NPCDeadState));

            gameObject.layer = LayerMask.NameToLayer("Enemy");

            foreach (var t in _toDestroyWhenDead) {
                Destroy(t);
            }
        }

        public void MakeBodyKinematic() {
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        }

        public void MakeBodyDynamic() {
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }

        public void Rescue() {
            // if hasn't been rescued, we calculate chance 
            HasBeenRescued = true;
            SetExpression(ExpressionType.Happy);

            OnRescued?.Invoke(HasBeenRescued);
        }

        public void ResetRescued() {
            HasBeenRescued = false;
            OnRescued?.Invoke(false);
            _expressionManager.ResetExpression();
            CancelInteraction();
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