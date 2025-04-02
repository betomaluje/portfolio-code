using System.Collections.Generic;
using System.Linq;
using Companions;
using Storage.Player;
using Player;
using UnityEngine;

namespace Stats {
    /// <summary>
    /// This class should be responsible for managing player stats such as:
    /// - Health
    /// - Speed
    /// - Attack
    /// - Defense
    /// - Luck (a.k.a. Chance of getting a critical hit)
    /// </summary>
    [RequireComponent(typeof(PlayerPersistence), typeof(PlayerMoneyManager))]
    [RequireComponent(typeof(CompanionsHolder))]
    [RequireComponent(typeof(PlayerLuckController), typeof(PlayerDefenseController))]
    public class PlayerStatsManager : BaseStatsManager {
        [SerializeField]
        private PlayerExperienceController _experienceController;

        [SerializeField]
        private PlayerPersistence _playerPersistence;

        [SerializeField]
        private PlayerMoneyManager _moneyManager;

        [SerializeField]
        private CompanionsHolder _companionsHolder;

        [SerializeField]
        private PlayerLuckController _luckController;

        [SerializeField]
        private PlayerDefenseController _defenseController;

        public PlayerMiniModel.TempStats PlayerStats { get; private set; }

        private Weapons.Weapon[] _allWeapons;

        protected IPlayerStorage _playerRepo;
        private PlayerMiniModel _loadedPlayer;

        private readonly Dictionary<StatType, float> _originalAmounts = new();

        public override event System.Action<StatType, float> OnStatModifierReset = delegate { };

        protected override void Awake() {
            base.Awake();
            _moneyManager = GetComponent<PlayerMoneyManager>();
            _companionsHolder = GetComponent<CompanionsHolder>();
            _experienceController = GetComponent<PlayerExperienceController>();
            _playerPersistence = GetComponent<PlayerPersistence>();
            _luckController = GetComponent<PlayerLuckController>();
            _defenseController = GetComponent<PlayerDefenseController>();

            _playerRepo = new LocalPlayerRepo();
        }

        protected override void OnValidate() {
            base.OnValidate();

            if (_moneyManager == null) {
                _moneyManager = GetComponent<PlayerMoneyManager>();
            }
            if (_companionsHolder == null) {
                _companionsHolder = GetComponent<CompanionsHolder>();
            }
            if (_experienceController == null) {
                _experienceController = GetComponent<PlayerExperienceController>();
            }
            if (_playerPersistence == null) {
                _playerPersistence = GetComponent<PlayerPersistence>();
            }
            if (_luckController == null) {
                _luckController = GetComponent<PlayerLuckController>();
            }
            if (_defenseController == null) {
                _defenseController = GetComponent<PlayerDefenseController>();
            }
        }

        private void OnEnable() {
            _playerPersistence.OnPlayerLoaded += UpdateFromLoaded;
        }

        private void OnDisable() {
            _playerPersistence.OnPlayerLoaded -= UpdateFromLoaded;
        }

        public override void SaveStats() {
            base.SaveStats();
            if (_playerPersistence != null && _loadedPlayer.Equals(default(PlayerMiniModel))) {
                DebugTools.DebugLog.Log("Saving player stats from powerup");
                _playerRepo.SavePlayer(ref _loadedPlayer);
            }
        }

        private void UpdateFromLoaded(PlayerMiniModel loadedPlayer) {
            _loadedPlayer = loadedPlayer;
            PlayerStats = loadedPlayer.Stats;

            SetupHealth(loadedPlayer);

            _moneyManager.SetMoney(loadedPlayer.Money);

            _characterHolder.WeaponManager.OverrideWeapons(LoadWeapons(loadedPlayer.Weapons), loadedPlayer.SelectedWeapon);
            _companionsHolder.LoadCompanions(loadedPlayer.Companions);

            _experienceController.SetupLevel(loadedPlayer.Level);
            _experienceController.SetupExperience(loadedPlayer.Experience);

            _defenseController.SetReduceDamageInfluence(loadedPlayer.Stats.DefenseMultiplier);

            UpdateTempStats(PlayerStats);
        }

        private List<Weapons.Weapon> LoadWeapons(IList<string> weaponNames) {
            if (_allWeapons == null || _allWeapons.Length <= 0) {
                _allWeapons = Resources.LoadAll<Weapons.Weapon>("Weapons");
            }
            // we need to check from allWeapons the ones that match the name from weaponNames
            return _allWeapons.Where(weapon => weaponNames.Contains(weapon.name)).ToList();
        }

        private void UpdateTempStats(PlayerMiniModel.TempStats playerStats) {
            RestoreSpeed(playerStats);
            RestoreAttack(playerStats);
            RestoreDefense(playerStats);
            RestoreHealRate(playerStats);
            RestoreCriticalHitChance(playerStats);
        }

        private float RemapValue(float value) {
            // if the multiplier is for example .2f, we need to add 1 so it becomes 1.2f since we are Multiplying the current value with that
            if (value >= 0 && value < 1f) {
                value += 1f;
            }
            return value;
        }

        private void SetupHealth(PlayerMiniModel loadedPlayer) {
            if (_characterHolder.Health != null) {
                var extraHealth = loadedPlayer.Stats.ExtraHealth;
                _characterHolder.Health.SetupHealth(loadedPlayer.MaxHealth + extraHealth);
            }
        }

        #region Restoring Stats

        private void RestoreSpeed(PlayerMiniModel.TempStats playerStats) {
            var speedMultiplier = RemapValue(playerStats.SpeedMultiplier);
            _originalAmounts[StatType.Speed] = speedMultiplier;
            AddSpeed(speedMultiplier);
        }

        private void RestoreAttack(PlayerMiniModel.TempStats playerStats) {
            var attackMultiplier = RemapValue(playerStats.AttackMultiplier);
            _originalAmounts[StatType.Attack] = attackMultiplier;
            AddAttack(attackMultiplier);
        }

        private void RestoreDefense(PlayerMiniModel.TempStats playerStats) {
            var defenseMultiplier = playerStats.DefenseMultiplier;
            _originalAmounts[StatType.Defense] = defenseMultiplier;
            AddDefense(defenseMultiplier);
        }

        private void RestoreHealRate(PlayerMiniModel.TempStats playerStats) {
            var healRate = RemapValue(playerStats.HealRate);
            _originalAmounts[StatType.HealRate] = healRate;
            AddHealRate(healRate);
        }

        public void RestoreCriticalHitChance(PlayerMiniModel.TempStats playerStats) {
            var amount = playerStats.CriticalHitChance;
            _originalAmounts[StatType.CriticalHitChance] = amount;
            AddCriticalHitChance(amount);
        }

        #endregion

        #region Modifying Stats

        public override void AddDefense(float amount) {
            if (_defenseController == null) {
                return;
            }

            _defenseController.SetReduceDamageInfluence(amount);

            base.AddDefense(amount);
        }

        public override void AddCriticalHitChance(float amount) {
            if (_luckController == null) {
                return;
            }

            _luckController.SetCriticalHitChance(amount);

            base.AddCriticalHitChance(amount);
        }

        #endregion

        #region Reset Stats

        public override void ResetSpeed() {
            if (_characterHolder.Movement == null) {
                return;
            }
            if (_originalAmounts.TryGetValue(StatType.Speed, out var amount)) {
                _characterHolder.Movement.SetMovementInfluence(amount);
                OnStatModifierReset?.Invoke(StatType.Speed, amount);
            }
        }

        public override void ResetAttack() {
            if (_characterHolder.WeaponManager == null) {
                return;
            }
            if (_originalAmounts.TryGetValue(StatType.Attack, out var amount)) {
                _characterHolder.WeaponManager.SetStrengthInfluence(amount);
                OnStatModifierReset?.Invoke(StatType.Attack, amount);
            }
        }

        public override void ResetDefense() {
            if (_defenseController == null) {
                return;
            }

            if (_originalAmounts.TryGetValue(StatType.Defense, out var amount)) {
                _defenseController.SetReduceDamageInfluence(amount);
                OnStatModifierReset?.Invoke(StatType.Defense, amount);
            }
        }

        public override void ResetHealRate() {
            if (_characterHolder.Health == null) {
                return;
            }
            if (_originalAmounts.TryGetValue(StatType.HealRate, out var amount)) {
                _characterHolder?.Health?.GiveHealth(Mathf.FloorToInt(amount));
                OnStatModifierReset?.Invoke(StatType.HealRate, amount);
            }
        }

        public override void ResetCriticalHitChance() {
            if (_luckController == null) {
                return;
            }
            if (_originalAmounts.TryGetValue(StatType.CriticalHitChance, out var amount)) {
                _luckController.SetCriticalHitChance(amount);
                OnStatModifierReset?.Invoke(StatType.CriticalHitChance, amount);
            }
        }

        #endregion
    }
}