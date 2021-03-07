namespace ClusteringVisualisation.Clustering.SingleLinkage
{
    public class LeafCluster : ICluster
    {
        public ICluster LeftCluster => null;

        public ICluster RightCluster => null;

        public float Distance => 0f;
    }
}
