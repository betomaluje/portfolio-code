using System;
using Base;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BerserkPixel.Health {
    public class CharacterHealth : MonoBehaviour, IHealth, IHealthSetup {
        [Header("Config")]
        [SerializeField]
        private HealthConfig _healthConfig;

        [SerializeField]
        [Range(0f, 1f)]
        private float _chanceToBlock = 0.5f;

        [SerializeField]
        private float _timeForRecovery = 0.2f;

        [HideInInspector]
        public int CurrentHealth;

        public float CurrentHealthPercentage => CurrentHealth / (float)_maxHealth;
        public int ExperienceGained => _healthConfig.ExperienceToGive;
        public bool IsDead => CurrentHealth <= 0;
        public int MaxHealth => _maxHealth;

        private bool _isImmune;
        public bool IsImmune => _isImmune;

        private int _maxHealth;

        // used to see if an enemy can block damage.
        public event Action OnBlock = delegate { };
        public event Action<HitData> OnHitReceived = delegate { };
        public event Action<HitData> OnDamagePerformed = delegate { };
        public event Action<int> OnGiveHealth = delegate { };
        public event Action OnDie = delegate { };
        public event Action<int, int> OnHealthChanged = delegate { };
        public event Action<float> OnPercentageChanged = delegate { };

        public static event Action<HitData> OnAnyDamagePerformed = delegate { };

        private void Awake() {
            if (_maxHealth <= 0) {
                SetupHealth(_healthConfig.MaxHealth, _healthConfig.MaxHealth);
            }

            CalculatePercentage();
        }

        private void HealthChanged(int newHealth) {
            var wasPlayerDamaged = newHealth < CurrentHealth;
            CurrentHealth = newHealth;
            CalculatePercentage();

            OnHealthChanged?.Invoke(CurrentHealth, _maxHealth);

            if (wasPlayerDamaged && newHealth <= 0) {
                // invoke the "killed" remote event when hp is 0. 
                Die();
            }
        }

        private void Die() {
            OnDie?.Invoke();

            CurrentHealth = 0;
        }

        private void CalculatePercentage() {
            var healthPercentage = CurrentHealth / (float)_maxHealth;
            OnPercentageChanged?.Invoke(healthPercentage);
        }

        public void AutoDestruct() {
            _isImmune = false;
            var hitData = new HitDataBuilder()
                    .WithDamage(_maxHealth)
                    .WithDirection(Vector2.zero)
                    .Build(transform, transform);

            PerformDamage(hitData);
        }

        private bool CheckIfBlocked() => _chanceToBlock >= UnityEngine.Random.value;

        #region IHealthSetup

        public void SetupHealth(int maxHealth, int currentHealth, bool resetCurrentHealth = true) {
            if (maxHealth == _maxHealth) {
                return;
            }

            _maxHealth = maxHealth;

            if (resetCurrentHealth) {
                CurrentHealth = maxHealth;
            }
            else {
                CurrentHealth = currentHealth;
            }

            OnHealthChanged?.Invoke(CurrentHealth, _maxHealth);
        }

        public void AddMaxHealth(int amount) {
            if (amount <= 0) {
                return;
            }

            _maxHealth = _healthConfig.MaxHealth + amount;
            GiveHealth(amount);
        }

        #endregion

        #region IHealth        

        public bool CanGiveHealth() => CurrentHealth > 0 && CurrentHealth != _maxHealth;

        public void GiveHealth(int amount) {
            if (!CanGiveHealth()) {
                return;
            }

            var newHealth = CurrentHealth + amount;

            if (newHealth > _maxHealth) {
                newHealth = _maxHealth;
            }

            OnGiveHealth?.Invoke(newHealth);

            // Apply damage and modify the "heal" property.
            HealthChanged(newHealth);
        }

        /// <summary>
        /// Performs damage to the player.
        /// </summary>
        /// <param name="hitData"></param>
        public void PerformDamage(HitData hitData) {
            OnHitReceived?.Invoke(hitData);
            
            if (_isImmune) {
                return;
            }

            if (CurrentHealth <= 0) {
                OnDamagePerformed?.Invoke(hitData);
                return;
            }

            if (CheckIfBlocked()) {
                OnBlock?.Invoke();
                return;
            }

            Weapons.Weapon weapon = hitData.weapon;
            Vector3 direction = hitData.direction.normalized;
            int damage = hitData.damage;
            var newHealth = CurrentHealth - damage;

            // if hp is lower than 0, set it to 0.
            if (newHealth < 0) {
                newHealth = 0;
            }

            var wasPlayerDamaged = newHealth < CurrentHealth;

            // we only instantiate blood when it's damaged, not healing
            if (wasPlayerDamaged) {
                // damaged performed
                OnDamagePerformed?.Invoke(hitData);
                OnAnyDamagePerformed?.Invoke(hitData);
            }

            // Apply damage and modify the "damage" property.
            HealthChanged(newHealth);
            ImmuneCharacter();
        }

        private async void ImmuneCharacter() {
            _isImmune = true;
            await UniTask.Delay(TimeSpan.FromSeconds(_timeForRecovery));
            _isImmune = false;
        }

        public void SetImmune() => _isImmune = true;

        public void ResetImmune() => _isImmune = false;

        #endregion
    }
}