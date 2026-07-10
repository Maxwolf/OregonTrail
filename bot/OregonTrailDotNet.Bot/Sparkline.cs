using System.Text;

namespace OregonTrailDotNet.Bot
{
    /// <summary>
    ///     Renders a numeric series as a compact single-line Unicode block sparkline, scaled to its own min/max. Shared by the
    ///     training-stats screen and the live benchmark dashboard.
    /// </summary>
    public static class Sparkline
    {
        private const string Blocks = "▁▂▃▄▅▆▇█";

        public static string Render(IEnumerable<double> series)
        {
            var values = series.ToList();
            if (values.Count == 0)
                return "(no data)";

            var min = values.Min();
            var max = values.Max();
            var range = max - min;

            var sb = new StringBuilder(values.Count);
            foreach (var v in values)
            {
                var level = range <= 0 ? Blocks.Length / 2 : (int) ((v - min) / range * (Blocks.Length - 1));
                sb.Append(Blocks[Math.Clamp(level, 0, Blocks.Length - 1)]);
            }

            return sb.ToString();
        }
    }
}
