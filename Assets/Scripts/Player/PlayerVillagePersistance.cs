namespace Player {
    public class PlayerVillagePersistance : PlayerPersistence {
        protected override void OnSignedInAsync() {
            LoadPlayer();
        }

        private async void LoadPlayer() {
            _loadedPlayer = await _playerRepo.LoadPlayer();

            OnPlayerLoaded?.Invoke(_loadedPlayer);
        }
    }
}