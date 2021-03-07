using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace ClusteringVisualisation.Clustering
{
    public class GaussianPointsGenerator
    {
        public readonly Random rng;

        private readonly double stdev;

        private readonly RectangleF area;

        private readonly int clustersCount;

        public GaussianPointsGenerator(Random rng, double stdev, RectangleF area, int clustersCount)
        {
            this.rng = rng;
            this.stdev = stdev;
            this.area = area;
            this.clustersCount = clustersCount;
        }

        public IEnumerable<Vector2> GeneratePoints(int pointsCount)
        {
            Vector2[] clusters = Enumerable.Range(0, this.clustersCount)
                .Select(_ => GenerateClusterCoordinates())
                .ToArray();

            for (int i = 0; i < pointsCount; i++)
            {
                Vector2 point;
                do
                {
                    int clusterIndex = this.rng.Next(clusters.Length);
                    point = new Vector2(
                        clusters[clusterIndex].X + (float)this.rng.NextGaussian(this.stdev),
                        clusters[clusterIndex].Y + (float)this.rng.NextGaussian(this.stdev)
                    );
                }
                while (!this.area.Contains(point.X, point.Y));
                yield return point;
            }
        }

        private Vector2 GenerateClusterCoordinates()
        {
            return new Vector2(
                (float)this.rng.NextDouble() * this.area.Width + this.area.X,
                (float)this.rng.NextDouble() * this.area.Height + this.area.Y
            );
        }
    }

    public class Point
    {
        private static readonly string[] clusterColors = new[]
        {
            "#000000",
            "#FF3838",
            "#FFCF38",
            "#9CFF38",
            "#38FFF3",
            "#387BFF",
            "#7438FF",
            "#CF38FF",
            "#FF38E1",
            "#38FFB1",
            "#A396AA",
            "#8DE2A8"
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
