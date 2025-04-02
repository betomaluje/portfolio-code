using System.Linq;
using Base;
using BerserkPixel.Health;
using BerserkPixel.Health.FX;
using BerserkPixel.StateMachine;
using Camera;
using Companions.States;
using Extensions;
using Interactable;
using Sirenix.OdinInspector;
using Sounds;
using UnityEngine;
using Weapons;

namespace Companions {
    public class CompanionStateMachine : StateMachine<CompanionStateMachine>, IInteract, IHealth {
        private const string SpriteTransform = "Sprite";

        [SerializeField]
        private bool _isWild = false;

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
        [Header("Attack Config")]
        [SerializeField]
        [Tooltip("Assign the collider to use for hit detection")]
        [ChildGameObjectsOnly]
        protected BoxCollider2D attackCollider;

        public IWeaponManager WeaponManager { get; private set; }

        public BoxCollider2D AttackCollider { get; private set; }

        public CharacterAnimations Animations { get; private set; }

        public CompanionExpressionManager ExpressionManager { get; private set; }

        public CompanionSounds Sounds { get; private set; }

        // everything move related: move, apply force and flip sprites
        public IMove Movement { get; private set; }

        private IFX[] _allFxs;

        protected override void Awake() {
            base.Awake();
            CloneStates();
            Animations = new CharacterAnimations(animator, animationConfig);
            Movement = new CharacterMovement(_rigidbody, spriteTransform);
            WeaponManager = GetComponent<IWeaponManager>();
            ExpressionManager = GetComponent<CompanionExpressionManager>();
            Sounds = GetComponent<CompanionSounds>();
            AttackCollider = attackCollider;
            _allFxs = GetComponentsInChildren<IFX>();
        }

        protected override void OnValidate() {
            base.OnValidate();
            if (animator == null && this.FindInChildren(out animator)) { }

            if (_rigidbody == null && TryGetComponent(out _rigidbody)) { }

            if (spriteTransform == null && transform.Find(SpriteTransform) != null) {
                transform.Find(SpriteTransform).TryGetComponent(out spriteTransform);
            }

            if (Sounds == null) {
                Sounds = gameObject.GetOrAdd<CompanionSounds>();
            }
        }

        public void Tint(Color color) {
            if (spriteTransform != null && spriteTransform.TryGetComponent(out SpriteRenderer spriteRenderer)) {
                spriteRenderer.color = color;
            }
        }

        public void MakeBodyKinematic() {
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        }

        public void MakeBodyDynamic() {
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }

        /// <summary>
        /// Check if the creature is wild or not. If it is it will perform states such as Wander.
        /// </summary>
        /// <returns>True if the creature is wild, false otherwise.</returns>
        public bool IsWild() => _isWild;

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

        #region IInteract
        public void DoInteract() {
            if (CurrentState == typeof(CompanionFollowState)) {
                ExpressionManager.SetExpression("Sleep");
                SetState(typeof(CompanionSleepState));
            }
            else {
                SetState(typeof(CompanionFollowState));
            }
        }

        public void CancelInteraction() { }
        #endregion

        #region IHealth
        public void SetupHealth(int maxHealth) { }

        public void PerformDamage(HitData hitData) {
            SoundManager.instance.Play("hit");
            CinemachineCameraShake.Instance.ShakeCamera(transform);

            foreach (var fx in _allFxs) {
                fx.DoFX(hitData);
            }
        }

        public void GiveHealth(int health) { }

        public bool CanGiveHealth() => false;
        #endregion
    }
}