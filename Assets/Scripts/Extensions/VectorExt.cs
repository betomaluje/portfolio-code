using UnityEngine;

namespace Extensions {
    public static class VectorExt {
        public static Vector2 GetRandomPosition(this Vector2 originPoint, float radius) {
            var randomPoint = Random.insideUnitCircle * radius;
            return originPoint + randomPoint;
        }

        public static Vector2 GetRandomPosition(this Vector3 originPoint, float radius) {
            var randomPoint = Random.insideUnitCircle * radius;
            return (Vector2)originPoint + randomPoint;
        }

        public static Vector3Int ToInt(this Vector3 vector) => Vector3Int.RoundToInt(vector);

        public static Vector3Int ToInt(this Vector2 vector) => Vector3Int.RoundToInt(vector);

        public static Vector3Int ToInt3(this Vector2Int vector) => new(vector.x, vector.y, 0);

        public static Vector3 ToVector3(this Vector3Int vector) => new(vector.x, vector.y, vector.z);

        public static Vector3 ToVector3(this Vector2Int vector) => new(vector.x, vector.y, 0);

        public static Vector2 ToVector2(this Vector3Int vector) => new(vector.x, vector.y);
        public static Vector2 ToVector2(this Vector2Int vector) => new(vector.x, vector.y);
    }
}