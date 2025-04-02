using System.Collections.Generic;
using UnityEngine;

namespace Dungeon {
    /// <summary>
    /// Responsible for setting up the first room.
    /// It also sets up each room with it's RoomBehaviour script.
    /// </summary>
    public class DungeonRoomBehaviours : DungeonCallback {
        private readonly FirstRoomSetup _firstRoomSetup;
        private readonly Transform _container;

        public Room FirstRoom { get; private set; }
        public List<RoomBehaviour> RoomBehaviours { get; private set; }

        private RoomBehaviour _firstRoomBehaviour;

        public DungeonRoomBehaviours(Transform container, FirstRoomSetup firstRoomSetup) {
            _container = container;
            _firstRoomSetup = firstRoomSetup;
        }

        public override void OnRoomsToCenter(ref IList<Room> rooms, ref IList<Vector3Int> positions) {
            var startPoint = Random.Range(0, rooms.Count);
            RoomBehaviour firstRoom = null;
            RoomBehaviours = new List<RoomBehaviour>(rooms.Count);

            int i = 0;
            foreach (var room in rooms) {
                var roomObject = new GameObject(room.ToString());
                roomObject.transform.parent = _container;
                var roomBehaviour = roomObject.AddComponent<RoomBehaviour>();
                roomBehaviour.UpdateCollider(room);
                RoomBehaviours.Add(roomBehaviour);

                room.IsFirstRoom = i == startPoint;

                if (i == startPoint) {
                    firstRoom = roomBehaviour;
                }

                i++;
            }

            if (firstRoom != null) {
                _firstRoomBehaviour = firstRoom;
                FirstRoom = firstRoom.Room;
            }
        }

        public override void OnMapLoaded() {
            base.OnMapLoaded();
            if (_firstRoomBehaviour != null) {
                _firstRoomBehaviour.SetupFirstRoom(_firstRoomSetup);
            }
        }
    }
}