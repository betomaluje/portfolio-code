using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Dungeon {
    public class DungeonRigidBodyRemover : DungeonCallback {
        private Dictionary<int, Transform> _allRooms;

        public override void OnAllRoomsGenerated(ref IList<Room> rooms, ref IList<Transform> roomTransforms) {
            base.OnAllRoomsGenerated(ref rooms, ref roomTransforms);
            _allRooms = new();
            int i = 0;
            foreach (var roomTransform in roomTransforms) {
                DestroyComponent<SpringJoint2D>(roomTransform.gameObject);
                DestroyComponent<Rigidbody2D>(roomTransform.gameObject);
                DestroyComponent<Collider2D>(roomTransform.gameObject);

                _allRooms[i] = roomTransform;

                i++;
            }
        }

        public override void OnRoomsSelected(ref IList<Room> selectedRooms) {
            base.OnRoomsSelected(ref selectedRooms);
            var mySet = new HashSet<int>(selectedRooms.Select(x => x.Index));

            var notSelected = _allRooms.Where(x => !mySet.Contains(x.Key))
                .Select(x => x.Value.parent.gameObject)
                .ToArray();

            CleanRigidBodies(notSelected);
        }

        private void CleanRigidBodies(GameObject[] toDestroy) {
            foreach (var destroy in toDestroy) {
                DestroyComponent<SpringJoint2D>(destroy);
                DestroyComponent<Rigidbody2D>(destroy);
                DestroyComponent<Collider2D>(destroy);
            }
        }

        private void DestroyComponent<T>(GameObject targetGameObject) where T : Component {
            if (targetGameObject.TryGetComponent(out T component)) {
                Object.Destroy(component);
            }
        }
    }
}