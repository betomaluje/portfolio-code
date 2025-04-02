using System.Collections.Generic;
using UnityEngine;

namespace Dungeon {
    public abstract class DungeonCallback : IDungeonCallback {
        public virtual void OnMapStarts() { }

        public virtual void OnAllRoomsGenerated(ref IList<Room> rooms, ref IList<Transform> roomTransforms) { }

        public virtual void OnRoomsSelected(ref IList<Room> selectedRooms) { }

        public virtual void OnRoomsToCenter(ref IList<Room> rooms, ref IList<Vector3Int> positions) { }

        public virtual void OnMapLoaded() { }
    }

    public interface IDungeonCallback {
        void OnMapStarts();

        void OnAllRoomsGenerated(ref IList<Room> rooms, ref IList<Transform> roomTransforms);

        void OnRoomsSelected(ref IList<Room> selectedRooms);

        void OnRoomsToCenter(ref IList<Room> rooms, ref IList<Vector3Int> positions);

        void OnMapLoaded();
    }
}