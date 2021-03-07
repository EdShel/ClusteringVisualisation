using ClusteringVisualisation.Clustering;
using ClusteringVisualisation.Clustering.KMeans;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;

namespace ClusteringVisualisation.Shared
{
    public partial class KMeansClustering : IClusteringMethod
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

        private int clustersCount = 4;

        private KMeansClusterer clusterer;

        public void StartClustering(IEnumerable<Vector2> points)
        {
            this.clusterer = new KMeansClusterer(points, this.clustersCount);
            StateHasChanged();
        }

        private void OnClustersCountChanged(ChangeEventArgs e)
        {
            this.clustersCount = int.Parse(e.Value.ToString());
            RandomizeCentroids();
        }

        private void RandomizeCentroids()
        {
            this.clusterer.ResetAndRandomizeCentroids(this.clustersCount);
            StateHasChanged();
        }

        private void NextStep()
        {
            this.clusterer.NextStep();
            StateHasChanged();
        }

        private static string GetPointStyle(Point point)
        {
            var culture = CultureInfo.InvariantCulture;
            return $"left: {(point.Coordinates.X * 100f).ToString(culture)}%;" +
                    $"top: {(point.Coordinates.Y * 100f).ToString(culture)}%;" +
                    $"animation-delay: {(point.Coordinates - new Vector2(0.5f, 0.5f)).Length().ToString(culture)}s;" +
                    $"background-color: {clusterColors[point.ClusterIndex]}";
        }
    }
}
