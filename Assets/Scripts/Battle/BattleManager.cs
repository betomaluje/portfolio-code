using System.Collections.Generic;
using Cinemachine;
using Dungeon;
using Extensions;
using Player.Input;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Battle {
    public class BattleManager : MonoBehaviour {
        [SerializeField]
        private GameObject _swarmsContainer;

        [Header("Player Config")]
        [SerializeField]
        private CinemachineVirtualCamera _virtualCamera;

        [SerializeField]
        private Portal.Portal _portalInScene;

        [SerializeField]
        private float _distanceFromRoomCenter = 2;

        [Required]
        [Tooltip("List of required objects to instantiate on the scene")]
        [SerializeField]
        private GameObject[] _requiredPrefabs;

        private Transform _playerReference;
        private DungeonMap _dungeonMap;
        private List<Room> _allRooms;

        private void Awake() {
            _dungeonMap = FindFirstObjectByType<DungeonMap>();
        }

        private void Start() {
            _portalInScene.gameObject.SetActive(false);
            var player = FindFirstObjectByType<PlayerBattleInput>();
            _playerReference = player.transform;
            _virtualCamera.Follow = _playerReference;

            _swarmsContainer?.SetActive(true);

            _allRooms = _dungeonMap.SelectedRooms;

            // activate all required objects on scene
            foreach (var obj in _requiredPrefabs) {
                Instantiate(obj, Vector3.zero, Quaternion.identity);
            }
        }

        public void PlacePortalNearPlayer() {
            Vector3 nearPlayer;

            // find the closest room center to the player
            if (_allRooms == null) {
                nearPlayer = _playerReference.position + (Vector3)Random.insideUnitCircle * _distanceFromRoomCenter;
                nearPlayer.z = 0;
            }
            else {
                var closestRoom = _allRooms[0];

                foreach (var room in _allRooms) {
                    if (Vector3.Distance(room.Center.ToVector2(), _playerReference.position) < Vector3.Distance(closestRoom.Center.ToVector2(), _playerReference.position)) {
                        closestRoom = room;
                    }
                }
                // we need to move the portal near the player
                nearPlayer = closestRoom.Center + Random.insideUnitCircle * _distanceFromRoomCenter;
                nearPlayer.z = 0;
            }

            _portalInScene.transform.position = nearPlayer;
            _portalInScene.gameObject.SetActive(true);
        }
    }
}