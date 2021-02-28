using System;
using System.Collections.Generic;
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
            IList<ICluster> clusters = points.Select(p => (ICluster)new LeafCluster(p)).ToList();
            while (clusters.Count > 1)
            {
                Cluster(clusters);
            }
            return clusters.Single();
        }

        private static void Cluster(IList<ICluster> clusters)
        {
            float[][] distanceMatrix = BuildDistanceMatrix(clusters);
            FindMinDistanceClustersIndices(distanceMatrix, out int firstIndex, out int secondIndex);
            ICluster firstCluster = clusters[firstIndex];
            ICluster secondCluster = clusters[secondIndex];
            ICluster commonCluster = new Cluster(
                firstCluster, 
                secondCluster, 
                Vector2.Distance(firstCluster.Coordinates, secondCluster.Coordinates));

            clusters[firstIndex] = commonCluster;
            clusters.RemoveAt(secondIndex);
        }

        private static float[][] BuildDistanceMatrix(IList<ICluster> clusters)
        {
            float[][] rows = new float[clusters.Count][];
            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] = new float[i];
                var row = rows[i];
                for (int j = 0; j < row.Length; j++)
                {
                    row[j] = Vector2.DistanceSquared(clusters[i].Coordinates, clusters[j].Coordinates);
                }
            }
            return rows;
        }

        private static void FindMinDistanceClustersIndices(float[][] distanceMatrix, out int firstIndex, out int secondIndex)
        {
            firstIndex = -1;
            secondIndex = -1;

            float minDistance = float.MaxValue;

            for (int i = 0; i < distanceMatrix.Length; i++)
            {
                var row = distanceMatrix[i];
                for (int j = 0; j < row.Length; j++)
                {
                    float distanceBetweenClusters = row[j];
                    if (distanceBetweenClusters < minDistance)
                    {
                        firstIndex = i;
                        secondIndex = j;
                        minDistance = distanceBetweenClusters;
                    }
                }
            }
        }
    }
}
