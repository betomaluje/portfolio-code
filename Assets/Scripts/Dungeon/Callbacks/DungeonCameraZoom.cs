using System.Collections.Generic;
using Camera;
using UnityEngine;

namespace Dungeon {
    public class DungeonCameraZoom : DungeonCallback {
        private readonly CinemachineCameraZoom _cinemachineCameraZoom;
        private readonly Transform _transform;

		public DungeonCameraZoom(CinemachineCameraZoom cinemachineCameraZoom, Transform transform) {
            _cinemachineCameraZoom = cinemachineCameraZoom;
            _transform = transform;
		}

		public override void OnMapStarts() {
            _cinemachineCameraZoom.ZoomOut();
        }

        public override void OnRoomsSelected(ref IList<Room> selectedRooms) {
            CinemachineCameraShake.Instance.ShakeCamera(_transform, 12f, 2f, true);
        }

        public override void OnMapLoaded() {
            _cinemachineCameraZoom.ZoomIn();
        }
    }
}