using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;
using Xunit;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>
    ///     Locks in the survival-shaped training objective: the optimizer must prefer parties that arrive with more people in
    ///     better health over parties that merely travelled further. The old fitness rewarded failed runs by distance alone,
    ///     which taught policies to rush and let the party die; these assertions pin the ordering that fixes that.
    /// </summary>
    public sealed class FitnessShapingTests
    {
        private static RunResult Run(GameOutcome outcome, int survivors, int partyHealth, int miles, int score = 0,
            int partySize = 5) =>
            new()
            {
                Outcome = outcome,
                Survivors = survivors,
                PartySize = partySize,
                PartyHealthValue = partyHealth,
                Miles = miles,
                Score = score
            };

        [Fact]
        public void KeepingPeopleAlive_BeatsTravellingFurtherAndDying()
        {
            // A party that stopped early but kept four people alive and healthy (timeout) must outrank one that floored it,
            // got further, and lost the whole party (death) - the exact trade the old distance-only reward got backwards.
            var aliveButShort = Fitness(Run(GameOutcome.Timeout, survivors: 4, partyHealth: 400, miles: 900, score: 4800));
            var deadButFar = Fitness(Run(GameOutcome.Death, survivors: 0, partyHealth: 0, miles: 2000));

            Assert.True(aliveButShort > deadButFar);
        }

        [Fact]
        public void HealthierParty_ScoresHigherThanBarelyAliveParty()
        {
            // Same survivor count and distance; the only difference is health. Arriving healthy must win.
            var healthy = Fitness(Run(GameOutcome.Timeout, survivors: 5, partyHealth: 500, miles: 1000, score: 7500));
            var barelyAlive = Fitness(Run(GameOutcome.Timeout, survivors: 5, partyHealth: 200, miles: 1000, score: 3000));

            Assert.True(healthy > barelyAlive);
        }

        [Fact]
        public void MoreSurvivors_ScoresHigher_AtEqualHealth()
        {
            var fivePeople = Fitness(Run(GameOutcome.Timeout, survivors: 5, partyHealth: 400, miles: 1000, score: 6000));
            var onePerson = Fitness(Run(GameOutcome.Timeout, survivors: 1, partyHealth: 400, miles: 1000, score: 1200));

            Assert.True(fivePeople > onePerson);
        }

        [Fact]
        public void FinishingDominates_EvenAgainstAHealthyTimeout()
        {
            var win = Fitness(Run(GameOutcome.Win, survivors: 5, partyHealth: 500, miles: 2040, score: 7650));
            var timeout = Fitness(Run(GameOutcome.Timeout, survivors: 5, partyHealth: 500, miles: 1500, score: 7500));

            Assert.True(win > timeout);
        }

        [Fact]
        public void AmongDeaths_FurtherStillRanksHigher()
        {
            // With everyone dead the survival term is zero for both, so distance remains a gentle tie-breaker - the optimizer
            // still gets a gradient out of the all-dying early generations rather than a flat zero everywhere.
            var farDeath = Fitness(Run(GameOutcome.Death, survivors: 0, partyHealth: 0, miles: 1500));
            var nearDeath = Fitness(Run(GameOutcome.Death, survivors: 0, partyHealth: 0, miles: 300));

            Assert.True(farDeath > nearDeath);
        }

        [Fact]
        public void AllAliveWin_IsWorthFarMoreThanAProportionateShare_OfALoneSurvivorWin()
        {
            // Super-linear reward: a full-party win must dominate a lone-survivor win by a wide margin, so the rare all-alive
            // finish beats a reliable low-survivor rush on expected value (the whole point of the reshape). Both travelled the
            // same distance, so the gap is pure survival/score - it must still be several times larger.
            var allFive = Fitness(Run(GameOutcome.Win, survivors: 5, partyHealth: 500, miles: 2040, score: 7650));
            var loneSurvivor = Fitness(Run(GameOutcome.Win, survivors: 1, partyHealth: 500, miles: 2040, score: 1650));

            Assert.True(allFive > loneSurvivor * 4);
        }

        [Fact]
        public void SacrificingMembers_IsPenalised_AmongOtherwiseEqualFinishers()
        {
            // Two finishers that travelled the same distance with equally healthy survivors; the one that lost more of its
            // party must score lower. This closes the "starve the party so survivors have more food" exploit.
            var keptFour = Fitness(Run(GameOutcome.Win, survivors: 4, partyHealth: 500, miles: 2040, score: 6150));
            var keptOne = Fitness(Run(GameOutcome.Win, survivors: 1, partyHealth: 500, miles: 2040, score: 1650));

            Assert.True(keptFour > keptOne);
        }

        [Fact]
        public void EachDeathFromAFinishingParty_IsHeavilyPenalised()
        {
            // Holding the game score constant to isolate the survival shaping: losing even one member from a party that
            // reaches Oregon must cost a large amount (super-linear reward drop + the heavy per-death penalty), so the
            // optimizer is pushed hard to bring the WHOLE party through rather than accept losses.
            var allAlive = Fitness(Run(GameOutcome.Win, survivors: 5, partyHealth: 500, miles: 2040, score: 5000));
            var lostOne = Fitness(Run(GameOutcome.Win, survivors: 4, partyHealth: 500, miles: 2040, score: 5000));

            Assert.True(allAlive - lostOne > 1000);
        }

        [Fact]
        public void FullDeath_IsNegative_ButStillFurtherRanksHigher()
        {
            // A wiped party is a net-negative outcome (the death penalty bites), yet distance still separates two wipeouts so
            // the optimizer keeps a gradient in the all-dying early generations.
            var farWipe = Fitness(Run(GameOutcome.Death, survivors: 0, partyHealth: 0, miles: 1500));
            var nearWipe = Fitness(Run(GameOutcome.Death, survivors: 0, partyHealth: 0, miles: 300));

            Assert.True(farWipe < 0);
            Assert.True(farWipe > nearWipe);
        }

        private static double Fitness(RunResult result) => TrainingSession.Fitness(result);
    }
}
