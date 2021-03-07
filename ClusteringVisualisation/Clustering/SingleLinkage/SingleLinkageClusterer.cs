using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ClusteringVisualisation.Clustering.SingleLinkage
{
    public class SingleLinkageClusterer
    {
        private readonly Vector2[] points;

        private IList<IList<float>> distanceMatrix;

        private IList<ICluster> clusters;

        public SingleLinkageClusterer(IEnumerable<Vector2> points)
        {
            this.points = points.ToArray();
        }

        public ICluster GetTopLevelCluster()
        {
            this.distanceMatrix = BuildDistanceMatrix();
            this.clusters = this.points.Select(p => (ICluster)new LeafCluster()).ToList();
            while (this.clusters.Count > 1)
            {
                Cluster();
            }
            return this.clusters.Single();
        }

        private IList<IList<float>> BuildDistanceMatrix()
        {
            IList<IList<float>> rows = new List<IList<float>>(this.points.Length);
            for (int i = 0; i < this.points.Length; i++)
            {
                var row = new List<float>(i);
                rows.Add(row);
                for (int j = 0; j < i; j++)
                {
                    row.Add(Vector2.DistanceSquared(this.points[i], this.points[j]));
                }
            }
            return rows;
        }

        private void Cluster()
        {
            FindNearestClustersIndices(out int firstIndex, out int secondIndex);
            ICluster firstCluster = this.clusters[firstIndex];
            ICluster secondCluster = this.clusters[secondIndex];
            ICluster commonCluster = new Cluster(
                firstCluster, 
                secondCluster,
                this.distanceMatrix[secondIndex][firstIndex]
            );

            clusters[firstIndex] = commonCluster;
            clusters.RemoveAt(secondIndex);
            RecalculateDistancesToMergedCluster(firstIndex, secondIndex);
            RemoveClusterFromDistanceMatrix(secondIndex);
        }

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

        private void RemoveClusterFromDistanceMatrix(int clusterIndex)
        {
            for(int i = clusterIndex + 1; i < this.distanceMatrix.Count; i++)
            {
                this.distanceMatrix[i].RemoveAt(clusterIndex);
            }
            this.distanceMatrix.RemoveAt(clusterIndex);
        }

        private void RecalculateDistancesToMergedCluster(
            int replaceClusterIndex,
            int deleteClusterIndex)
        {
            for(int i = 0; i < replaceClusterIndex; i++)
            {
                this.distanceMatrix[replaceClusterIndex][i] = Math.Min(
                    this.distanceMatrix[replaceClusterIndex][i],
                    this.distanceMatrix[deleteClusterIndex][i]
                );
            }
            for(int i = replaceClusterIndex + 1; i < deleteClusterIndex; i++)
            {
                this.distanceMatrix[i][replaceClusterIndex] = Math.Min(
                    this.distanceMatrix[i][replaceClusterIndex],
                    this.distanceMatrix[deleteClusterIndex][i]
                );
            }
            for(int i = deleteClusterIndex + 1; i < distanceMatrix.Count; i++)
            {
                this.distanceMatrix[i][replaceClusterIndex] = Math.Min(
                    this.distanceMatrix[i][replaceClusterIndex],
                    this.distanceMatrix[i][deleteClusterIndex]
                );
            }
        }
    }
}
