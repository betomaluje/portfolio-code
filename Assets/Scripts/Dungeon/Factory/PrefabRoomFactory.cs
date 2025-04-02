using System.Collections.Generic;
using System.Linq;
using Dungeon.Factory.Strategies;
using UnityEngine;

namespace Dungeon.Factory {
    [CreateAssetMenu(menuName = "Dungeon/Factory/PrefabRoomFactory")]
    public class PrefabRoomFactory : RoomFactory {
        [SerializeField]
        private bool _randomlySelect = false;

        [SerializeField]
        private RoomWrapper[] _prefabs;

        [SerializeField]
        private SelectStrategy _selectStrategy;

        private Room[] _selectedRooms;

        public override Room[] GenerateRooms(List<Vector2> points) {
            var maxAmount = points.Count;

            _selectedRooms = _prefabs.Shuffle().Select(p => p.Room).Take(maxAmount).ToArray();

            var index = 0;
            foreach (var room in _selectedRooms) {
                room.Index = index++;
                room.Center = Vector2Int.FloorToInt(points[Random.Range(0, maxAmount)]);
            }

            _selectStrategy.Setup(_selectedRooms.ToList());

            return _selectedRooms;
        }

        public override List<Room> SelectMainRooms(int maxToTake) {
            if (_randomlySelect) {
                return _selectedRooms.Take(maxToTake).ToList();
            }
            else {
                var selectedRooms = _selectStrategy.SelectMainRooms(maxToTake);
                return selectedRooms;
            }
        }


    }
}