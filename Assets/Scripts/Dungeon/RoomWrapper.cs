using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Dungeon {
    public class RoomWrapper : MonoBehaviour {
        [SerializeField]
        private Room _room;

        public Room Room => _room;

        private void OnValidate() {
            if (_room != null) {
                var localScale = transform.localScale;
                localScale.x = _room.Width;
                localScale.y = _room.Height;
                transform.localScale = localScale;
            }
        }
#if UNITY_EDITOR
        [Button]
        private void CreatePrefab() {
            CreatePrefab(_room);
        }

        private void CreatePrefab(Room room) {
            string fileName = $"Room_{room.Width}x{room.Height}";
            string fileLocation = $"Assets/Prefabs/Rooms/{fileName}.prefab";
            var clone = gameObject;
            clone.name = fileName;
            OnValidate();
            var emptyObj = PrefabUtility.SaveAsPrefabAsset(clone, fileLocation);
        }

        [Button]
        private void CreateAll() {
            for (int i = 10; i <= 15; i++) {
                for (int j = 5; j <= 15; j++) {
                    var room = new Room(0, Vector2.zero, i, j);
                    _room = room;
                    CreatePrefab(_room);
                }
            }
        }
#endif
    }
}