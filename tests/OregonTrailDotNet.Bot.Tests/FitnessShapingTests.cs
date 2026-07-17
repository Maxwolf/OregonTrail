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
        private static RunResult Run(GameOutcomeEnum outcome, int survivors, int partyHealth, int miles, int score = 0,
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
            var aliveButShort = Fitness(Run(GameOutcomeEnum.Timeout, survivors: 4, partyHealth: 400, miles: 900, score: 4800));
            var deadButFar = Fitness(Run(GameOutcomeEnum.Death, survivors: 0, partyHealth: 0, miles: 2000));

            Assert.True(aliveButShort > deadButFar);
        }

        [Fact]
        public void HealthierParty_ScoresHigherThanBarelyAliveParty()
        {
            // Same survivor count and distance; the only difference is health. Arriving healthy must win.
            var healthy = Fitness(Run(GameOutcomeEnum.Timeout, survivors: 5, partyHealth: 500, miles: 1000, score: 7500));
            var barelyAlive = Fitness(Run(GameOutcomeEnum.Timeout, survivors: 5, partyHealth: 200, miles: 1000, score: 3000));

            Assert.True(healthy > barelyAlive);
        }

        [Fact]
        public void MoreSurvivors_ScoresHigher_AtEqualHealth()
        {
            var fivePeople = Fitness(Run(GameOutcomeEnum.Timeout, survivors: 5, partyHealth: 400, miles: 1000, score: 6000));
            var onePerson = Fitness(Run(GameOutcomeEnum.Timeout, survivors: 1, partyHealth: 400, miles: 1000, score: 1200));

            Assert.True(fivePeople > onePerson);
        }

        [Fact]
        public void FinishingDominates_EvenAgainstAHealthyTimeout()
        {
            var win = Fitness(Run(GameOutcomeEnum.Win, survivors: 5, partyHealth: 500, miles: 2040, score: 7650));
            var timeout = Fitness(Run(GameOutcomeEnum.Timeout, survivors: 5, partyHealth: 500, miles: 1500, score: 7500));

            Assert.True(win > timeout);
        }

        [Fact]
        public void AmongDeaths_FurtherStillRanksHigher()
        {
            // With everyone dead the survival term is zero for both, so distance remains a gentle tie-breaker - the optimizer
            // still gets a gradient out of the all-dying early generations rather than a flat zero everywhere.
            var farDeath = Fitness(Run(GameOutcomeEnum.Death, survivors: 0, partyHealth: 0, miles: 1500));
            var nearDeath = Fitness(Run(GameOutcomeEnum.Death, survivors: 0, partyHealth: 0, miles: 300));

            Assert.True(farDeath > nearDeath);
        }

        [Fact]
        public void AllAliveWin_IsWorthFarMoreThan_ALoneSurvivorWin()
        {
            // Super-linear reward: a full-party win must dominate a lone-survivor win by a wide margin. (The old 4x ratio is
            // no longer attainable: every win now carries the flat dominance bonus that floors it above every non-win, so
            // the survival gap is expressed within the win band rather than by driving bad wins toward zero.)
            var allFive = Fitness(Run(GameOutcomeEnum.Win, survivors: 5, partyHealth: 500, miles: 2040, score: 7650));
            var loneSurvivor = Fitness(Run(GameOutcomeEnum.Win, survivors: 1, partyHealth: 500, miles: 2040, score: 1650));

            Assert.True(allFive > loneSurvivor * 2);
        }

        [Fact]
        public void HighScoreWin_OutValuesACheapWin_ByAtLeastTwiceTheScoreGap()
        {
            // Both parties finish with five healthy members; one arrives as a Farmer with supplies to spare (Meek's 7650),
            // the other ground out the cheapest tally that still counts as a win (2550). The score term is amplified so
            // chasing the great score pays even at a somewhat lower win rate — without it, training settled on reliable
            // ~2700-point finishes and never pushed higher.
            var meekGrade = Fitness(Run(GameOutcomeEnum.Win, survivors: 5, partyHealth: 500, miles: 2040, score: 7650));
            var cheapWin = Fitness(Run(GameOutcomeEnum.Win, survivors: 5, partyHealth: 500, miles: 2040, score: 2550));

            Assert.True(meekGrade - cheapWin >= 2 * (7650 - 2550));
        }

        [Fact]
        public void WorstWin_StillBeats_TheBestPossibleNonWin()
        {
            // The whole point of the finish bonus: no non-win outcome - not even a full healthy party that stalled out the
            // clock at Oregon's doorstep - may ever outrank an actual finish, or the optimizer climbs toward stalling.
            var worstWin = Fitness(Run(GameOutcomeEnum.Win, survivors: 1, partyHealth: 200, miles: 2040, score: 300));
            var bestTimeout = Fitness(Run(GameOutcomeEnum.Timeout, survivors: 5, partyHealth: 500, miles: 2000, score: 7650));
            var bestStranding = Fitness(Run(GameOutcomeEnum.Death, survivors: 5, partyHealth: 500, miles: 2000));

            Assert.True(worstWin > bestTimeout);
            Assert.True(worstWin > bestStranding);
        }

        [Fact]
        public void FatParking_AtTheTrailhead_LosesTo_PushingDownTheTrail()
        {
            // The reward-hack observed in real training: a party that parks early and strands with five fat members must
            // NOT out-score a party that pushed deep into the trail keeping most people alive. The survival term scales
            // with progress off the finish line, so survival credit cannot be farmed by refusing to travel.
            var fatParker = Fitness(Run(GameOutcomeEnum.Death, survivors: 5, partyHealth: 500, miles: 300));
            var pusher = Fitness(Run(GameOutcomeEnum.Death, survivors: 4, partyHealth: 400, miles: 1400));

            Assert.True(pusher > fatParker);
        }

        [Fact]
        public void Timeout_EarnsNoFinishBonus_AndIgnoresItsPartialScore()
        {
            // Running out the 246-day clock is a failure, scored exactly like any other non-finish (survival + progress).
            // It used to share the win branch's finish bonus - a healthy party idling to day 246 out-scored every genuine
            // attempt, so the accessible fitness optimum was stalling, not winning.
            var timedOut = Fitness(Run(GameOutcomeEnum.Timeout, survivors: 4, partyHealth: 400, miles: 900, score: 9999));
            var sameButDead = Fitness(Run(GameOutcomeEnum.Death, survivors: 4, partyHealth: 400, miles: 900));

            Assert.Equal(sameButDead, timedOut);
        }

        [Fact]
        public void SacrificingMembers_IsPenalised_AmongOtherwiseEqualFinishers()
        {
            // Two finishers that travelled the same distance with equally healthy survivors; the one that lost more of its
            // party must score lower. This closes the "starve the party so survivors have more food" exploit.
            var keptFour = Fitness(Run(GameOutcomeEnum.Win, survivors: 4, partyHealth: 500, miles: 2040, score: 6150));
            var keptOne = Fitness(Run(GameOutcomeEnum.Win, survivors: 1, partyHealth: 500, miles: 2040, score: 1650));

            Assert.True(keptFour > keptOne);
        }

        [Fact]
        public void EachDeathFromAFinishingParty_IsHeavilyPenalised()
        {
            // Holding the game score constant to isolate the survival shaping: losing even one member from a party that
            // reaches Oregon must cost a large amount (super-linear reward drop + the heavy per-death penalty), so the
            // optimizer is pushed hard to bring the WHOLE party through rather than accept losses.
            var allAlive = Fitness(Run(GameOutcomeEnum.Win, survivors: 5, partyHealth: 500, miles: 2040, score: 5000));
            var lostOne = Fitness(Run(GameOutcomeEnum.Win, survivors: 4, partyHealth: 500, miles: 2040, score: 5000));

            Assert.True(allAlive - lostOne > 1000);
        }

        [Fact]
        public void FullDeath_IsNegative_ButStillFurtherRanksHigher()
        {
            // A wiped party is a net-negative outcome (the death penalty bites), yet distance still separates two wipeouts so
            // the optimizer keeps a gradient in the all-dying early generations.
            var farWipe = Fitness(Run(GameOutcomeEnum.Death, survivors: 0, partyHealth: 0, miles: 1500));
            var nearWipe = Fitness(Run(GameOutcomeEnum.Death, survivors: 0, partyHealth: 0, miles: 300));

            Assert.True(farWipe < 0);
            Assert.True(farWipe > nearWipe);
        }

        [Fact]
        public void StrandedButAlive_GetsSurvivalCredit_NotTheOldZero()
        {
            // A party that stranded/died at mile 900 but kept four healthy people alive must score far above a same-distance
            // full wipe. The survival term now applies off the finish line too, so the old "no credit unless you finish" cliff
            // - the main reason fitness used to be dominated by luck - is gone.
            var strandedAlive = Fitness(Run(GameOutcomeEnum.Death, survivors: 4, partyHealth: 400, miles: 900));
            var wipedOut = Fitness(Run(GameOutcomeEnum.Death, survivors: 0, partyHealth: 0, miles: 900));

            Assert.True(strandedAlive > wipedOut + 1000);
        }

        [Fact]
        public void Fitness_IsMonotonic_In_Survivors_And_In_Progress()
        {
            // More survivors is always better at equal health/distance (even among non-finishers)...
            var twoAlive = Fitness(Run(GameOutcomeEnum.Death, survivors: 2, partyHealth: 400, miles: 800));
            var fourAlive = Fitness(Run(GameOutcomeEnum.Death, survivors: 4, partyHealth: 400, miles: 800));
            Assert.True(fourAlive > twoAlive);

            // ...and more distance is always better at equal survival, so the optimizer keeps a gradient everywhere.
            var nearer = Fitness(Run(GameOutcomeEnum.Death, survivors: 2, partyHealth: 400, miles: 400));
            var farther = Fitness(Run(GameOutcomeEnum.Death, survivors: 2, partyHealth: 400, miles: 1200));
            Assert.True(farther > nearer);
        }

        private static double Fitness(RunResult result) => TrainingSession.Fitness(result);
    }
}
