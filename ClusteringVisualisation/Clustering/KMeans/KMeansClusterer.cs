using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ClusteringVisualisation.Clustering.KMeans
{
    public class KMeansClusterer
    {
        private Point[] points = Array.Empty<Point>();

        private Point[] clusterCenters = Array.Empty<Point>();

        private int clusterizationIteration = 0;

        public KMeansClusterer(IEnumerable<Vector2> points, int centroidsCount)
        {
            this.points = points.Select(p => new Point { Coordinates = p }).ToArray();
            this.clusterCenters = CreateClusterCenters(centroidsCount);
            this.clusterizationIteration = 0;
        }

        public int ClusterizationIteration => this.clusterizationIteration;

        public IEnumerable<Point> Points => this.points;

        public IEnumerable<Point> Centroids => this.clusterCenters;

        public void RandomizeCentroids(int centroidsCount)
        {
            if (this.clusterizationIteration != 0)
            {
                this.clusterizationIteration = 0;
                foreach (var p in this.points)
                {
                    p.ClusterIndex = 0;
                }
            }
            this.clusterCenters = CreateClusterCenters(centroidsCount);
        }

        private Point[] CreateClusterCenters(int centroidsCount)
        {
            var points = new Point[centroidsCount];
            var rng = new Random();
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new Point();
                points[i].Coordinates = new Vector2(
                    x: (float)rng.NextDouble(),
                    y: (float)rng.NextDouble()
                );
                points[i].ClusterIndex = i + 1;
            }
            return points;
        }

        public void NextStep()
        {
            this.clusterizationIteration++;

            foreach (var point in this.points)
            {
                float minDistance = float.MaxValue;
                foreach (var cluster in this.clusterCenters)
                {
                    float distance = (cluster.Coordinates - point.Coordinates).LengthSquared();
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        point.ClusterIndex = cluster.ClusterIndex;
                    }
                }
            }
            foreach (var cluster in this.clusterCenters)
            {
                int pointsOfThisCluster = 0;
                Vector2 coordinatesSum = Vector2.Zero;
                foreach (var point in this.points)
                {
                    if (point.ClusterIndex == cluster.ClusterIndex)
                    {
                        pointsOfThisCluster++;
                        coordinatesSum += point.Coordinates;
                    }
                }
                if (pointsOfThisCluster != 0)
                {
                    cluster.Coordinates = coordinatesSum / pointsOfThisCluster;
                }
            }

        }
    }
}
