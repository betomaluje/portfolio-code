using System.Collections.Generic;
using Dungeon.Renderer;
using UnityEngine;

namespace Dungeon {
    [System.Serializable]
    public class Room {
        public int Index;
        public Vector2Int Center;
        public int Width;
        public int Height;

        public bool IsFirstRoom = false;

        public ChunkSettings ChunkSettings { get; set; }

        public Vector2[] Corners = null;

        public IList<Vector3Int> AllPositions => _allPositions;
        public IList<Vector3Int> AllInnerPositions => _allInnerPositions;

        private IList<Vector3Int> _allPositions;
        private IList<Vector3Int> _allInnerPositions;

        public Room(int index, Vector2 center, int width, int height) {
            Index = index;
            Center = Vector2Int.FloorToInt(center);
            Width = width;
            Height = height;
        }

        private IList<Vector3Int> FillAllPositions() {
            _allPositions = new List<Vector3Int>();
            _allInnerPositions = new List<Vector3Int>();
            var center = Center;
            var roomWidth = Width;
            var roomHeight = Height;

            var x = center.x - roomWidth / 2;
            var y = center.y - roomHeight / 2;

            int border = 2;

            for (var i = 0; i < roomWidth; i++) {
                for (var j = 0; j < roomHeight; j++) {
                    var point = new Vector3Int(x + i, y + j);
                    _allPositions.Add(point);

                    // Check if the point lies within the inner area (excluding border cells)
                    bool isInnerColumn = i >= border && i < roomWidth - border;
                    bool isInnerRow = j >= border && j < roomHeight - border;

                    if (isInnerColumn && isInnerRow) {
                        _allInnerPositions.Add(point);
                    }
                }
            }

            return _allPositions;
        }

        public void UpdatePosition(Vector2 newPosition) {
            Center = Vector2Int.FloorToInt(newPosition);
            FillAllPositions();
        }

        public Vector2[] GenerateCorners() {
            Corners = new Vector2[4];
            // bottom left
            Corners[0] = new Vector2(Center.x - Width / 2, Center.y - Height / 2);
            // bottom right
            Corners[1] = new Vector2(Center.x + Width / 2, Center.y - Height / 2);
            // top right
            Corners[2] = new Vector2(Center.x + Width / 2, Center.y + Height / 2);
            // top left
            Corners[3] = new Vector2(Center.x - Width / 2, Center.y + Height / 2);

            return Corners;
        }

        public override string ToString() {
            return $"Room-{Index}";
        }
    }
}