using ClusteringVisualisation.Clustering;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace ClusteringVisualisation.Shared
{
    public partial class ClusteringChart : IClusteringMethod
    {
        private Point[] points = new Point[0];

        private Point[] clusterCenters = new Point[0];

        private int clustersCount = 4;

        private int clusterizationIteration = 0;

        public void StartClustering(IEnumerable<Vector2> points)
        {
            this.points = points.Select(p => new Point { Coordinates = p }).ToArray();
            this.clusterCenters = CreateClusterCenters();
            this.clusterizationIteration = 0;
            StateHasChanged();
        }

        private void OnClustersCountChanged(ChangeEventArgs e)
        {
            this.clustersCount = int.Parse(e.Value.ToString());
            RandomizeCentroids();
        }

        private void RandomizeCentroids()
        {
            if (this.clusterizationIteration != 0)
            {
                this.clusterizationIteration = 0;
                foreach(var p in this.points)
                {
                    p.ClusterIndex = 0;
                }
            }
            this.clusterCenters = CreateClusterCenters();
            StateHasChanged();
        }

        private Point[] CreateClusterCenters()
        {
            var points = new Point[this.clustersCount];
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

        private void NextStep()
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

            StateHasChanged();
        }
    }
}
