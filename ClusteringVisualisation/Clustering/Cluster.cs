using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace ClusteringVisualisation.Clustering
{
    public interface IClusteringMethod
    {
        void StartClustering(IEnumerable<Point> points);

    }

    public interface ICluster
    {
        Vector2 Coordinates { get; }

        float Distance { get; }

        ICluster LeftCluster { get; }

        ICluster RightCluster { get; }
    }

    public class Cluster : ICluster
    {
        private Vector2? coordinates;

        public Cluster(ICluster left, ICluster right, float distance)
        {
            this.LeftCluster = left;
            this.RightCluster = right;
            this.Distance = distance;
        }

        public ICluster LeftCluster { get; }

        public ICluster RightCluster { get; }

        // TODO: change it to use avg of leaves and not top-level clusters
        public Vector2 Coordinates
        {
            get
            {
                if (coordinates.HasValue)
                {
                    return this.coordinates.Value;
                }
                this.coordinates = (LeftCluster.Coordinates + RightCluster.Coordinates) / 2f;
                return this.coordinates.Value;
            }
        }

        public float Distance { get; }
    }

    public class LeafCluster : ICluster
    {
        private readonly Point point;

        public LeafCluster(Point point)
        {
            this.point = point;
        }

        public int ClusterIndex { get; set; }

        public Vector2 Coordinates => this.point.Coordinates;

        public float Distance => 0f;

        public ICluster LeftCluster => null;

        public ICluster RightCluster => null;
    }

    public class DendrogramBuilder
    {
        public static ICluster ClusterPoints(IEnumerable<Point> points)
        {
            var timer = Stopwatch.StartNew();
            IList<ICluster> clusters = points.Select(p => (ICluster)new LeafCluster(p)).ToList();
            IList<IList<float>> distanceMatrix = BuildDistanceMatrix(clusters);
            while (clusters.Count > 1)
            {
                Cluster(clusters, distanceMatrix);
            }
            Console.WriteLine($"Time {timer.ElapsedMilliseconds / 1000f}s");
            return clusters.Single();
        }

        private static IList<IList<float>> BuildDistanceMatrix(IList<ICluster> clusters)
        {
            IList<IList<float>> rows = new List<IList<float>>(clusters.Count);
            for (int i = 0; i < clusters.Count; i++)
            {
                var row = new List<float>(i);
                rows.Add(row);
                for (int j = 0; j < i; j++)
                {
                    row.Add(Vector2.DistanceSquared(clusters[i].Coordinates, clusters[j].Coordinates));
                }
            }
            return rows;
        }

        private static void Cluster(IList<ICluster> clusters, IList<IList<float>> distanceMatrix)
        {
            FindNearestClustersIndices(distanceMatrix, out int firstIndex, out int secondIndex);
            ICluster firstCluster = clusters[firstIndex];
            ICluster secondCluster = clusters[secondIndex];
            ICluster commonCluster = new Cluster(
                firstCluster, 
                secondCluster, 
                Vector2.Distance(firstCluster.Coordinates, secondCluster.Coordinates));

            clusters[firstIndex] = commonCluster;
            clusters.RemoveAt(secondIndex);
            RemoveClusterFromDistanceMatrix(distanceMatrix, secondIndex);
            RecalculateDistancesForClusterWithIndex(clusters, distanceMatrix, firstIndex);
        }

        private static void FindNearestClustersIndices(IList<IList<float>> distanceMatrix, out int firstIndex, out int secondIndex)
        {
            firstIndex = -1;
            secondIndex = -1;

            float minDistance = float.MaxValue;

            for (int i = 0; i < distanceMatrix.Count; i++)
            {
                var row = distanceMatrix[i];
                for (int j = 0; j < row.Count; j++)
                {
                    float distanceBetweenClusters = row[j];
                    if (distanceBetweenClusters < minDistance)
                    {
                        firstIndex = j;
                        secondIndex = i;
                        minDistance = distanceBetweenClusters;
                    }
                }
            }
        }

        private static void RemoveClusterFromDistanceMatrix(IList<IList<float>> distanceMatrix, int clusterIndex)
        {
            for(int i = clusterIndex + 1; i < distanceMatrix.Count; i++)
            {
                distanceMatrix[i].RemoveAt(clusterIndex);
            }
            distanceMatrix.RemoveAt(clusterIndex);
        }

        private static void RecalculateDistancesForClusterWithIndex(
            IList<ICluster> clusters, 
            IList<IList<float>> distanceMatrix, 
            int clusterIndex)
        {
            var clusterRow = distanceMatrix[clusterIndex];
            Vector2 clusterCoord = clusters[clusterIndex].Coordinates;
            for(int i = 0; i < clusterRow.Count; i++)
            {
                clusterRow[i] = Vector2.DistanceSquared(clusterCoord, clusters[i].Coordinates);
            }

            for(int i = clusterIndex + 1; i < distanceMatrix.Count; i++)
            {
                var row = distanceMatrix[i];
                row[clusterIndex] = Vector2.DistanceSquared(clusterCoord, clusters[i].Coordinates); 
            }
        }
    }
}
