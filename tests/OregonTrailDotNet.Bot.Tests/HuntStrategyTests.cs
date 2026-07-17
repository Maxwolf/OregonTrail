using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Window.Travel.Hunt;
using Xunit;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>
    ///     Covers the "have I bagged enough?" rule the bot uses to stop a hunt early like a player would. The hunt costs a full
    ///     day whenever it ends, so leaving early never saves time — it only wastes bullets past the carry cap, or throws away
    ///     food below it. So the rule stops exactly at the carry cap: a strict win (same food, fewer bullets) that never cuts a
    ///     hungry party's haul short.
    /// </summary>
    public sealed class HuntStrategyTests
    {
        private static GameSnapshot Hunt(int bagged, int food = 50, int living = 5) => new()
        {
            HuntBagged = bagged,
            Food = food,
            LivingCount = living,
            PartySize = living
        };

        [Fact]
        public void NothingBaggedYet_KeepsHunting()
        {
            Assert.False(HuntStrategy.HasEnoughFood(Hunt(bagged: 0)));
        }

        [Fact]
        public void BelowTheCarryCap_KeepsHunting_SinceTheDayIsAlreadySpent()
        {
            // Even a solid haul below the cap is real food worth staying for — the day costs the same whether we leave now or
            // fire until dark, and stopping short would only mean re-hunting sooner.
            Assert.False(HuntStrategy.HasEnoughFood(Hunt(bagged: HuntManager.MAXFOOD - 1)));
        }

        [Fact]
        public void SmallHungryParty_StillKeepsHuntingBelowTheCap()
        {
            // The stop rule must never cut a hungry party's haul short regardless of party size. Stated against the carry
            // cap rather than a literal, so it keeps testing the rule rather than the cap's current value.
            Assert.False(HuntStrategy.HasEnoughFood(Hunt(bagged: HuntManager.MAXFOOD - 1, food: 20, living: 2)));
            Assert.False(HuntStrategy.HasEnoughFood(Hunt(bagged: HuntManager.MAXFOOD - 1, food: 20, living: 1)));
        }

        [Fact]
        public void ReachingTheCarryCap_StopsHunting()
        {
            Assert.True(HuntStrategy.HasEnoughFood(Hunt(bagged: HuntManager.MAXFOOD)));
        }

        [Fact]
        public void OverTheCarryCap_StopsHunting_SinceTheExtraIsDiscarded()
        {
            // A single big animal (a buffalo is 350-500 lb) fills the cap on its own; every shot after is wasted ammunition.
            Assert.True(HuntStrategy.HasEnoughFood(Hunt(bagged: HuntManager.MAXFOOD + 300)));
        }
    }
}
