using System.Collections.Generic;
using System.Linq;
using Extensions;
using Player.Input;
using Scene_Management;
using TMPro;
using Unity.Services.Ugc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Raid {
    public class RaidPlayersUI : MonoBehaviour {
        [SerializeField]
        private RectTransform _container;

        [SerializeField]
        private RectTransform _raidButtonsContainer;

        [SerializeField]
        private GameObject _buttonPrefab;

        [Header("Raids")]
        [SerializeField]
        private PersistentRaid _raid;

        [SerializeField]
        private SceneField _raidScene;

        private PlayerBattleInput _input;

        private void Awake() {
            _input = FindFirstObjectByType<PlayerBattleInput>();
            _raid.ContentId = null;
            _container.gameObject.SetActive(false);
        }

        private void OnEnable() {
            _input.EastButtonEvent += CancelMenu;
        }

        private void OnDisable() {
            _input.EastButtonEvent -= CancelMenu;
        }

        private void CancelMenu() {
            _input.BattleActions.Enable();
            _container.gameObject.SetActive(false);
        }

        public async void OnOtherPlayersLoaded(List<Content> contents) {
            _input.BattleActions.Disable();
            if (!contents.Any()) {
                _input.BattleActions.Enable();
                return;
            }

            await _raidButtonsContainer.DestroyChildrenAsync();
            _container.gameObject.SetActive(true);

            foreach (var content in contents) {
                var buttonObj = Instantiate(_buttonPrefab, _raidButtonsContainer);
                if (buttonObj.TryGetComponent<Button>(out var button)) {
                    var id = content.Id;
                    button.onClick.AddListener(() => OnPlayerClicked(id));

                    var text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                    if (text) {
                        text.text = content.Description;
                    }
                }
            }
            if (_raidButtonsContainer.childCount > 0) {
                // Set the first button as selected (highlighted)
                var first = _raidButtonsContainer.GetChild(0).gameObject;
                EventSystem.current.firstSelectedGameObject = first;
                EventSystem.current.SetSelectedGameObject(first);
            }
        }

        private void OnPlayerClicked(string contentId) {
            _raid.ContentId = contentId;
            SceneLoader.Instance.LoadScene(_raidScene);
        }
    }
}
