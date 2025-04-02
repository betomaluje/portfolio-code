using System.Collections.Generic;
using Sirenix.OdinInspector;
using Storage.Buildings;
using UnityEngine;
using Utils;

namespace Buildings {
    public class BuildingPersistence : Singleton<BuildingPersistence> {
        [SerializeField]
        private Transform _container;

        [SerializeField]
        private BuildingBlueprint _blueprintPrefab;

        [SerializeField]
        private bool _shouldSave = true;

        private Dictionary<Vector2, Building> _allBuildings;
        private IBuildingStorage _buildingStorage;
        public List<Building> AllBuildingScriptableObjects { get; private set; }

        protected override void Awake() {
            base.Awake();
            _buildingStorage = new BuildingRepo();

            AllBuildingScriptableObjects = _buildingStorage.AllBuildingScriptableObjects;
        }

        private void OnEnable() {
            RestoreBuildings();
        }

        private void OnDisable() {
            if (_shouldSave) {
                _buildingStorage.SaveBuildings(_allBuildings);
            }
        }

        private void RestoreBuildings() {
            if (_shouldSave) {
                _allBuildings = _buildingStorage.LoadBuildings();
                foreach (var (pos, storedBuilding) in _allBuildings) {
                    var building = Instantiate(_blueprintPrefab, _container);
                    building.RestoreFromRepo(storedBuilding);
                    building.transform.position = pos;
                }
            }
            else {
                _allBuildings = new Dictionary<Vector2, Building>();
            }
        }

        public void AddBuilding(Transform buildingTransform, Building building) {
            if (_allBuildings.TryAdd(buildingTransform.position, building)) {
                buildingTransform.parent = _container;
            }
        }

        public void RemoveBuilding(Vector2 position) {
            if (_allBuildings.ContainsKey(position)) {
                _allBuildings.Remove(position);
            }
        }

        [Button]
        private void ClearBuildings() {
            new BuildingRepo().SaveBuildings(new Dictionary<Vector2, Building>());
        }
    }
}