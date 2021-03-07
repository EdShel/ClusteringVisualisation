namespace ClusteringVisualisation.Clustering.SingleLinkage
{
    public class Cluster : ICluster
    {
        public Cluster(ICluster left, ICluster right, float distance)
        {
            this.LeftCluster = left;
            this.RightCluster = right;
            this.Distance = distance;
        }

        public ICluster LeftCluster { get; }

        public ICluster RightCluster { get; }

        public float Distance { get; }
    }
}
