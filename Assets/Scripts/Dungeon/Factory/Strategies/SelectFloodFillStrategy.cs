using System.Collections.Generic;
using UnityEngine;

namespace Dungeon.Factory.Strategies {
    [CreateAssetMenu(menuName = "Dungeon/Factory/Strategies/SelectFloodFillStrategy")]
    public class SelectFloodFillStrategy : SelectStrategy {
        [SerializeField]
        private int _minMainRooms = 6;

        [SerializeField]
        private float _maxDistance = 10f;

        private List<Room> _rooms;

        override public void Setup(List<Room> rooms) {
            _rooms = rooms;
        }

        public override List<Room> SelectMainRooms(int maxToTake) {
            if (maxToTake <= 0 || _rooms == null || _rooms.Count == 0)
                return new();

            List<Room> selectedRooms = new();
            Queue<Room> roomQueue = new();

            // Step 1: Random starting room
            Room startRoom = _rooms[Random.Range(0, _rooms.Count)];
            roomQueue.Enqueue(startRoom);
            selectedRooms.Add(startRoom);

            // Step 2: Continue flood fill (BFS)
            while (selectedRooms.Count < maxToTake && roomQueue.Count > 0) {
                Room currentRoom = roomQueue.Dequeue();
                // Find neighboring rooms that haven't been selected yet
                List<Room> neighbors = FindNearbyRooms(currentRoom, selectedRooms);

                foreach (Room neighbor in neighbors) {
                    if (!selectedRooms.Contains(neighbor) && selectedRooms.Count < maxToTake) {
                        roomQueue.Enqueue(neighbor);
                        selectedRooms.Add(neighbor);
                    }
                }
            }

            // If we selected less than the min number of rooms, try to add more randomly
            while (selectedRooms.Count < _minMainRooms) {
                Room randomRoom = _rooms[Random.Range(0, _rooms.Count)];
                if (!selectedRooms.Contains(randomRoom)) {
                    selectedRooms.Add(randomRoom);
                }
            }

            return selectedRooms;
        }

        // Helper function to find nearby rooms (within a certain distance)
        private List<Room> FindNearbyRooms(Room currentRoom, List<Room> selectedRooms) {
            List<Room> nearbyRooms = new();

            foreach (Room room in _rooms) {
                if (!selectedRooms.Contains(room)) {
                    float distance = Vector2Int.Distance(currentRoom.Center, room.Center);
                    if (distance <= _maxDistance) {
                        nearbyRooms.Add(room);
                    }
                }
            }

            return nearbyRooms;
        }
    }
}