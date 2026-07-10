namespace OregonTrailDotNet.Bot.Learning
{
    /// <summary>Shared sampling helpers for the optimizers.</summary>
    internal static class Sampling
    {
        /// <summary>A standard-normal sample via Box-Muller.</summary>
        public static double Gaussian(Random rng)
        {
            var u1 = 1.0 - rng.NextDouble();
            var u2 = 1.0 - rng.NextDouble();
            return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
        }
    }
}
