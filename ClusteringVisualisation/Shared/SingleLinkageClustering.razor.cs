using ClusteringVisualisation.Clustering;
using ClusteringVisualisation.Clustering.SingleLinkage;
using ClusteringVisualisation.Clustering.SingleLinkage.HtmlBuild;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ClusteringVisualisation.Shared
{
    public partial class SingleLinkageClustering : IClusteringMethod
    {
        private ICluster topCluster;

        private float axisValue = 0f;

        public void StartClustering(IEnumerable<Vector2> points)
        {
            this.topCluster = new SingleLinkageClusterer(points).GetTopLevelCluster();
            StateHasChanged();
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
    }
}
