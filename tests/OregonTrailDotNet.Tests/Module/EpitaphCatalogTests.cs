using OregonTrailDotNet.Module.Tombstone;
using Xunit;

namespace OregonTrailDotNet.Tests.Module
{
    /// <summary>
    ///     Covers the pool of silly default epitaphs: every entry must be short enough to fit on a tombstone (so a random
    ///     default never overflows the epitaph length limit), and a random pick always returns one of the pooled messages.
    /// </summary>
    public class EpitaphCatalogTests
    {
        // Mirrors EpitaphEditor.EPITAPH_MAXLENGTH — the longest an epitaph is allowed to be once typed and truncated.
        private const int EpitaphMaxLength = 38;

        [Fact]
        public void Pool_Is_Not_Empty_And_Includes_The_Classic_Silly_Messages()
        {
            Assert.NotEmpty(EpitaphCatalog.All);
            Assert.Contains("pepperoni and cheese", EpitaphCatalog.All);
            Assert.Contains("welp", EpitaphCatalog.All);
            Assert.Contains("HECK", EpitaphCatalog.All);
        }

        [Fact]
        public void Every_Epitaph_Fits_Within_The_Length_Limit()
        {
            Assert.All(EpitaphCatalog.All, epitaph =>
            {
                Assert.False(string.IsNullOrWhiteSpace(epitaph));
                Assert.True(epitaph.Length <= EpitaphMaxLength, $"'{epitaph}' is {epitaph.Length} chars, over the limit");
            });
        }

        [Fact]
        public void Random_Always_Returns_A_Pooled_Message()
        {
            for (var i = 0; i < 200; i++)
                Assert.Contains(EpitaphCatalog.Random(), EpitaphCatalog.All);
        }
    }
}
