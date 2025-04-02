using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dungeon.Factory.Strategies {
    /// <summary>
    /// Optimizes room selection by trying to minimize a cost function (area vs. distance).
    /// </summary>
    [CreateAssetMenu(menuName = "Dungeon/Factory/Strategies/SelectSimulatedAnnealingStrategy")]
    public class SelectSimulatedAnnealingStrategy : SelectStrategy {
        [SerializeField]
        private int _minMainRooms = 6;

        private List<Room> _rooms;

        override public void Setup(List<Room> rooms) {
            _rooms = rooms;
        }

        public override List<Room> SelectMainRooms(int maxToTake) {
            if (maxToTake <= 0 || _rooms == null || _rooms.Count == 0)
                return new();


            // Step 1: Initialize with a random selection of rooms
            List<Room> currentRooms = GetRandomRooms(Mathf.Min(maxToTake, _minMainRooms));
            List<Room> bestRooms = new(currentRooms);
            float bestScore = CalculateScore(bestRooms);

            float temperature = 100f;      // Starting temperature
            float coolingRate = 0.99f;     // Cooling rate

            // Step 2: Simulated Annealing loop
            while (temperature > 1f) {
                // Create a new mutated version of the current rooms
                List<Room> newRooms = MutateRooms(currentRooms);
                float newScore = CalculateScore(newRooms);

                // Accept the new rooms if they are better, or probabilistically if not
                if (newScore > bestScore || Random.Range(0f, 1f) < Mathf.Exp((newScore - bestScore) / temperature)) {
                    currentRooms = new List<Room>(newRooms);
                    bestScore = newScore;
                }

                temperature *= coolingRate;
            }

            return bestRooms;
        }

        // Helper function: Get a random set of rooms
        private List<Room> GetRandomRooms(int count) => _rooms.SimpleShuffle().Take(count);

        // Helper function to randomly mutate the room selection
        private List<Room> MutateRooms(List<Room> rooms) {
            List<Room> mutatedRooms = new(rooms);

            // Randomly swap a room with a different room
            int randomIndex = Random.Range(0, rooms.Count);
            Room newRoom = rooms[Random.Range(0, rooms.Count)];

            mutatedRooms[randomIndex] = newRoom;

            return mutatedRooms;
        }

        // Helper function to calculate the score (optimize based on distance and size)
        private float CalculateScore(List<Room> rooms) {
            float totalArea = rooms.Sum(r => r.Width * r.Height);
            float totalDistance = 0f;

            for (int i = 0; i < rooms.Count; i++) {
                for (int j = i + 1; j < rooms.Count; j++) {
                    totalDistance += Vector2Int.Distance(rooms[i].Center, rooms[j].Center);
                }
            }

            // Higher score is better: total area minus total distance
            return totalArea - totalDistance;
        }

    }
}