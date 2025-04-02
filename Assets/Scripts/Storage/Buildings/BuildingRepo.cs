using System.Collections.Generic;
using System.Linq;
using Buildings;
using Newtonsoft.Json;
using UnityEngine;

namespace Storage.Buildings {
    public class BuildingRepo : IBuildingStorage {
        private const string BUILDINGS_KEY = "buildings_key";

        private readonly HashSet<Building> _allBuildingScriptableObjects;

        public BuildingRepo() {
            _allBuildingScriptableObjects = new HashSet<Building>(AllBuildingScriptableObjects);
        }

        public List<Building> AllBuildingScriptableObjects {
            get => Resources.LoadAll<Building>("Buildings").ToList();
            set { }
        }

        /// <summary>
        ///     We save the list by BuildingMiniModel (basically just position and name) so it's lightweight
        /// </summary>
        /// <param name="allBuildings"></param>
        public void SaveBuildings(Dictionary<Vector2, Building> allBuildings) {
            if (allBuildings is not { Count: > 0 }) {
                PlayerPrefs.SetString(BUILDINGS_KEY, "");
                return;
            }

            var list = new List<BuildingMiniModel>();
            foreach (var (pos, building) in allBuildings) {
                var mini = new BuildingMiniModel {
                    Name = building.Name,
                    X = pos.x,
                    Y = pos.y
                };

                list.Add(mini);
            }

            var toJson = JsonConvert.SerializeObject(list);

            PlayerPrefs.SetString(BUILDINGS_KEY, toJson);
        }

        public Dictionary<Vector2, Building> LoadBuildings() {
            var jsonString = PlayerPrefs.GetString(BUILDINGS_KEY, null);

            if (string.IsNullOrEmpty(jsonString)) {
                return new Dictionary<Vector2, Building>();
            }

            var fromJson = JsonConvert.DeserializeObject<List<BuildingMiniModel>>(jsonString);
            var dict = new Dictionary<Vector2, Building>();
            foreach (var mini in fromJson) {
                var pos = new Vector2(mini.X, mini.Y);
                var building = FromName(mini.Name);
                dict[pos] = building;
            }

            return dict;
        }

        private Building FromName(string name) {
            return _allBuildingScriptableObjects.FirstOrDefault(building => building.Name.Equals(name));
        }
    }
}