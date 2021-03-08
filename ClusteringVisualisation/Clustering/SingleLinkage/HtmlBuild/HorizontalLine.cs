namespace ClusteringVisualisation.Clustering.SingleLinkage.HtmlBuild
{
    public class HorizontalLine : IDendrogramElement
    {
        private float leftX;

        private float rightX;

        private float y;

        public HorizontalLine(float leftX, float rightX, float y)
        {
            this.leftX = leftX;
            this.rightX = rightX;
            this.y = y;
        }

        public string GetClass()
        {
            return "line horizontal";
        }

        public string GetStyle()
        {
            return $"bottom: {CssHelper.ToPercent(this.y)};" +
                   $"left: {CssHelper.ToPercent(this.leftX)};" +
                   $"width: {CssHelper.ToPercent(this.rightX - this.leftX)}";
        }
    }
}
