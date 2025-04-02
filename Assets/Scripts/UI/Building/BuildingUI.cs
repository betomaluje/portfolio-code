using System.Collections.Generic;
using Buildings;
using Player;
using Player.Input;
using Sounds;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Building {
    public class BuildingUI : MonoBehaviour {
        [SerializeField]
        private PlayerUIInput _playerInput;

        [SerializeField]
        private PlayerMoneyManager _playerMoneyManager;

        [SerializeField]
        private GameObject _buildingMenu;

        [SerializeField]
        private Transform _container;

        [SerializeField]
        private BuildingButton _buttonPrefab;

        [Header("Preview")]
        [SerializeField]
        private TextMeshProUGUI _buildingName;

        [SerializeField]
        private TextMeshProUGUI _buildingDescription;

        [SerializeField]
        private TextMeshProUGUI _buildingCost;

        [SerializeField]
        private TextMeshProUGUI _purchaseStatus;

        [SerializeField]
        private Image _previewImage;

        [Header("Placing")]
        [SerializeField]
        private GhostBuilding _ghostPrefab;

        private List<Buildings.Building> _allBuildings;
        private int _lastSelected;

        private PlayerBattleInput _input;

        private void Awake() {
            _input = FindFirstObjectByType<PlayerBattleInput>();
        }

        private void Start() {
            _buildingMenu.SetActive(false);

            // TODO: maybe do some logic with saved settings or something
            _lastSelected = 0;

            PopulateBuildings();
        }

        private void OnEnable() {
            _input.WestButtonEvent += ToggleBuildingMenu;
            _playerInput.CancelEvent += CancelBuildingMenu;

            if (_allBuildings != null && _allBuildings.Count > 0) {
                BuildingPreview(_allBuildings[_lastSelected]);
            }
        }

        private void OnDisable() {
            _input.WestButtonEvent -= ToggleBuildingMenu;
            _playerInput.CancelEvent -= CancelBuildingMenu;
        }

        private void CancelBuildingMenu() {
            _buildingMenu.gameObject.SetActive(false);
            Time.timeScale = 1;
            _input.BattleActions.Enable();
        }

        private void ToggleBuildingMenu() {
            var alreadyThere = _buildingMenu.gameObject.activeInHierarchy;
            _buildingMenu.gameObject.SetActive(!alreadyThere);

            if (_buildingMenu.gameObject.activeInHierarchy) {
                SoundManager.instance.Play("menu_open");
                var firstSelected = _container.GetChild(_lastSelected).gameObject;
                EventSystem.current.firstSelectedGameObject = firstSelected;
                EventSystem.current.SetSelectedGameObject(firstSelected);
                Time.timeScale = 0;
                _input.BattleActions.Disable();
            }
            else {
                Time.timeScale = 1;
                _input.BattleActions.Enable();
            }
        }

        private void PopulateBuildings() {
            _allBuildings = BuildingPersistence.Instance.AllBuildingScriptableObjects;

            for (var i = 0; i < _allBuildings.Count; i++) {
                var building = _allBuildings[i];
                var button = Instantiate(_buttonPrefab, _container);
                button.Setup(building, () => BuildingPreview(building));
                if (button.TryGetComponent(out Button b)) {
                    b.onClick.AddListener(delegate { OnBuildingClick(building); });
                }
            }

            BuildingPreview(_allBuildings[_lastSelected]);
        }

        private void BuildingPreview(Buildings.Building building) {
            _lastSelected = _allBuildings.IndexOf(building);

            _previewImage.sprite = building.Sprite;
            _previewImage.preserveAspect = true;

            _buildingName.text = building.Name;
            _buildingDescription.text = building.Description;
            _buildingCost.text = $"${building.Cost}";

            _purchaseStatus.gameObject.SetActive(_playerMoneyManager.CurrentMoney < building.Cost);
        }

        private void OnBuildingClick(Buildings.Building building) {
            if (building.Cost > _playerMoneyManager.CurrentMoney) {
                return;
            }

            // hide menu
            var alreadyThere = _buildingMenu.gameObject.activeInHierarchy;
            _buildingMenu.gameObject.SetActive(!alreadyThere);
            Time.timeScale = 1;

            // instantiate ghost prefab with building sprite
            var ghost = Instantiate(_ghostPrefab, _playerMoneyManager.transform.position, Quaternion.identity);
            ghost.Setup(building);
        }
    }
}