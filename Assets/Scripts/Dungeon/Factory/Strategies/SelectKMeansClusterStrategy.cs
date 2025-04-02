using System.Collections.Generic;
using UnityEngine;

namespace Dungeon.Factory.Strategies {
    /// <summary>
    /// Clusters the rooms into groups and selects one room from each group.
    /// </summary>
    [CreateAssetMenu(menuName = "Dungeon/Factory/Strategies/SelectKMeansClusterStrategy")]
    public class SelectKMeansClusterStrategy : SelectStrategy {
        [SerializeField]
        [Tooltip("The number of main rooms to select")]
        private int k = 3;

        [SerializeField]
        private int _minMainRooms = 6;

        private List<Room> _rooms;

        override public void Setup(List<Room> rooms) {
            _rooms = rooms;
        }

        public override List<Room> SelectMainRooms(int maxToTake) {
            if (maxToTake <= 0 || _rooms == null || _rooms.Count == 0)
                return new();

            // Step 1: Cluster the _rooms using K-Means
            List<List<Room>> clusters = KMeansCluster(_rooms, k);

            List<Room> selectedRooms = new();

            // Step 2: Select one room from each cluster
            foreach (var cluster in clusters) {
                if (cluster.Count > 0 && selectedRooms.Count < maxToTake) {
                    Room randomRoom = cluster[Random.Range(0, cluster.Count)];
                    selectedRooms.Add(randomRoom);
                }
            }

            // Step 3: If less than _minMainRooms are selected, fill the remaining with random rooms
            while (selectedRooms.Count < _minMainRooms) {
                Room randomRoom = _rooms[Random.Range(0, _rooms.Count)];
                if (!selectedRooms.Contains(randomRoom)) {
                    selectedRooms.Add(randomRoom);
                }
            }

            return selectedRooms;
        }

        // Helper function to cluster rooms using K-Means
        private List<List<Room>> KMeansCluster(List<Room> rooms, int k) {
            List<Vector2Int> centroids = new();
            List<List<Room>> clusters = new();

            // Step 1: Initialize centroids with random room centers
            for (int i = 0; i < k; i++) {
                centroids.Add(rooms[Random.Range(0, rooms.Count)].Center);
                clusters.Add(new List<Room>());
            }

            bool centroidsChanged;
            do {
                // Step 2: Assign rooms to the closest centroid
                foreach (Room room in rooms) {
                    int closestCentroidIndex = FindClosestCentroid(room.Center, centroids);
                    clusters[closestCentroidIndex].Add(room);
                }

                // Step 3: Recalculate centroids
                centroidsChanged = false;
                for (int i = 0; i < k; i++) {
                    if (clusters[i].Count > 0) {
                        Vector2Int newCentroid = CalculateCentroid(clusters[i]);
                        if (newCentroid != centroids[i]) {
                            centroidsChanged = true;
                            centroids[i] = newCentroid;
                        }
                    }
                }

                // Clear clusters for the next iteration
                if (centroidsChanged) {
                    clusters.ForEach(cluster => cluster.Clear());
                }

            } while (centroidsChanged);

            return clusters;
        }

        // Helper function to find the closest centroid
        private int FindClosestCentroid(Vector2Int point, List<Vector2Int> centroids) {
            int closestIndex = 0;
            float minDistance = Vector2Int.Distance(point, centroids[0]);

            for (int i = 1; i < centroids.Count; i++) {
                float distance = Vector2Int.Distance(point, centroids[i]);
                if (distance < minDistance) {
                    closestIndex = i;
                    minDistance = distance;
                }
            }

            return closestIndex;
        }

        // Helper function to calculate the centroid of a cluster
        private Vector2Int CalculateCentroid(List<Room> cluster) {
            int sumX = 0;
            int sumY = 0;

            foreach (Room room in cluster) {
                sumX += room.Center.x;
                sumY += room.Center.y;
            }

            return new Vector2Int(sumX / cluster.Count, sumY / cluster.Count);
        }

    }

}