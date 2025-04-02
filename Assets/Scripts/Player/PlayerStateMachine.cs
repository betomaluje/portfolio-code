using System;
using Base;
using BerserkPixel.Health;
using Camera;
using Dungeon;
using Modifiers.Skills;
using Player.Components;
using Player.Input;
using Player.States;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Player {
    public class PlayerStateMachine : CharacterStateMachine<PlayerStateMachine> {
        private const float HitDamageMultiplier = 0.2f;

        [SerializeField]
        [Tooltip("Assign the collider to use for hit detection")]
        [ChildGameObjectsOnly]
        protected Collider2D _interactCollider;

        [SerializeField]
        [Tooltip("Assign the collider to use for hit detection")]
        [ChildGameObjectsOnly]
        protected Collider2D _characterCollider;

        [SerializeField]
        private ParticleSystem _moveParticles;

        // shows the player input at any time
        public Vector2 PlayerInputMovement { get; private set; }
        public Vector2 PlayerAimDirection { get; private set; }
        public Vector2 LastDirection { get; private set; }

        // returns if the player is moving
        public bool IsMoving => PlayerInputMovement.sqrMagnitude != 0f;

        public Collider2D InteractCollider { get; private set; }
        public PlayerLuckController LuckController { get; private set; }

        public Collider2D CharacterCollider => _characterCollider;

        public PlayerSounds Sounds { get; private set; }

        public HitComboCounter HitComboCounter { get; private set; }

        public ICharacterSkills Skills { get; private set; }

        public Action<HitData> OnHit = delegate { };

        private IPlayerInput _playerInput;

        private GroundDetection _groundDetection;

        protected override void Awake() {
            base.Awake();
            _playerInput = GetComponent<IPlayerInput>();
            _groundDetection = GetComponentInChildren<GroundDetection>();
            Skills = GetComponent<ICharacterSkills>();
            Sounds = GetComponent<PlayerSounds>();
            LuckController = GetComponent<PlayerLuckController>();
            InteractCollider = _interactCollider;
            HitComboCounter = new HitComboCounter();
        }

        protected override void Start() {
            base.Start();

            if (_moveParticles != null) {
                Movement.MovementParticles = _moveParticles;
            }

            SetState(typeof(IdleState));
        }

        protected override void Update() {
            base.Update();
            HitComboCounter.Tick();
            PlayerInputMovement = _playerInput.GetMoveDirection();
            PlayerAimDirection = _playerInput.GetAimDirection();

            Movement.FlipSprite(PlayerAimDirection);

            if (PlayerInputMovement.sqrMagnitude > .1f) {
                LastDirection = PlayerInputMovement;
            }
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)PlayerAimDirection * 3f);
        }

        private void OnEnable() {
            characterHealth.OnDamagePerformed += HandleHurt;
            characterHealth.OnDie += HandleDie;

            _playerInput.SouthButtonEvent += HandleAttack;
            _playerInput.EastButtonEvent += HandleRoll;
            _playerInput.WestButtonEvent += HandlePowerup;
        }

        private void OnDisable() {
            characterHealth.OnDamagePerformed -= HandleHurt;
            characterHealth.OnDie -= HandleDie;

            _playerInput.SouthButtonEvent -= HandleAttack;
            _playerInput.EastButtonEvent -= HandleRoll;
            _playerInput.WestButtonEvent -= HandlePowerup;
        }

        private void HandlePowerup() => SetState(typeof(ActivateSkillsState));

        private void HandleHurt(HitData hitData) {
            SetState(typeof(HitState));
            CinemachineCameraShake.Instance.ShakeCameraWithIntensity(transform, 1f);
        }

        private void HandleDie() {
            // show some game over screen
            SetState(typeof(DeadState));
            DungeonWinCounter.Instance.AddLoss();
            // HERE we decide if we remove money or weapons to reset the player or not
            // characterHealth.SetHealth(0);
            // GetComponent<PlayerPersistence>().SavePlayer();
        }

        private void HandleRoll() {
            if (IsMoving) {
                if (_groundDetection.ShouldJump(PlayerInputMovement)) {
                    SetState(typeof(JumpState));
                }
                else {
                    SetState(typeof(RollState));
                }
            }
        }

        private void HandleAttack(bool isPressed) {
            if (isPressed) {
                SetState(typeof(SouthButtonState));
            }
        }

        /// <summary>
        /// Called from the AttackState when there is a hit. Adds a hit to the combo
        /// </summary>
        public void AddEnemiesHit(int enemiesHit) {
            if (enemiesHit > 0) {
                HitComboCounter.Hit(enemiesHit);
            }
        }

        /// <summary>
        /// Called from the AttackState when there is a hit. Propagates the hit using the OnHit event
        /// </summary>
        /// <param name="hitData"></param>
        public void AddHitData(HitData hitData) => OnHit?.Invoke(hitData);

        public int CalculateWeaponDamage() {
            // from Cult of the Lamb https://cult-of-the-lamb.fandom.com/wiki/Weapons
            //[Base value] × (1 + (0.13 + 0.07×[Weapon level]) + [Tarot Card multiplier] + [Fleece multiplier] + [Run Damage multiplier] × [Difficulty modifier]
            var adjustedDamage = WeaponManager.Weapon.GetDamage() + Mathf.RoundToInt(HitComboCounter.CurrentHits * HitDamageMultiplier);
            // maybe add some player character strength
            // DebugTools.DebugLog.Log($"Adjusted damage: {adjustedDamage}");

            return adjustedDamage;
        }
    }
}