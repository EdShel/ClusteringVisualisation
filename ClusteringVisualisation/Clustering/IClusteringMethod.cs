using System.Collections.Generic;
using System.Numerics;

namespace ClusteringVisualisation.Clustering
{
    public interface IClusteringMethod
    {
        void StartClustering(IEnumerable<Vector2> points);
    }
}
