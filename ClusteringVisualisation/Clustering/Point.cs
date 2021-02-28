using System.Globalization;
using System.Numerics;

namespace ClusteringVisualisation.Clustering
{
    public class Point
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
