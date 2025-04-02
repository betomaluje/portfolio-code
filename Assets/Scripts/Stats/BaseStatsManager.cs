using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using UnityEngine;
using Utils;

namespace Stats {
    [RequireComponent(typeof(ICharacterHolder))]
    [DisallowMultipleComponent]
    public abstract class BaseStatsManager : MonoBehaviour, IStatsModifier {
        [SerializeField]
        protected ICharacterHolder _characterHolder;

        public event Action<StatType, float> OnStatModifierAdded = delegate { };
        public abstract event Action<StatType, float> OnStatModifierReset;

        private readonly Dictionary<StatType, CountdownTimer> _timersPerType = new();
        private StatType? _latestTimerType;

        protected virtual void Awake() {
            _characterHolder = GetComponent<ICharacterHolder>();
        }

        protected virtual void OnValidate() {
            _characterHolder ??= GetComponent<ICharacterHolder>();
        }

        private void OnDestroy() {
            if (_timersPerType == null || _timersPerType.Count == 0) {
                return;
            }

            foreach (var timer in _timersPerType.Values) {
                if (timer != null && timer.IsRunning) {
                    timer.Stop();
                }
            }
        }

        private void Update() {
            if (_timersPerType.Count == 0) {
                return;
            }

            foreach (var timer in _timersPerType.Values) {
                if (timer != null && timer.IsRunning) {
                    timer.Tick(Time.deltaTime);
                }
            }
        }

        protected void SetTimer(StatType type, float duration) {
            if (!_timersPerType.TryGetValue(type, out CountdownTimer timer)) {
                timer = new CountdownTimer(duration);
                _timersPerType.Add(type, timer);
            }
            else {
                if (timer.IsRunning) {
                    return;
                }

                timer.Stop();
                timer.Reset(duration);
            }

            _latestTimerType = type;

            timer.OnTimerStop += HandleTimerStop;

            timer.Start();
        }

        private void HandleTimerStop() {
            if (_latestTimerType != null) {
                ResetStatModifier(_latestTimerType ?? StatType.Speed);
                _latestTimerType = null;
            }
        }

        public void AddStatModifier(StatType type, float amount, float duration = 0) {
            switch (type) {
                case StatType.Speed:
                    AddSpeed(amount);
                    break;
                case StatType.Attack:
                    AddAttack(amount);
                    break;
                case StatType.Defense:
                    AddDefense(amount);
                    break;
                case StatType.HealRate:
                    AddHealRate(amount);
                    break;
                case StatType.CriticalHitChance:
                    AddCriticalHitChance(amount);
                    break;
            }

            if (duration > 0) {
                SetTimer(type, duration);
            }
        }

        public virtual void ResetStatModifier(StatType type) {
            switch (type) {
                case StatType.Speed:
                    ResetSpeed();
                    break;
                case StatType.Attack:
                    ResetAttack();
                    break;
                case StatType.Defense:
                    ResetDefense();
                    break;
                case StatType.HealRate:
                    ResetHealRate();
                    break;
                case StatType.CriticalHitChance:
                    ResetCriticalHitChance();
                    break;
            }
        }

        public virtual void SaveStats() { }

        #region Add Stat Methods

        public virtual void AddSpeed(float amount) {
            if (_characterHolder.Movement == null || amount < 0f) {
                return;
            }

            _characterHolder.Movement.SetMovementInfluence(amount);

            OnStatModifierAdded?.Invoke(StatType.Speed, amount);
        }

        public virtual void AddAttack(float amount) {
            if (_characterHolder.WeaponManager == null || amount <= 0f) {
                return;
            }

            _characterHolder.WeaponManager.SetStrengthInfluence(amount);

            OnStatModifierAdded?.Invoke(StatType.Attack, amount);
        }

        public virtual void AddDefense(float amount) {
            OnStatModifierAdded?.Invoke(StatType.Defense, amount);
        }

        public virtual void AddHealRate(float amount) {
            if (_characterHolder.Health == null || !_characterHolder.Health.CanGiveHealth()) {
                return;
            }

            _characterHolder?.Health?.GiveHealth(Mathf.FloorToInt(amount));

            OnStatModifierAdded?.Invoke(StatType.HealRate, amount);
        }


        public virtual void AddCriticalHitChance(float amount) {
            OnStatModifierAdded?.Invoke(StatType.CriticalHitChance, amount);
        }

        #endregion

        #region Reset Stat Methods

        public abstract void ResetSpeed();

        public abstract void ResetAttack();

        public abstract void ResetDefense();

        public abstract void ResetHealRate();

        public abstract void ResetCriticalHitChance();

        #endregion
    }
}