using Xunit;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>Covers the shared block-character sparkline used by the training-stats screen and the benchmark dashboard.</summary>
    public sealed class SparklineTests
    {
        [Fact]
        public void Renders_Blocks_Scaled_Between_Min_And_Max()
        {
            Assert.Equal("(no data)", Sparkline.Render(Array.Empty<double>()));

            var line = Sparkline.Render(new double[] { 0, 5, 10 });
            Assert.Equal(3, line.Length);
            Assert.Equal('▁', line[0]);  // lowest value -> lowest block
            Assert.Equal('█', line[^1]); // highest value -> full block
        }
    }
}
