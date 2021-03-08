using System.Globalization;

namespace ClusteringVisualisation.Clustering.SingleLinkage.HtmlBuild
{
    public static class CssHelper
    {
        public static string ToPercent(float value)
        {
            return $"{(value * 100f).ToString(CultureInfo.InvariantCulture)}%";
        }
    }
}
