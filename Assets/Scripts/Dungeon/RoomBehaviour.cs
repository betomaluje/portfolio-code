using Enemies;
using Extensions;
using UnityEditor;
using UnityEngine;

namespace Dungeon {
    [RequireComponent(typeof(PolygonCollider2D))]
    public class RoomBehaviour : MonoBehaviour {
        public PolygonCollider2D PolygonCollider { get; private set; }

        public Room Room;

        private Vector2[] _corners;

        private bool _isWaveCleared;
        private EnemyWaveController _enemyWaveController;

        private void Awake() => PolygonCollider = GetComponent<PolygonCollider2D>();

        public void UpdateCollider(Room room) {
            Room = room;
            PolygonCollider.isTrigger = true;
            _corners = room.GenerateCorners();
            PolygonCollider.points = _corners;
        }

        public void SetupFirstRoom(FirstRoomSetup firstRoomSetup) {
            var centerOfRoom = Room.Center.ToVector2();
            centerOfRoom.y -= firstRoomSetup.SpacingForPlayer;

            Instantiate(firstRoomSetup.Player, centerOfRoom, Quaternion.identity);

            if (firstRoomSetup.Merchant != null) {
                // we need to instantiate the ModifierMerchant and set all the ranodm available items
                var modifierMerchant = Instantiate(firstRoomSetup.Merchant, Room.Center.ToVector2(), Quaternion.identity, transform);
                modifierMerchant.SetupFirstRoom(firstRoomSetup);
            }
        }

        public void SetRoomForSafeZone(EnemyWaveController enemyWaveController) {
            _isWaveCleared = true;
            _enemyWaveController = enemyWaveController;
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (_isWaveCleared && _enemyWaveController != null && other.CompareTag("Player")) {
                _isWaveCleared = false;
                _enemyWaveController.ResumeWaves();
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            if (_corners == null || _corners.Length != 4) {
                return;
            }

            Gizmos.color = Color.black;
            foreach (var corner in _corners) {
                Gizmos.DrawWireCube(corner, Vector3.one);
            }

            var roomName = new GUIContent($"{Room}");
            var position = Room.Center.ToVector3();
            var style = new GUIStyle(GUI.skin.box) {
                richText = true,
                fontSize = 12,
                normal = {
                    textColor = Color.black
                }
            };

            Handles.Label(position, roomName, style);

            if (Room.ChunkSettings != null) {
                var content = new GUIContent($"{Room.ChunkSettings}");
                Handles.Label(position, content, style);
            }
        }
#endif
    }
}