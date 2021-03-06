namespace System
{
    public static class NumbersExtension
    {
        public static float Map01(this float value, float min, float max)
        {
            return (value - min) / (max - min);
        }
    }
}
