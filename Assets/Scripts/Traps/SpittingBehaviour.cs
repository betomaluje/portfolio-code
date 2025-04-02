using Base;
using BerserkPixel.Health;
using Detection;
using UnityEngine;
using Weapons;

namespace Traps {
    public class SpittingBehaviour : MonoBehaviour, ICharacterHolder {
        [SerializeField]
        private ParticleSystem _particleSystem;

        [SerializeField]
        [Range(0f, 1f)]
        private float _chanceForDeathWeapon = .5f;

        [SerializeField]
        private AnimationConfig _animationConfig;

        [Header("Health")]
        [SerializeField]
        private CharacterHealth _characterHealth;

        [SerializeField]
        private Transform _deathParticles;

        [SerializeField]
        [Tooltip("Weapon that will be spawned when the entity dies")]
        private Transform _deathWeapon;

        private TargetDetection _enemyDetection;

        public CharacterAnimations Animations { get; private set; }
        public IMove Movement { get; private set; }
        public IWeaponManager WeaponManager { get; private set; }
        public IHealth Health { get => _characterHealth; }

        private void Awake() {
            Animations = new CharacterAnimations(GetComponent<Animator>(), _animationConfig);
            WeaponManager = GetComponent<IWeaponManager>();
            _enemyDetection = GetComponentInChildren<TargetDetection>();
        }

        private void OnEnable() {
            _characterHealth.OnDie += HandleDie;

            if (_enemyDetection != null) {
                _enemyDetection.OnPlayerDetected.AddListener(HandlePlayerDetected);
            }
        }

        private void OnDisable() {
            _characterHealth.OnDie -= HandleDie;

            if (_enemyDetection != null) {
                _enemyDetection.OnPlayerDetected.RemoveListener(HandlePlayerDetected);
            }
        }

        private void HandleDie() {
            Instantiate(_deathParticles, transform.position, Quaternion.identity);
            if (_chanceForDeathWeapon >= Random.value && _deathWeapon != null) {
                Instantiate(_deathWeapon, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }

        public void HandlePlayerDetected(Transform target) {
            var direction = (target.position - transform.position).normalized;
            WeaponManager.Attack(direction);

            if (_particleSystem != null) {
                _particleSystem.Play();
            }
        }
    }
}