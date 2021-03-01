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

        private IEnumerable<IDendrogramElement> GetDendrogramLines()
        {
            return new DendrogramLinesGenerator(GetClustersTotal(this.topCluster), this.topCluster.Distance)
                .GetDendrogramElements(this.topCluster, this.topCluster.Distance)
                .Where(l => l != null);
        }

        private class DendrogramLinesGenerator
        {
            private int x;

            private readonly float maxX;

            private readonly float maxDistance;

            public DendrogramLinesGenerator(float maxX, float maxDistance)
            {
                this.maxX = maxX;
                this.maxDistance = maxDistance;
            }

            public IEnumerable<IDendrogramElement> GetDendrogramElements(ICluster cluster, float callerDistance)
            {
                bool isLeaf = cluster.LeftCluster == null;
                if (isLeaf)
                {
                    yield return new LeafCluster(
                        x: this.x / this.maxX,
                        y: cluster.Distance / this.maxDistance
                    );
                    yield return new VerticalLine(
                        topY: callerDistance / this.maxDistance,
                        bottomY: cluster.Distance / this.maxDistance,
                        x: this.x / this.maxX
                    );
                    yield return null;
                    yield break;
                }

                int leftClusterX = this.x;
                int rightClusterX = this.x;

                foreach (var leftClusterLine in GetDendrogramElements(cluster.LeftCluster, cluster.Distance))
                {
                    if (leftClusterLine == null)
                    {
                        leftClusterX = this.x;
                    }
                    else
                    {
                        yield return leftClusterLine;
                    }
                }

                this.x++;
                yield return null;

                yield return new VerticalLine(
                    topY: callerDistance / this.maxDistance,
                    bottomY: cluster.Distance / this.maxDistance,
                    x: this.x / this.maxX
                );
                this.x++;

                foreach (var rightClusterLine in GetDendrogramElements(cluster.RightCluster, cluster.Distance))
                {
                    if (rightClusterLine == null)
                    {
                        rightClusterX = this.x;
                    }
                    else
                    {
                        yield return rightClusterLine;
                    }
                }

                yield return new HorizontalLine(
                    leftX: leftClusterX / this.maxX,
                    rightX: rightClusterX / this.maxX,
                    y: cluster.Distance / this.maxDistance
                );
            }
        }

        private static string ToPercent(float value)
        {
            return $"{(value * 100f).ToString(CultureInfo.InvariantCulture)}%";
        }

        private interface IDendrogramElement
        {
            string GetClass();

            string GetStyle();
        }

        private class LeafCluster : IDendrogramElement
        {
            private float x;

            private float y;

            public LeafCluster(float x, float y)
            {
                this.x = x;
                this.y = y;
            }

            public string GetClass()
            {
                return "point";
            }

            public string GetStyle()
            {
                return $"left: {ToPercent(x)};" +
                       $"bottom: {ToPercent(y)}";
            }
        }

        private class VerticalLine : IDendrogramElement
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

        private class HorizontalLine : IDendrogramElement
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
