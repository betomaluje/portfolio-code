using UnityEngine;
using Random = System.Random;

namespace Dungeon {
    /// <summary>
    /// This class will generate a dungeon composed by rooms from different sizes.
    /// The rooms will be placed at random positions on the grid
    /// </summary>
    public class DungeonGenerator {
        private Random _random;
        private int _tileSize = 4;

        public DungeonGenerator(int tileSize = 4) {
            _tileSize = tileSize;
            _random = new Random();
        }

        public DungeonGenerator(int tileSize = 4, int seed = 666) {
            _tileSize = tileSize;
            _random = new Random(seed);
        }

        private int RoundToNearestGridMultiple(float value, int gridSize) {
            // Rounds a value to the nearest multiple of the grid size
            return (int)(Mathf.Floor((value + gridSize - 1) / gridSize) * gridSize);
        }

        public Vector2 GetRandomPointInCircle(int radius) {
            float t = (float)(2f * Mathf.PI * _random.NextDouble());
            double u = _random.NextDouble() + _random.NextDouble();
            float r = (float)(u > 1 ? 2 - u : u);

            return new Vector2(RoundToNearestGridMultiple(radius * r * Mathf.Cos(t), _tileSize), RoundToNearestGridMultiple(radius * r * Mathf.Sin(t), _tileSize));
        }

        public Vector2 GetRandomPointInEllipse(int width, int height) {
            float t = (float)(2f * Mathf.PI * _random.NextDouble());
            double u = _random.NextDouble() + _random.NextDouble();
            float r = (float)(u > 1 ? 2 - u : u);

            return new Vector2(RoundToNearestGridMultiple(width * r * Mathf.Cos(t) / 2, _tileSize), RoundToNearestGridMultiple(height * r * Mathf.Sin(t) / 2, _tileSize));
        }

        public Vector2 GetRandomPointInSquare(int size) {
            return GetRandomPointInRectangle(size, size);
        }

        public Vector2 GetRandomPointInRectangle(int width, int height) {
            // Generate random x and y in the ranges [-width/2, width/2] and [-height/2, height/2]
            float x = (float)(_random.NextDouble() * width) - width / 2f;
            float y = (float)(_random.NextDouble() * height) - height / 2f;

            // Optionally round to nearest multiple of _tileSize
            return new Vector2(RoundToNearestGridMultiple(x, _tileSize), RoundToNearestGridMultiple(y, _tileSize));
        }


        public Vector2 GetRandomPointOnLine(Vector2 pointA, Vector2 pointB) {
            float t = (float)_random.NextDouble();  // A value between 0 and 1
            float x = Mathf.Lerp(pointA.x, pointB.x, t);
            float y = Mathf.Lerp(pointA.y, pointB.y, t);

            return new Vector2(RoundToNearestGridMultiple(x, _tileSize), RoundToNearestGridMultiple(y, _tileSize));
        }

        public Vector2 GetRandomPointInHexagon(float radius) {
            // Generate a random angle and a random distance from the center
            float angle = (float)(_random.NextDouble() * 2 * Mathf.PI); // Random angle from 0 to 2PI
            float distance = radius * Mathf.Pow((float)_random.NextDouble(), 1f / 3f); // Scales the distance correctly in hexagonal space

            // Hexagonal coordinates
            float x = distance * Mathf.Cos(angle);
            float y = distance * Mathf.Sin(angle);

            // Convert to the axial coordinate system for a hexagon
            float absX = Mathf.Abs(x);
            float absY = Mathf.Abs(y);

            if (absX > radius * Mathf.Sqrt(3) / 2 || absY > radius * 3f / 2) {
                // If the point is outside the hexagon, normalize it
                float maxComponent = Mathf.Max(absX, absY);
                x = x * radius / maxComponent;
                y = y * radius / maxComponent;
            }

            return new Vector2(RoundToNearestGridMultiple(x, _tileSize), RoundToNearestGridMultiple(y, _tileSize));
        }


        public Vector2 GetRandomPointInSpiral(float maxRadius, float spiralTurns) {
            float t = (float)(_random.NextDouble() * spiralTurns * 2 * Mathf.PI);
            float r = (float)(_random.NextDouble() * maxRadius);

            float x = r * Mathf.Cos(t);
            float y = r * Mathf.Sin(t);

            return new Vector2(RoundToNearestGridMultiple(x, _tileSize), RoundToNearestGridMultiple(y, _tileSize));
        }

        public Vector2 GetRandomPointInRing(float innerRadius, float outerRadius) {
            float angle = (float)(2 * Mathf.PI * _random.NextDouble());
            float radius = Mathf.Sqrt((float)_random.NextDouble() * (outerRadius * outerRadius - innerRadius * innerRadius) + innerRadius * innerRadius);

            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);

            return new Vector2(RoundToNearestGridMultiple(x, _tileSize), RoundToNearestGridMultiple(y, _tileSize));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="scale">Uniformly scales the entire shape, allowing you to shrink or expand it.</param>
        /// <param name="aspectRatio">Lets you stretch the shape along the x-axis. If you want to control the y-axis instead, 
        /// you can divide by this parameter when calculating y. For example, you could do r * Mathf.Sin(theta) / aspectRatio to adjust the height.</param>
        /// <returns></returns>
        public Vector2 GetRandomPointInLemniscate(float a, float scale = 1f, float aspectRatio = 1f) {
            // Generate a random angle between 0 and 2π
            float theta = (float)(_random.NextDouble() * 2 * Mathf.PI);

            // Use the polar equation for the lemniscate of Bernoulli: r^2 = a^2 * cos(2θ)
            // The range of cos(2θ) ensures both loops (positive and negative values for r)
            float rSquared = a * a * Mathf.Cos(2 * theta);

            // Handle negative values of r correctly for both loops
            float r = Mathf.Sign(rSquared) * Mathf.Sqrt(Mathf.Abs(rSquared));

            // Convert from polar to Cartesian coordinates
            float x = scale * r * Mathf.Cos(theta) * aspectRatio;
            float y = scale * r * Mathf.Sin(theta);

            // Return the result, optionally rounding to nearest tile
            return new Vector2(RoundToNearestGridMultiple(x, _tileSize), RoundToNearestGridMultiple(y, _tileSize));
        }

        public Vector2 GetRandomPointInCross(float length) {
            float randomValue = (float)(_random.NextDouble() * 2 - 1);  // Random value between -1 and 1
            if (_random.NextDouble() > 0.5) {
                return new Vector2(RoundToNearestGridMultiple(randomValue * length, _tileSize), RoundToNearestGridMultiple(randomValue * length, _tileSize));  // One diagonal
            }
            else {
                return new Vector2(RoundToNearestGridMultiple(-randomValue * length, _tileSize), RoundToNearestGridMultiple(randomValue * length, _tileSize));  // Other diagonal
            }
        }

        public Vector2 GetRandomPointInGrid(int rows, int cols, float cellSize) {
            int randomRow = _random.Next(rows);
            int randomCol = _random.Next(cols);

            float xOffset = (float)(_random.NextDouble() - 0.5) * cellSize;
            float yOffset = (float)(_random.NextDouble() - 0.5) * cellSize;

            float x = randomCol * cellSize + xOffset;
            float y = randomRow * cellSize + yOffset;

            return new Vector2(RoundToNearestGridMultiple(x, _tileSize), RoundToNearestGridMultiple(y, _tileSize));
        }


    }
}