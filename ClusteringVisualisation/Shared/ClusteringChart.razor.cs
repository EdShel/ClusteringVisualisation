using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace ClusteringVisualisation.Shared
{
    public partial class ClusteringChart
    {
        private Point[] points = new Point[0];

        private Point[] clusterCenters = new Point[0];

        public void StartClustering(int pointsCount)
        {
            this.points = CreatePoints(pointsCount);
            this.clusterCenters = CreateClusterCenters(4);
            StateHasChanged();
        }

        private static Point[] CreatePoints(int count)
        {
            var points = new Point[count];
            var rng = new Random();
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new Point();
                points[i].Coordinates = new Vector2(
                    x: (float)rng.NextDouble(),
                    y: (float)rng.NextDouble()
                );
            }
            return points;
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

        }

        private class Point
        {
            private static readonly string[] clusterColors = new[]
            {
                "#000000",
                "#ff0000",
                "#00ff00",
                "#0000ff",
                "#ffff00"
            };

            public Vector2 Coordinates;

            public int ClusterIndex;

            public string GetPointStyle()
            {
                var culture = CultureInfo.InvariantCulture;
                return $"left: {(this.Coordinates.X * 100f).ToString(culture)}%;" +
                        $"top: {(this.Coordinates.Y * 100f).ToString(culture)}%;" +
                        $"animation-delay: {this.Coordinates.Length().ToString(culture)}s;" +
                        $"background-color: {clusterColors[this.ClusterIndex]}";
            }
        }
    }
}
