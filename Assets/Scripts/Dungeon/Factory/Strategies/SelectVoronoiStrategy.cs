using System.Collections.Generic;
using UnityEngine;

namespace Dungeon.Factory.Strategies {
    [CreateAssetMenu(menuName = "Dungeon/Factory/Strategies/SelectVoronoiStrategy")]
    public class SelectVoronoiStrategy : SelectStrategy {
        [SerializeField]
        private int _minMainRooms = 6;

        private List<Room> _rooms;

        override public void Setup(List<Room> rooms) {
            _rooms = rooms;
        }

        public override List<Room> SelectMainRooms(int maxToTake) {
            if (maxToTake <= 0 || _rooms == null || _rooms.Count == 0)
                return new();

            // Step 1: Generate seed points (these will form the Voronoi regions)
            int numSeeds = Random.Range(_minMainRooms, maxToTake + 1);
            List<Vector2Int> seedPoints = GenerateRandomSeedPoints(numSeeds);

            // Step 2: Create a dictionary to store the rooms belonging to each Voronoi region
            Dictionary<Vector2Int, List<Room>> voronoiRegions = new Dictionary<Vector2Int, List<Room>>();

            foreach (Room room in _rooms) {
                // Find the closest seed point to the room (i.e., Voronoi region)
                Vector2Int closestSeed = FindClosestSeed(room.Center, seedPoints);
                if (!voronoiRegions.ContainsKey(closestSeed)) {
                    voronoiRegions[closestSeed] = new List<Room>();
                }
                voronoiRegions[closestSeed].Add(room);
            }

            // Step 3: Randomly select rooms from the different Voronoi regions
            List<Room> selectedRooms = new();

            foreach (var region in voronoiRegions.Values) {
                if (selectedRooms.Count >= maxToTake)
                    break;

                // Select a random room from each region
                Room randomRoom = region[Random.Range(0, region.Count)];
                selectedRooms.Add(randomRoom);
            }

            // If fewer rooms are selected than _minMainRooms, randomly fill in more rooms
            while (selectedRooms.Count < _minMainRooms) {
                Room randomRoom = _rooms[Random.Range(0, _rooms.Count)];
                if (!selectedRooms.Contains(randomRoom)) {
                    selectedRooms.Add(randomRoom);
                }
            }

            return selectedRooms;
        }

        // Helper function to generate random seed points
        private List<Vector2Int> GenerateRandomSeedPoints(int count) {
            List<Vector2Int> seedPoints = new();
            for (int i = 0; i < count; i++) {
                Vector2Int randomPoint = new(
                    Random.Range(-30, 30), // Adjust the range according to your map size
                    Random.Range(-30, 30)
                );
                seedPoints.Add(randomPoint);
            }
            return seedPoints;
        }

        // Helper function to find the closest seed point to a room
        private Vector2Int FindClosestSeed(Vector2Int roomCenter, List<Vector2Int> seedPoints) {
            Vector2Int closestSeed = seedPoints[0];
            float minDistance = Vector2Int.Distance(roomCenter, closestSeed);

            foreach (Vector2Int seed in seedPoints) {
                float distance = Vector2Int.Distance(roomCenter, seed);
                if (distance < minDistance) {
                    closestSeed = seed;
                    minDistance = distance;
                }
            }

            return closestSeed;
        }

    }
}
