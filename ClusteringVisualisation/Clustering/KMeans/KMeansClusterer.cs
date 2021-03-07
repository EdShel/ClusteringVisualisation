using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ClusteringVisualisation.Clustering.KMeans
{
    /// <summary>
    /// Provides interface for clustering points
    /// using K-means.
    /// </summary>
    public class KMeansClusterer
    {
        private readonly Point[] points = Array.Empty<Point>();

        private Point[] clusterCenters = Array.Empty<Point>();

        private int clusterizationIteration = 0;

        /// <summary>
        /// Creates new clustering context.
        /// </summary>
        /// <param name="points">Points to cluster.</param>
        /// <param name="centroidsCount">How many clusters to create.</param>
        public KMeansClusterer(IEnumerable<Vector2> points, int centroidsCount)
        {
            this.points = points.Select(p => new Point { Coordinates = p }).ToArray();
            this.clusterCenters = CreateClusterCenters(centroidsCount);
            this.clusterizationIteration = 0;
        }

        /// <summary>
        /// Index of the clusterization phase.
        /// </summary>
        public int ClusterizationIteration => this.clusterizationIteration;

        /// <summary>
        /// All the points and their cluster indices.
        /// </summary>
        public IEnumerable<Point> Points => this.points;

        /// <summary>
        /// All the cluster centers' coordinates and cluster indices.
        /// </summary>
        public IEnumerable<Point> Centroids => this.clusterCenters;

        /// <summary>
        /// Makes new centroids and resets the clustering process.
        /// </summary>
        /// <param name="centroidsCount"></param>
        public void ResetAndRandomizeCentroids(int centroidsCount)
        {
            // If points have clusters
            if (this.clusterizationIteration != 0)
            {
                this.clusterizationIteration = 0;
                // Reset their clusters
                foreach (var p in this.points)
                {
                    p.ClusterIndex = 0;
                }
            }
            this.clusterCenters = CreateClusterCenters(centroidsCount);
        }

        /// <summary>
        /// Generate cluster centers;
        /// </summary>
        private static Point[] CreateClusterCenters(int centroidsCount)
        {
            var points = new Point[centroidsCount];
            var rng = new Random();
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new Point
                {
                    Coordinates = new Vector2(
                        x: (float)rng.NextDouble(),
                        y: (float)rng.NextDouble()
                    ),
                    ClusterIndex = i + 1
                };
            }
            return points;
        }

        /// <summary>
        /// Progress to the next step of K-means clustering.
        /// </summary>
        public void NextStep()
        {
            this.clusterizationIteration++;
            ApplyPointsToNearestCluster();
            RecalculateClusterCenters();
        }

        /// <summary>
        /// Assigns points' cluster indices according
        /// to the nearest cluster center.
        /// </summary>
        private void ApplyPointsToNearestCluster()
        {
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
        }

        /// <summary>
        /// Finds new cluster centers according to the mean
        /// coordinates of its points.
        /// </summary>
        private void RecalculateClusterCenters()
        {
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
                // Note: this is a special case when the cluster center
                //       has not points assigned to it.
                //       So we don't move it.
                if (pointsOfThisCluster != 0)
                {
                    cluster.Coordinates = coordinatesSum / pointsOfThisCluster;
                }
            }
        }
    }
}
