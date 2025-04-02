using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dungeon.Factory.Strategies {
    [InlineEditor]
    public abstract class SelectStrategy : ScriptableObject {
        public abstract void Setup(List<Room> rooms);
        public abstract List<Room> SelectMainRooms(int maxToTake);
    }
}