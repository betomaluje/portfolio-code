using System.Collections.Generic;
using System.Linq;
using EditorTool;
using Extensions;
using UnityEngine;
using Utils;

namespace Dungeon {
    public class DungeonPositionsHolder : Singleton<DungeonPositionsHolder> {
        public IList<Vector3Int> AllPositions => _allPositions;

        private readonly IList<Vector3Int> _allPositions = new List<Vector3Int>();

        private HashSet<Vector3Int> _usedPositions = new();

        private DungeonRoomBehaviours _dungeonRoomBehaviours;

        public void Setup(DungeonRoomBehaviours dungeonRoomBehaviours) {
            if (_dungeonRoomBehaviours == null && dungeonRoomBehaviours != null) {
                _dungeonRoomBehaviours = dungeonRoomBehaviours;
            }
        }

        public void ClearPositions() {
            _allPositions.Clear();
            _usedPositions.Clear();
        }

        public void FillAllPositions(IList<Room> rooms) {
            foreach (var room in rooms) {
                _allPositions.AddRange(room.AllPositions);
            }
        }

        public void AddUsedPositions(Vector3Int[] usedPositions) {
            _usedPositions.UnionWith(usedPositions);
        }

        /// <summary>
        /// Get a random amount of valid positions for a dungeon
        /// </summary>
        /// <param name="amount">The amount of points to get</param>
        /// <returns>An array of valid random points inside a whole dungeon</returns>
        public Vector3Int[] GetRandomPoints(int amount, bool includeUsedPositions = false) {
            if (_allPositions != null && _allPositions.Count > 0) {

                if (includeUsedPositions) {
                    var points = _allPositions.SimpleShuffle().Take(amount).ToArray();
                    _usedPositions.UnionWith(points);

                    return points;
                }
                else {
                    var points = _allPositions.Except(_usedPositions).SimpleShuffle().Take(amount).ToArray();
                    _usedPositions.UnionWith(points);

                    return points;
                }
            }
            else {
                return BattleGrid.Instance.GetRandomPoints(amount).ToArray();
            }
        }

        /// <summary>
        /// Get a random amount of positions outside the first room
        /// </summary>
        /// <param name="amount">The amount of points to get</param>
        /// <returns>An array of random points outside the first room</returns>
        public Vector3Int[] GetPointsOutsideFirstRoom(int amount, bool includeUsedPositions = false) {
            if (_allPositions != null && _allPositions.Count > 0) {
                var firstRoom = _dungeonRoomBehaviours.FirstRoom;

                if (includeUsedPositions) {
                    var points = _allPositions.Except(firstRoom.AllInnerPositions).SimpleShuffle().Take(amount).ToArray();
                    _usedPositions.UnionWith(points);
                    return points;
                }
                else {
                    var points = _allPositions.Except(firstRoom.AllInnerPositions).Except(_usedPositions).SimpleShuffle().Take(amount).ToArray();
                    _usedPositions.UnionWith(points);
                    return points;
                }
            }
            else {
                return BattleGrid.Instance.GetRandomPoints(amount).ToArray();
            }
        }

        /// <summary>
        /// Get a random amount of positions inside the first room
        /// </summary>
        /// <param name="amount">The amount of points to get</param>
        /// <returns>An array of random points inside the first room</returns>
        public Vector3Int[] GetPointsInsideFirstRoom(int amount, bool includeUsedPositions = false) {
            var firstRoom = _dungeonRoomBehaviours.FirstRoom;
            return firstRoom.AllPositions.SimpleShuffle().Take(amount).ToArray();
        }

        public RoomBehaviour GetClosestRoomToPoint(Vector3 point) {
            var rooms = _dungeonRoomBehaviours.RoomBehaviours;
            return rooms.Select(room => new { Distance = Vector3.Distance(point, room.Room.Center.ToVector3()), Room = room }).OrderBy(room => room.Distance).First().Room;
        }

        /// <summary>
        /// Get a random position inside a dungeon
        /// </summary>
        /// <returns>A random point inside a dungeon</returns>
        public Vector3Int GetRandomPosition() => _allPositions[Random.Range(0, _allPositions.Count)];
    }
}