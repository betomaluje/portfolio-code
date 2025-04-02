using System;
using Companions;
using Sirenix.OdinInspector;
using Storage.Player;
using UnityEngine;
using Weapons;

namespace Player {
    [RequireComponent(typeof(PlayerMoneyManager), typeof(PlayerExperienceController))]
    [RequireComponent(typeof(WeaponManager), typeof(CompanionsHolder))]
    [DisallowMultipleComponent]
    public abstract class PlayerPersistence : MonoBehaviour {
        public Action<PlayerMiniModel> OnPlayerLoaded = delegate { };

        protected PlayerMoneyManager _moneyManager;
        protected WeaponManager _weaponManager;
        protected CompanionsHolder _companionsHolder;
        protected PlayerExperienceController _experienceController;

        protected IPlayerStorage _playerRepo;
        // private PlayerCloudServices _cloudServices;

        protected PlayerMiniModel _loadedPlayer;

        private void Awake() {
            _moneyManager = GetComponent<PlayerMoneyManager>();
            _experienceController = GetComponent<PlayerExperienceController>();
            _weaponManager = GetComponent<WeaponManager>();
            _companionsHolder = GetComponent<CompanionsHolder>();
            _playerRepo = new LocalPlayerRepo();
        }

        private void Start() {
            _weaponManager.OnWeaponChanged += UpdatePlayersWeaponIndex;
            PlayerMoneyManager.OnMoneyChange += UpdatePlayersMoney;
            _experienceController.OnExperienceGained += UpdatePlayerExperience;
            _experienceController.OnLevelUp += UpdatePlayerLevel;

            OnSignedInAsync();
        }

        private void OnDestroy() {
            _weaponManager.OnWeaponChanged -= UpdatePlayersWeaponIndex;
            PlayerMoneyManager.OnMoneyChange -= UpdatePlayersMoney;
            _experienceController.OnExperienceGained -= UpdatePlayerExperience;
            _experienceController.OnLevelUp -= UpdatePlayerLevel;
        }

        private void UpdatePlayersMoney(int currentMoney) {
            _loadedPlayer.Money = currentMoney;
            _playerRepo.SavePlayer(ref _loadedPlayer);
        }

        private void UpdatePlayersWeaponIndex(int newWeaponIndex) {
            _loadedPlayer.SelectedWeapon = newWeaponIndex;
            _playerRepo.SavePlayer(ref _loadedPlayer);
        }

        private void UpdatePlayerExperience(int newExperience) {
            _loadedPlayer.Experience = newExperience;
            _playerRepo.SavePlayer(ref _loadedPlayer);
        }

        private void UpdatePlayerLevel(int newLevel) {
            _loadedPlayer.Level = newLevel;
            _playerRepo.SavePlayer(ref _loadedPlayer);
        }

        protected abstract void OnSignedInAsync();

        public void SaveBoughtItems() {
            _loadedPlayer.Weapons = _weaponManager.WeaponNames;
            _loadedPlayer.Companions = _companionsHolder.EquippedCompanions;
            _playerRepo.SavePlayer(ref _loadedPlayer);
        }

        #region PlayerCloudServices
        // DON'T FORGET TO ADD PlayerCloudServices again to player in the Editor
        // private void DecideRepo() {
        // if (await InternetConnection.HasInternet()) {
        //     _cloudServices = GetComponent<PlayerCloudServices>();
        //     _playerRepo = new CloudPlayerRepo();
        // }
        // else {
        //     // we bypass it
        //     _playerRepo = new LocalPlayerRepo();
        //     OnSignedInAsync();
        // }
        // }

        // protected virtual void OnEnable() {
        //     if (_cloudServices != null) {
        //         _cloudServices.OnSignedIn += OnSignedInAsync;
        //     }
        // }

        // protected virtual void OnDisable() {
        //     if (_cloudServices != null) {
        //         _cloudServices.OnSignedIn -= OnSignedInAsync;
        //     }
        // }
        #endregion

        #region Test Editor callbacks

        // [PropertySpace]
        // [Button("Reset Health")]
        // private async void ResetHealth() {
        //     var playerRepo = new CloudPlayerRepo();
        //     var model = PlayerMiniModel.GetDefault();
        //     var storedPlayer = await playerRepo.LoadPlayer();

        //     playerRepo.SavePlayer(storedPlayer.Money, model.Health, model.Weapons);
        // }

        // [Button("Reset Money")]
        // private async void ResetMoney() {
        //     var playerRepo = new CloudPlayerRepo();
        //     var model = PlayerMiniModel.GetDefault();
        //     var storedPlayer = await playerRepo.LoadPlayer();

        //     playerRepo.SavePlayer(model.Money, storedPlayer.Health, model.Weapons);
        // }

        [PropertySpace]
        [Button("Reset Local")]
        private void ResetLocalStorage() {
            var playerRepo = new LocalPlayerRepo();
            var model = PlayerMiniModelExt.GetDefault();
            playerRepo.SavePlayer(ref model);
        }

        [Button]
        private async void GiveAndSaveMoney(int amount) {
            var playerRepo = new LocalPlayerRepo();
            var model = await playerRepo.LoadPlayer();
            var total = model.Money + amount;
            model.Money = total;
            playerRepo.SavePlayer(ref model);
        }

        // [Button("Reset Cloud")]
        // private void ResetCloudStorage() {
        //     var playerRepo = new CloudPlayerRepo();
        //     var model = PlayerMiniModel.GetDefault();
        //     playerRepo.SavePlayer(model.Money, model.Health, model.Weapons);
        // }

        #endregion
    }
}