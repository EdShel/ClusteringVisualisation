namespace ClusteringVisualisation.Clustering.SingleLinkage.HtmlBuild
{
    public class LeafCluster : IDendrogramElement
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
            return $"left: {CssHelper.ToPercent(this.x)};" +
                   $"bottom: {CssHelper.ToPercent(this.y)}";
        }
    }
}
