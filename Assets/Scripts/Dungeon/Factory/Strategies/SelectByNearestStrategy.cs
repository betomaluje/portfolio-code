using System.Collections.Generic;
using UnityEngine;

namespace Dungeon.Factory.Strategies {
    [CreateAssetMenu(menuName = "Dungeon/Factory/Strategies/SelectByNearestStrategy")]
    public class SelectByNearestStrategy : SelectStrategy {
        [SerializeField]
        private float _maxDistance = 10f;

        private List<Room> _rooms;

        override public void Setup(List<Room> rooms) {
            _rooms = rooms;
        }

        public override List<Room> SelectMainRooms(int maxToTake) {
            if (maxToTake <= 0 || _rooms == null || _rooms.Count == 0)
                return new();
                
            return FromNearbyStrategy(_rooms, maxToTake);
        }

        private List<Room> FromNearbyStrategy(List<Room> allRooms, int maxToTake) {
            List<Room> selectedRooms = new();
            while (selectedRooms.Count < maxToTake) {
                // Choose a random starting room from the weighted list
                Room startRoom = allRooms[Random.Range(0, allRooms.Count)];

                // if we already have the starting room, skip it
                if (selectedRooms.Contains(startRoom)) {
                    continue;
                }

                // Find neighboring rooms that are close to the selected room
                List<Room> nearbyRooms = FindNearbyRooms(startRoom, allRooms, selectedRooms);

                // Add random nearby rooms to the selection
                foreach (Room room in nearbyRooms) {
                    if (!selectedRooms.Contains(room) && selectedRooms.Count < maxToTake) {
                        selectedRooms.Add(room);
                    }
                }

                // Break the loop if we've already selected enough rooms
                if (selectedRooms.Count >= maxToTake)
                    break;
            }

            return selectedRooms;
        }

        // Function to find nearby rooms based on proximity to the start room
        private List<Room> FindNearbyRooms(Room startRoom, List<Room> rooms, List<Room> alreadySelected) {
            List<Room> nearbyRooms = new();

            foreach (Room room in rooms) {
                if (!alreadySelected.Contains(room)) {
                    float distance = Vector2Int.Distance(startRoom.Center, room.Center);
                    if (distance <= _maxDistance) {
                        nearbyRooms.Add(room);
                    }
                }
            }

            return nearbyRooms;
        }
    }

}