using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Dungeon {
    [RequireComponent(typeof(CinematicBlackLines))]
    public class CinematicDungeon : MonoBehaviour, IDungeonCallback {
        private CinematicBlackLines _cinematicBlackLines;

        private void Awake() {
            _cinematicBlackLines = GetComponent<CinematicBlackLines>();
        }

        public void OnMapStarts() {
            _cinematicBlackLines.DoAppear();
        }

        public void OnAllRoomsGenerated(ref IList<Room> rooms, ref IList<Transform> roomTransforms) { }

        public void OnRoomsSelected(ref IList<Room> selectedRooms) { }

        public void OnRoomsToCenter(ref IList<Room> rooms, ref IList<Vector3Int> positions) { }

        public void OnMapLoaded() {
            _cinematicBlackLines.DoDisappear();
        }
    }
}