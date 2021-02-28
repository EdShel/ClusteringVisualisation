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

        public void StartClustering(IEnumerable<Point> points)
        {
            this.points = points.ToArray();
            this.clusterCenters = CreateClusterCenters(4);
            StateHasChanged();
        }

        private static Point[] CreateClusterCenters(int clustersCount)
        {
            var points = new Point[clustersCount];
            var rng = new Random();
            for(int i  = 0; i < points.Length; i++)
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
            foreach(var point in this.points)
            {
                float minDistance = float.MaxValue;
                foreach(var cluster in this.clusterCenters)
                {
                    float distance = (cluster.Coordinates - point.Coordinates).LengthSquared();
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        point.ClusterIndex = cluster.ClusterIndex;
                    }
                }
            }
            foreach(var cluster in this.clusterCenters)
            {
                int pointsOfThisCluster = 0;
                Vector2 coordinatesSum = Vector2.Zero;
                foreach(var point in this.points)
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
