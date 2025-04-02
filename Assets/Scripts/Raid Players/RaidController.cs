using System.Collections.Generic;
using Interactable;
using Sounds;
using Unity.Services.Ugc;
using UnityEngine;
using UnityEngine.Events;

namespace Raid {
    public class RaidController : MonoBehaviour, IInteract {
        public UnityEvent<List<Content>> OnPlayersLoaded;

        private CloudPlayersList _playersList;

        private void Awake() {
            _playersList = new CloudPlayersList();
        }

        public void CancelInteraction() {

        }

        public async void DoInteract() {
            SoundManager.instance.Play("build_place");
            var players = await _playersList.GetContentsListAsync();
            OnPlayersLoaded?.Invoke(players);
        }
    }
}