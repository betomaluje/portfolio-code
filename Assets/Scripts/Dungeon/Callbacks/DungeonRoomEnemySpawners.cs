using System;
using System.Collections.Generic;
using System.Linq;
using Battle;
using Enemies;
using Extensions;
using Sirenix.OdinInspector;
using Sounds;
using UnityEngine;

namespace Dungeon {
    public class DungeonRoomEnemySpawners : MonoBehaviour, IDungeonCallback {
        [Header("Enemy Swarms")]
        [SerializeField]
        private EnemySwarmConfig[] _enemySwarmConfigs;

        [SerializeField]
        private RoomWall _roomWallPrefab;

        [SerializeField]
        [RequiredIn(PrefabKind.InstanceInScene)]
        private Transform _roomContainer;

        [Header("Shops")]
        [SerializeField]
        private bool _includeShops = true;

        [ShowIf("_includeShops")]
        [SerializeField]
        private Transform _weaponShopContainer;

        [ShowIf("_includeShops")]
        [SerializeField]
        private Transform _companionShopContainer;

        public Action OnRoomEnemiesDefeated = delegate { };
        private int _currentRoomsDefeated = 0;
        private int _totalRoomsDefeated = 0;

        private readonly IList<Room> _rooms = new List<Room>();

        private void Start() {
            _totalRoomsDefeated = 0;
            _currentRoomsDefeated = 0;
        }

        private void OnEnable() {
            OnRoomEnemiesDefeated += CheckCurrentRoomDefeated;
        }

        private void OnDisable() {
            OnRoomEnemiesDefeated -= CheckCurrentRoomDefeated;
        }

        private void CheckCurrentRoomDefeated() {
            _currentRoomsDefeated += 1;
            // TODO: maybe check for a percentage of defeated rooms instead of a fixed number
            if (_currentRoomsDefeated >= _totalRoomsDefeated) {
                FindFirstObjectByType<BattleManager>().PlacePortalNearPlayer();
                DungeonWinCounter.Instance.AddWin();
                SoundManager.instance.Play("portal");
            }
        }

        public void OnRoomsToCenter(ref IList<Room> rooms, ref IList<Vector3Int> positions) {
            _rooms.AddRange(rooms);
        }

        public void OnMapStarts() { }

        public void OnAllRoomsGenerated(ref IList<Room> rooms, ref IList<Transform> roomTransforms) { }

        public void OnRoomsSelected(ref IList<Room> selectedRooms) { }

        public void OnMapLoaded() {
            _totalRoomsDefeated = _rooms.Count(r => !r.IsFirstRoom);
            int randomCompanionShop = -1;
            int randomWeaponShop = -1;

            if (_includeShops && _weaponShopContainer != null && _companionShopContainer != null) {
                randomCompanionShop = UnityEngine.Random.Range(0, _rooms.Count);
                randomWeaponShop = randomCompanionShop == 0 ? UnityEngine.Random.Range(randomCompanionShop, _rooms.Count) : UnityEngine.Random.Range(0, randomCompanionShop);
            }

            foreach (var room in _rooms) {
                var roomTransform = _roomContainer.Find(room.ToString());
                if (roomTransform == null) {
                    continue;
                }
                
                if (!room.IsFirstRoom) {

                    // we need to add here the shops
                    if (randomCompanionShop != -1) {
                        //spawn companion shop
                        Instantiate(_companionShopContainer, room.Center.ToInt3(), Quaternion.identity, roomTransform);
                        // we reset the flag
                        randomCompanionShop = -1;
                        _totalRoomsDefeated--;
                    }
                    else if (randomWeaponShop != -1) {
                        Instantiate(_weaponShopContainer, room.Center.ToInt3(), Quaternion.identity, roomTransform);
                        randomWeaponShop = -1;
                        _totalRoomsDefeated--;
                    }
                    else {
                        var roomEnemySwarm = roomTransform.GetOrAdd<RoomEnemySwarm>();
                        var randomSwarmConfig = _enemySwarmConfigs[UnityEngine.Random.Range(0, _enemySwarmConfigs.Length)];
                        roomEnemySwarm.Setup(room, randomSwarmConfig, _roomWallPrefab, OnRoomEnemiesDefeated);
                    }
                }
            }
        }
    }
}