namespace ClusteringVisualisation.Clustering.SingleLinkage.HtmlBuild
{
    public class VerticalLine : IDendrogramElement
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
            return $"left: {CssHelper.ToPercent(this.x)};" +
                   $"bottom: {CssHelper.ToPercent(this.bottomY)};" +
                   $"height: {CssHelper.ToPercent(this.topY - this.bottomY)};";
        }
    }
}
