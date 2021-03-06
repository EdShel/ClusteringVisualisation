namespace System
{
    public static class RandomExtensions
    {
        public static double NextGaussian(this Random rng, double stdev)
        {
            double u1 = 1.0 - rng.NextDouble();
            double u2 = 1.0 - rng.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0d * Math.Log(u1))
                * Math.Sin(2.0 * Math.PI * u2);
            return stdev * randStdNormal;
        }
    }
}
