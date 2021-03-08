using System.Collections.Generic;

namespace ClusteringVisualisation.Clustering.SingleLinkage.HtmlBuild
{
    public class DendrogramLinesGenerator
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
}
