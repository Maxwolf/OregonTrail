using System.Linq;
using OregonTrailDotNet.Entity.Item;
using OregonTrailDotNet.Module.Scoring;
using WolfCurses.Utility;
using Xunit;

namespace OregonTrailDotNet.Tests.Module
{
    /// <summary>
    ///     Covers high score ratings, the top ten list, and the point-line display objects. All of
    ///     this is pure logic with no game simulation required.
    /// </summary>
    public class ScoringTests
    {
        [Theory]
        [InlineData(0, PerformanceEnum.Greenhorn)]
        [InlineData(2999, PerformanceEnum.Greenhorn)]
        [InlineData(3000, PerformanceEnum.Adventurer)]
        [InlineData(6999, PerformanceEnum.Adventurer)]
        [InlineData(7000, PerformanceEnum.TrailGuide)]
        [InlineData(9999, PerformanceEnum.TrailGuide)]
        public void Highscore_RatesPerformanceByPointThresholds(int points, PerformanceEnum expected)
        {
            var highscore = new Highscore("Test Player", points);

            Assert.Equal(expected.ToDescriptionAttribute(), highscore.Rating);
        }

        [Fact]
        public void DefaultTopTen_HasTenEntriesRankedByPoints()
        {
            var defaults = ScoringModule.DefaultTopTen.ToList();

            Assert.Equal(10, defaults.Count);
            Assert.Equal(defaults.OrderByDescending(score => score.Points), defaults);
            Assert.Equal("Stephen Meek", defaults.First().Name);
            Assert.Equal(7650, defaults.First().Points);
        }

        [Fact]
        public void TopTen_IncludesNewHighScoreInRankedOrder()
        {
            var scoring = new ScoringModule();
            scoring.Add(new Highscore("Test Player", 9999));

            var topTen = scoring.TopTen.ToList();

            Assert.Equal(10, topTen.Count);
            Assert.Equal("Test Player", topTen.First().Name);
        }

        [Fact]
        public void TopTen_DropsScoresThatDoNotMakeTheCut()
        {
            var scoring = new ScoringModule();
            scoring.Add(new Highscore("Test Player", 1));

            var topTen = scoring.TopTen.ToList();

            Assert.Equal(10, topTen.Count);
            Assert.DoesNotContain(topTen, score => score.Name == "Test Player");
        }

        [Fact]
        public void Reset_RestoresDefaultTopTen()
        {
            var scoring = new ScoringModule();
            scoring.Add(new Highscore("Test Player", 9999));
            scoring.Reset();

            Assert.Equal(ScoringModule.DefaultTopTen.Select(score => score.Points),
                scoring.TopTen.Select(score => score.Points));
        }

        [Fact]
        public void Points_ToString_ShowsPerAmountWhenAboveOne()
        {
            // Food awards points per 25 pounds.
            Assert.Equal("Food (per 25)", new Points(Resources.Food).ToString());
        }

        [Fact]
        public void Points_ToString_OmitsPerAmountOfOne()
        {
            // Oxen award points per single animal.
            Assert.Equal("Oxen", new Points(Parts.Oxen).ToString());
        }

        [Fact]
        public void Points_ToString_PrefersOptionalDisplayName()
        {
            Assert.Equal("Pounds of Food (per 25)", new Points(Resources.Food, "Pounds of Food").ToString());
        }
    }
}
