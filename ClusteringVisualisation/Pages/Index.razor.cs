using ClusteringVisualisation.Clustering;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Numerics;

namespace ClusteringVisualisation.Pages
{
    public partial class Index
    {
        private const int MIN_POINTS = 1;

        private const int MAX_POINTS = 500;

        private const int MIN_RNG_SEED = 0;

        private const int MAX_RNG_SEED = 100500;

        private bool needToClusterAfterRender = true;

        private int methodIndex = 0;

        private IClusteringMethod clusteringMethod;

        private int pointsCount = 100;

        private int clustersCount = 4;

        private float stdev = 0.75f;

        private float areaSize = 10f;

        private int randomSeed = 0;

        private void OnClusteringMethodChanged(ChangeEventArgs e)
        {
            this.methodIndex = int.Parse(e.Value?.ToString() ?? "0");
            needToClusterAfterRender = true;
        }

        private void OnPointsCountChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString() ?? "0", out int parsed))
            {
                this.pointsCount = Math.Clamp(parsed, MIN_POINTS, MAX_POINTS);
                StartClustering();
            }
        }

        private void OnClustersCountChanged(ChangeEventArgs e)
        {
            this.clustersCount = int.Parse(e.Value?.ToString() ?? "0");
            StartClustering();
        }

        private void OnStdevChanged(ChangeEventArgs e)
        {
            this.stdev = float.Parse(e.Value?.ToString() ?? "0", System.Globalization.CultureInfo.InvariantCulture);
            StartClustering();
        }

        private void OnAreaSizeChanged(ChangeEventArgs e)
        {
            this.areaSize = float.Parse(e.Value?.ToString() ?? "0", System.Globalization.CultureInfo.InvariantCulture);
            StartClustering();
        }

        private void OnRandomSeedChanged(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString() ?? "0", out int parsed))
            {
                this.randomSeed = Math.Clamp(parsed, MIN_RNG_SEED, MAX_RNG_SEED);
                StartClustering();
            }
        }

        private void RandomizeSeed()
        {
            this.randomSeed = new Random().Next(MIN_RNG_SEED, MAX_RNG_SEED);
            StartClustering();
        }

        private void StartClustering()
        {
            var points = CreatePoints();
            clusteringMethod.StartClustering(points);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (needToClusterAfterRender)
            {
                needToClusterAfterRender = false;
                StartClustering();
            }
        }

        private Vector2[] CreatePoints()
        {
            return new GaussianPointsGenerator(
                rng: new Random(this.randomSeed),
                stdev: this.stdev,
                area: new System.Drawing.RectangleF(
                    x: -this.areaSize / 2f,
                    y: -this.areaSize / 2f,
                    width: this.areaSize,
                    height: this.areaSize
                ),
                clustersCount: this.clustersCount
            )
            .GeneratePoints(this.pointsCount)
            .Select(p => new Vector2(
                    x: p.X.Map01(-this.areaSize / 2f, this.areaSize / 2f),
                    y: p.Y.Map01(-this.areaSize / 2f, this.areaSize / 2f)
                )
            )
            .ToArray();
        }
    }
}
