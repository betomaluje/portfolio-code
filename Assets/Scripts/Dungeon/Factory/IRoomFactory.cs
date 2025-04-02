using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon.Factory {
    [InlineEditor]
    public abstract class RoomFactory : ScriptableObject, IRoomFactory {
        public abstract Room[] GenerateRooms(List<Vector2> points);
        public abstract List<Room> SelectMainRooms(int maxToTake);
    }

    interface IRoomFactory {
        Room[] GenerateRooms(List<Vector2> points);

        List<Room> SelectMainRooms(int maxToTake);
    }
}