using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace ClusteringVisualisation.Clustering
{
    /// <summary>
    /// Generates points within a given range
    /// almost equally distributed between
    /// randomly generated clusters.
    /// </summary>
    public class GaussianPointsGenerator
    {
        public readonly Random rng;

        private readonly double stdev;

        private readonly RectangleF area;

        private readonly int clustersCount;

        /// <summary>
        /// Creates new generator.
        /// </summary>
        /// <param name="rng">Random number generator.</param>
        /// <param name="stdev">Standard deviation between points in the cluster.</param>
        /// <param name="area">Range of points coordinates.</param>
        /// <param name="clustersCount">How many clusters will be generated.</param>
        public GaussianPointsGenerator(Random rng, double stdev, RectangleF area, int clustersCount)
        {
            this.rng = rng;
            this.stdev = stdev;
            this.area = area;
            this.clustersCount = clustersCount;
        }

        /// <summary>
        /// Creates random points.
        /// </summary>
        /// <param name="pointsCount">How many points to generate.</param>
        public IEnumerable<Vector2> GeneratePoints(int pointsCount)
        {
            // Generate clusters
            Vector2[] clusters = Enumerable.Range(0, this.clustersCount)
                .Select(_ => GenerateClusterCoordinates())
                .ToArray();

            for (int i = 0; i < pointsCount; i++)
            {
                Vector2 point;
                do
                {
                    // Choose a random cluster
                    int clusterIndex = this.rng.Next(clusters.Length);
                    // Generate a point for it until the point is within the range
                    point = new Vector2(
                        clusters[clusterIndex].X + (float)this.rng.NextGaussian(this.stdev),
                        clusters[clusterIndex].Y + (float)this.rng.NextGaussian(this.stdev)
                    );
                }
                while (!this.area.Contains(point.X, point.Y));
                yield return point;
            }
        }

        /// <summary>
        /// Returns coordinates for new cluster within the range.
        /// </summary>
        private Vector2 GenerateClusterCoordinates()
        {
            return new Vector2(
                (float)this.rng.NextDouble() * this.area.Width + this.area.X,
                (float)this.rng.NextDouble() * this.area.Height + this.area.Y
            );
        }
    }
}
