using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ClusteringVisualisation.Clustering.SingleLinkage
{
    /// <summary>
    /// Performs hierarchical clustering using
    /// Single Linkage clustering method.
    /// </summary>
    public class SingleLinkageClusterer
    {
        private readonly Vector2[] points;

        /// <summary>
        /// Contains distances between clusters.
        /// The structure is following
        /// (rows are outer list elements and 
        /// Xs are elements of an inner list):
        /// 0\1 A B C D E
        /// A   - - - - -
        /// B   X - - - - 
        /// C   X X - - -
        /// D   X X X - -
        /// E   X X X X -
        /// </summary>
        private IList<IList<float>> distanceMatrix;

        /// <summary>
        /// All clusters left.
        /// </summary>
        private IList<ICluster> clusters;

        /// <summary>
        /// Creates new clustering context.
        /// </summary>
        /// <param name="points">Points we need to cluster.</param>
        public SingleLinkageClusterer(IEnumerable<Vector2> points)
        {
            this.points = points.ToArray();
        }

        /// <summary>
        /// Clusters all the points and returns the top-level cluster
        /// of the hierarchy.
        /// </summary>
        public ICluster GetTopLevelCluster()
        {
            this.distanceMatrix = BuildDistanceMatrix();
            this.clusters = this.points.Select(p => (ICluster)new LeafCluster()).ToList();
            // Repeat 'till there is only one cluster left
            while (this.clusters.Count > 1)
            {
                CombineNearestClusters();
            }
            return this.clusters.Single();
        }

        /// <summary>
        /// Builds jagged array of distances between points.
        /// </summary>
        private IList<IList<float>> BuildDistanceMatrix()
        {
            IList<IList<float>> rows = new List<IList<float>>(this.points.Length);
            for (int i = 0; i < this.points.Length; i++)
            {
                var row = new List<float>(i);
                rows.Add(row);
                for (int j = 0; j < i; j++)
                {
                    // Taking distance sqr because it is much faster
                    row.Add(Vector2.DistanceSquared(this.points[i], this.points[j]));
                }
            }
            return rows;
        }

        /// <summary>
        /// Combines two nearest clusters.
        /// </summary>
        private void CombineNearestClusters()
        {
            FindNearestClustersIndices(out int firstIndex, out int secondIndex);
            ICluster firstCluster = this.clusters[firstIndex];
            ICluster secondCluster = this.clusters[secondIndex];
            ICluster commonCluster = new Cluster(
                firstCluster,
                secondCluster,
                this.distanceMatrix[secondIndex][firstIndex]
            );

            // Replace first cluster with their combination
            this.clusters[firstIndex] = commonCluster;
            // And the second one will be deleted
            this.clusters.RemoveAt(secondIndex);

            // Find distances to the combined cluster
            RecalculateDistancesToMergedCluster(firstIndex, secondIndex);
            // Remove distances to the deleted cluster
            RemoveClusterFromDistanceMatrix(secondIndex);
        }

        /// <summary>
        /// Searches for the pair of nearest clusters and
        /// returns their indices.
        /// </summary>
        /// <param name="firstIndex">The least index.</param>
        /// <param name="secondIndex">The greatest index.</param>
        private void FindNearestClustersIndices(out int firstIndex, out int secondIndex)
        {
            firstIndex = -1;
            secondIndex = -1;

            float minDistance = float.MaxValue;

            for (int i = 0; i < this.distanceMatrix.Count; i++)
            {
                var row = this.distanceMatrix[i];
                for (int j = 0; j < row.Count; j++)
                {
                    float distanceBetweenClusters = row[j];
                    if (distanceBetweenClusters < minDistance)
                    {
                        firstIndex = j;
                        secondIndex = i;
                        minDistance = distanceBetweenClusters;
                    }
                }
            }
        }

        /// <summary>
        /// Replaces distances to the first cluster
        /// with min distances to its subclusters.
        /// </summary>
        /// <param name="replaceClusterIndex">Index of the cluster to replace.</param>
        /// <param name="deleteClusterIndex">Index of the cluster to be deleted.</param>
        private void RecalculateDistancesToMergedCluster(
            int replaceClusterIndex,
            int deleteClusterIndex)
        {
            // We need 3 loops here 'cause we use
            // jagged array to save computational time while 
            // calculating distances.
            // What we simply doing here is just taking the min distance
            // from each cluster to the first cluster (the one we replace).

            // Alternatively we can rebuild the whole distance matrix
            // from scratch, but this approach is much-much faster.

            for (int i = 0; i < replaceClusterIndex; i++)
            {
                this.distanceMatrix[replaceClusterIndex][i] = Math.Min(
                    this.distanceMatrix[replaceClusterIndex][i],
                    this.distanceMatrix[deleteClusterIndex][i]
                );
            }
            for (int i = replaceClusterIndex + 1; i < deleteClusterIndex; i++)
            {
                this.distanceMatrix[i][replaceClusterIndex] = Math.Min(
                    this.distanceMatrix[i][replaceClusterIndex],
                    this.distanceMatrix[deleteClusterIndex][i]
                );
            }
            for (int i = deleteClusterIndex + 1; i < this.distanceMatrix.Count; i++)
            {
                this.distanceMatrix[i][replaceClusterIndex] = Math.Min(
                    this.distanceMatrix[i][replaceClusterIndex],
                    this.distanceMatrix[i][deleteClusterIndex]
                );
            }
        }

        /// <summary>
        /// Removes cluster from distance matrix.
        /// </summary>
        private void RemoveClusterFromDistanceMatrix(int clusterIndex)
        {
            for (int i = clusterIndex + 1; i < this.distanceMatrix.Count; i++)
            {
                this.distanceMatrix[i].RemoveAt(clusterIndex);
            }
            this.distanceMatrix.RemoveAt(clusterIndex);
        }
    }
}
