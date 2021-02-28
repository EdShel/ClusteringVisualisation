using ClusteringVisualisation.Clustering;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ClusteringVisualisation.Shared
{
    public partial class Dendrogram : IClusteringMethod
    {
        private ICluster topCluster;

        public void StartClustering(IEnumerable<Point> points)
        {
            Console.WriteLine("Started clustering");
            this.topCluster = DendrogramBuilder.ClusterPoints(points);
            this.StateHasChanged();
        }

        private int GetClustersTotal(ICluster cluster)
        {
            if (cluster.LeftCluster == null)
            {
                return 1;
            }
            return 1 + GetClustersTotal(cluster.LeftCluster) + GetClustersTotal(cluster.RightCluster);
        }

        private int GetHeight(ICluster cluster)
        {
            if (cluster.LeftCluster == null)
            {
                return 1;
            }
            return 1 + Math.Max(GetHeight(cluster.LeftCluster), GetHeight(cluster.RightCluster));
        }

        private interface ILine
        {
            string GetClass();

            string GetStyle();
        }

        private static string ToPercent(float value)
        {
            return $"{(value * 100f).ToString(CultureInfo.InvariantCulture)}%";
        }

        private class VerticalLine : ILine
        {
            private float topY;

            private float bottomY;

            private float x;

            public VerticalLine(float topY, float bottomY, float x)
            {
                this.topY = topY;
                this.bottomY = bottomY;
                this.x = x;
            }

            public string GetClass()
            {
                return "line vertical";
            }

            public string GetStyle()
            {
                return $"left: {ToPercent(x)};" +
                       $"bottom: {ToPercent(bottomY)};" +
                       $"height: {ToPercent(topY - bottomY)};";
            }
        }

        private class HorizontalLine : ILine
        {
            private float leftX;

            private float rightX;

            private float y;

            public HorizontalLine(float leftX, float rightX, float y)
            {
                this.leftX = leftX;
                this.rightX = rightX;
                this.y = y;
            }

            public string GetClass()
            {
                return "line horizontal";
            }

            public string GetStyle()
            {
                return $"bottom: {ToPercent(y)};" +
                       $"left: {ToPercent(leftX)};" +
                       $"width: {ToPercent(rightX - leftX)}";
            }
        }
    }
}
