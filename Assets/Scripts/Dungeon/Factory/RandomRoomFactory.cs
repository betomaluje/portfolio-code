using System.Collections.Generic;
using System.Linq;
using Dungeon.Factory.Strategies;
using UnityEngine;

namespace Dungeon.Factory {
    [CreateAssetMenu(menuName = "Dungeon/Factory/RandomRoomFactory")]
    public class RandomRoomFactory : RoomFactory {
        [SerializeField]
        private Vector2Int _roomSize = new(5, 15);

        private Room[] _allRooms;

        [SerializeField]
        private SelectStrategy _selectStrategy;

        public override Room[] GenerateRooms(List<Vector2> points) {
            _allRooms = new Room[points.Count];

            // we create the first rooms
            int index = 0;
            foreach (var point in points) {
                var roomWidth = Random.Range(_roomSize.x, _roomSize.y);
                var roomHeight = Random.Range(_roomSize.x, _roomSize.y);

                _allRooms[index] = new Room(index, point, roomWidth, roomHeight);

                index++;
            }

            _selectStrategy.Setup(_allRooms.ToList());

            return _allRooms;
        }

        public override List<Room> SelectMainRooms(int maxToTake) {
            var selectedRooms = _selectStrategy.SelectMainRooms(maxToTake);
            return selectedRooms;
        }
    }
}