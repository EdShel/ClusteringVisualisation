namespace ClusteringVisualisation.Clustering.SingleLinkage
{
    public interface ICluster
    {
        float Distance { get; }

        ICluster LeftCluster { get; }

        ICluster RightCluster { get; }
    }
}
