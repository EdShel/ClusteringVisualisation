using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace ClusteringVisualisation.Clustering
{
    public interface IClusteringMethod
    {
        void StartClustering(IEnumerable<Vector2> points);
    }

    public interface ICluster
    {
        float Distance { get; }

        ICluster LeftCluster { get; }

        ICluster RightCluster { get; }
    }

    public class Cluster : ICluster
    {
        public Cluster(ICluster left, ICluster right, float distance)
        {
            this.LeftCluster = left;
            this.RightCluster = right;
            this.Distance = distance;
        }

        public ICluster LeftCluster { get; }

        public ICluster RightCluster { get; }

        public float Distance { get; }
    }

    public class LeafCluster : ICluster
    {
        private readonly Vector2 coordinates;

        public LeafCluster(Vector2 coordinates)
        {
            this.coordinates = coordinates;
        }

        public int ClusterIndex { get; set; }

        public Vector2 Coordinates => this.coordinates;

        public float Distance => 0f;

        public ICluster LeftCluster => null;

        public ICluster RightCluster => null;
    }

    public class DendrogramBuilder
    {
        public static ICluster ClusterPoints(IEnumerable<Vector2> points)
        {
            var timer = Stopwatch.StartNew();
            IList<IList<float>> distanceMatrix = BuildDistanceMatrix(points.ToArray());
            IList<ICluster> clusters = points.Select(p => (ICluster)new LeafCluster(p)).ToList();
            while (clusters.Count > 1)
            {
                Cluster(clusters, distanceMatrix);
            }
            return clusters.Single();
        }

        private static IList<IList<float>> BuildDistanceMatrix(IList<Vector2> points)
        {
            IList<IList<float>> rows = new List<IList<float>>(points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                var row = new List<float>(i);
                rows.Add(row);
                for (int j = 0; j < i; j++)
                {
                    row.Add(Vector2.DistanceSquared(points[i], points[j]));
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
                distanceMatrix[secondIndex][firstIndex]);

            clusters[firstIndex] = commonCluster;
            clusters.RemoveAt(secondIndex);
            RecalculateDistancesToMergedCluster(distanceMatrix, firstIndex, secondIndex);
            RemoveClusterFromDistanceMatrix(distanceMatrix, secondIndex);
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

        private static void RecalculateDistancesToMergedCluster(
            IList<IList<float>> distanceMatrix, 
            int replaceClusterIndex,
            int deleteClusterIndex)
        {
            for(int i = 0; i < replaceClusterIndex; i++)
            {
                distanceMatrix[replaceClusterIndex][i] = Math.Min(
                    distanceMatrix[replaceClusterIndex][i],
                    distanceMatrix[deleteClusterIndex][i]
                );
            }
            for(int i = replaceClusterIndex + 1; i < deleteClusterIndex; i++)
            {
                distanceMatrix[i][replaceClusterIndex] = Math.Min(
                    distanceMatrix[i][replaceClusterIndex],
                    distanceMatrix[deleteClusterIndex][i]
                );
            }
            for(int i = deleteClusterIndex + 1; i < distanceMatrix.Count; i++)
            {
                distanceMatrix[i][replaceClusterIndex] = Math.Min(
                    distanceMatrix[i][replaceClusterIndex],
                    distanceMatrix[i][deleteClusterIndex]
                );
            }
        }
    }
}
