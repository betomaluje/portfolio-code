using System.Collections.Generic;
using Buildings;
using UnityEngine;

namespace Storage.Buildings {
    public interface IBuildingStorage {
        public List<Building> AllBuildingScriptableObjects { get; set; }

        void SaveBuildings(Dictionary<Vector2, Building> allBuildings);

        Dictionary<Vector2, Building> LoadBuildings();
    }
}