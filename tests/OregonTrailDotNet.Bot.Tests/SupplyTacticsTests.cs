using OregonTrailDotNet.Bot.Game;
using OregonTrailDotNet.Bot.Learning;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Window.Travel;
using Xunit;

namespace OregonTrailDotNet.Bot.Tests
{
    /// <summary>
    ///     Pins the shared restock/trade tactics: when a policy walks back into a fort store, and which emigrant trades it
    ///     accepts. These are the counters to the game's dominant failure mode (oxen bleed out, wagon strands), so the rules
    ///     are locked down here independent of any policy.
    /// </summary>
    public sealed class SupplyTacticsTests
    {
        private static GameSnapshot Snapshot(int oxen = 6, int food = 800, int clothing = 10, int cash = 100,
            int living = 5, EntitiesEnum? brokenPart = null, int wheels = 1, TradeOfferView? trade = null) =>
            new()
            {
                Oxen = oxen,
                Food = food,
                Clothing = clothing,
                Cash = cash,
                LivingCount = living,
                PartySize = 5,
                Wheels = wheels,
                BrokenPart = brokenPart,
                Trade = trade
            };

        [Fact]
        public void Desperate_Means_NoOxen_Or_UnrepairableBrokenPart()
        {
            Assert.True(SupplyTactics.IsDesperate(Snapshot(oxen: 0)));
            Assert.True(SupplyTactics.IsDesperate(Snapshot(brokenPart: EntitiesEnum.Wheel, wheels: 0)));

            // A broken part WITH a spare on hand is routine (the repair flow consumes the spare), and a healthy team is fine.
            Assert.False(SupplyTactics.IsDesperate(Snapshot(brokenPart: EntitiesEnum.Wheel, wheels: 1)));
            Assert.False(SupplyTactics.IsDesperate(Snapshot()));
        }

        [Fact]
        public void Stranded_Party_Accepts_Any_Payable_Oxen_Offer_And_Nothing_Else()
        {
            // Wants 20 clothes for 1 ox — robbery at store prices, but a stranded wagon pays it.
            var oxenOffer = new TradeOfferView(EntitiesEnum.Animal, 1, EntitiesEnum.Clothes, 20, CanPay: true);
            Assert.True(SupplyTactics.AcceptTrade(Snapshot(oxen: 0, clothing: 25, trade: oxenOffer), margin: 0));

            // A generous non-oxen offer does not fix the stranding — decline and keep browsing for the rescue.
            var foodOffer = new TradeOfferView(EntitiesEnum.Food, 200, EntitiesEnum.Ammo, 1, CanPay: true);
            Assert.False(SupplyTactics.AcceptTrade(Snapshot(oxen: 0, trade: foodOffer), margin: 0));

            // Unpayable offers are never accepted (the game would not show the Y/N prompt anyway).
            var unpayable = new TradeOfferView(EntitiesEnum.Animal, 1, EntitiesEnum.Medicine, 9, CanPay: false);
            Assert.False(SupplyTactics.AcceptTrade(Snapshot(oxen: 0, trade: unpayable), margin: 0));
        }

        [Fact]
        public void PartStranded_Party_Accepts_The_Part_It_Needs()
        {
            var wheelOffer = new TradeOfferView(EntitiesEnum.Wheel, 1, EntitiesEnum.Food, 100, CanPay: true);
            var state = Snapshot(brokenPart: EntitiesEnum.Wheel, wheels: 0, food: 500, trade: wheelOffer);
            Assert.True(SupplyTactics.AcceptTrade(state, margin: 0));
        }

        [Fact]
        public void PartStranded_Party_Never_Hands_Over_Its_Whole_Team_For_The_Part()
        {
            // Fixing a wheel by giving away all six oxen just re-strands the wagon on the spot — declined. Keeping even
            // one ox (slow but rolling) is acceptable.
            var wheelForWholeTeam = new TradeOfferView(EntitiesEnum.Wheel, 1, EntitiesEnum.Animal, 6, CanPay: true);
            Assert.False(SupplyTactics.AcceptTrade(
                Snapshot(oxen: 6, brokenPart: EntitiesEnum.Wheel, wheels: 0, trade: wheelForWholeTeam), margin: 0));

            var wheelForMostOfTeam = new TradeOfferView(EntitiesEnum.Wheel, 1, EntitiesEnum.Animal, 5, CanPay: true);
            Assert.True(SupplyTactics.AcceptTrade(
                Snapshot(oxen: 6, brokenPart: EntitiesEnum.Wheel, wheels: 0, trade: wheelForMostOfTeam), margin: 0));
        }

        [Fact]
        public void Healthy_Party_Trades_On_Value_At_Store_Prices()
        {
            // 2 oxen ($40) for 10 boxes of bullets ($20): +$20 of value — accepted at margin 0, declined at margin 30.
            var goodDeal = new TradeOfferView(EntitiesEnum.Animal, 2, EntitiesEnum.Ammo, 10, CanPay: true);
            Assert.True(SupplyTactics.AcceptTrade(Snapshot(trade: goodDeal), margin: 0));
            Assert.False(SupplyTactics.AcceptTrade(Snapshot(trade: goodDeal), margin: 30));

            // 1 box of bullets ($2) for 1 ox ($20): value-negative — declined.
            var badDeal = new TradeOfferView(EntitiesEnum.Ammo, 1, EntitiesEnum.Animal, 1, CanPay: true);
            Assert.False(SupplyTactics.AcceptTrade(Snapshot(oxen: 10, trade: badDeal), margin: 0));
        }

        [Fact]
        public void Survival_Guards_Block_Trades_That_Strip_The_Party()
        {
            // Selling 2 oxen from a team of 7 would drop below the 6-ox full-speed floor — declined despite +$160 of value.
            var oxenForFood = new TradeOfferView(EntitiesEnum.Food, 2000, EntitiesEnum.Animal, 2, CanPay: true);
            Assert.False(SupplyTactics.AcceptTrade(Snapshot(oxen: 7, trade: oxenForFood), margin: 0));

            // Same offer with oxen to spare is taken.
            Assert.True(SupplyTactics.AcceptTrade(Snapshot(oxen: 9, trade: oxenForFood), margin: 0));

            // Clothing never drops below 2 sets per living member (the hail-freeze guard)...
            var clothesDeal = new TradeOfferView(EntitiesEnum.Medicine, 3, EntitiesEnum.Clothes, 3, CanPay: true);
            Assert.False(SupplyTactics.AcceptTrade(Snapshot(clothing: 12, living: 5, trade: clothesDeal), margin: 0));

            // ...and the larder never drops below the 300 lb floor.
            var foodDeal = new TradeOfferView(EntitiesEnum.Medicine, 5, EntitiesEnum.Food, 300, CanPay: true);
            Assert.False(SupplyTactics.AcceptTrade(Snapshot(food: 500, trade: foodDeal), margin: 0));
        }

        [Fact]
        public void Restock_Triggers_On_Any_Gap_But_Not_When_Broke_Or_Fully_Stocked()
        {
            var genome = StrategyGenome.Default();
            int Target(EntitiesEnum item) => genome.TargetQuantity(item);

            // Oxen below the genome's target -> restock.
            Assert.True(SupplyTactics.ShouldRestock(Snapshot(oxen: 2), Target));

            // Broke -> no point walking into the store.
            Assert.False(SupplyTactics.ShouldRestock(Snapshot(oxen: 2, cash: 3), Target));

            // Everything at or above target -> no visit.
            var stocked = new GameSnapshot
            {
                Oxen = 20, Food = 2000, Clothing = 50, Medicine = 99, Ammo = 99,
                Wheels = 3, Axles = 3, Tongues = 3, Cash = 500, LivingCount = 5, PartySize = 5
            };
            Assert.False(SupplyTactics.ShouldRestock(stocked, Target));
        }

        [Fact]
        public void Stranded_GenomePolicy_Chooses_To_Trade_Until_Its_Budget_Runs_Out()
        {
            var policy = new GenomePolicy(StrategyGenome.Default(), "Tactician (bot)");
            var menu = new[] { TravelCommandsEnum.ContinueOnTrail, TravelCommandsEnum.AttemptToTrade, TravelCommandsEnum.HuntForFood };
            var stranded = Snapshot(oxen: 0);

            for (var i = 0; i < SupplyTactics.MaxDesperateTradeAttempts; i++)
                Assert.Equal(TravelCommandsEnum.AttemptToTrade, policy.ChooseTravel(stranded, menu));

            // Budget exhausted: fall through to the normal choice so the run can still end as a stranding.
            Assert.Equal(TravelCommandsEnum.ContinueOnTrail, policy.ChooseTravel(stranded, menu));

            // A rescue (oxen back) resets the budget for any later stranding.
            policy.ChooseTravel(Snapshot(oxen: 4), menu);
            Assert.Equal(TravelCommandsEnum.AttemptToTrade, policy.ChooseTravel(stranded, menu));
        }

        [Fact]
        public void Stranded_At_A_Fort_With_Cash_Goes_To_The_Store_Before_Gambling_On_Trades()
        {
            // The fort store is a guaranteed oxen seller; emigrant offers are random. A stranded party with money must
            // try the store FIRST — and must be allowed back in even if it already restocked at this fort earlier.
            var policy = new GenomePolicy(StrategyGenome.Default(), "Tactician (bot)");
            var fortMenu = new[]
            {
                TravelCommandsEnum.ContinueOnTrail, TravelCommandsEnum.BuySupplies, TravelCommandsEnum.AttemptToTrade
            };

            var genome = StrategyGenome.Default();
            var strandedAtFort = new GameSnapshot
            {
                Oxen = 0, Food = 100, Clothing = 10, Cash = 150, LivingCount = 5, PartySize = 5,
                LocationName = "Fort Hall", LocationIndex = 8,
                Pace = genome.DesiredPace, Ration = genome.DesiredRation
            };

            Assert.Equal(TravelCommandsEnum.BuySupplies, policy.ChooseTravel(strandedAtFort, fortMenu));

            // Store already tried this episode (e.g. couldn't afford an ox after all): fall back to trade browsing
            // rather than ping-ponging between the menu and the store.
            Assert.Equal(TravelCommandsEnum.AttemptToTrade, policy.ChooseTravel(strandedAtFort, fortMenu));
        }

        [Fact]
        public void ThinTeam_Browses_A_Few_Trades_Per_Stop_Then_Moves_On()
        {
            // Below the 6-ox full-speed floor with no store here: check a handful of emigrant offers (the margin rule
            // decides acceptance), then continue rather than farming the free-browse loop.
            var policy = new GenomePolicy(StrategyGenome.Default(), "Tactician (bot)");
            var menu = new[] { TravelCommandsEnum.ContinueOnTrail, TravelCommandsEnum.AttemptToTrade };

            var genome = StrategyGenome.Default();
            var thinTeam = new GameSnapshot
            {
                Oxen = 4, Food = 800, Clothing = 10, Cash = 100, LivingCount = 5, PartySize = 5,
                Wheels = 1, Axles = 1, Tongues = 1,
                LocationName = "Chimney Rock", LocationIndex = 4,
                Pace = genome.DesiredPace, Ration = genome.DesiredRation
            };

            for (var i = 0; i < 3; i++)
                Assert.Equal(TravelCommandsEnum.AttemptToTrade, policy.ChooseTravel(thinTeam, menu));
            Assert.Equal(TravelCommandsEnum.ContinueOnTrail, policy.ChooseTravel(thinTeam, menu));
        }

        [Fact]
        public void GenomePolicy_Restocks_Once_Per_Location()
        {
            var policy = new GenomePolicy(StrategyGenome.Default(), "Tactician (bot)");
            var menu = new[]
            {
                TravelCommandsEnum.ContinueOnTrail, TravelCommandsEnum.BuySupplies, TravelCommandsEnum.HuntForFood
            };

            // Down to 2 oxen at a fort: shop once, then move on rather than bouncing menu <-> store forever. Pace/ration
            // are pre-matched to the genome's desire so those choices don't shadow the restock decision.
            var genome = StrategyGenome.Default();
            var atFort = new GameSnapshot
            {
                Oxen = 2, Food = 800, Clothing = 10, Cash = 200, LivingCount = 5, PartySize = 5,
                LocationName = "Fort Laramie", LocationIndex = 5,
                Pace = genome.DesiredPace, Ration = genome.DesiredRation
            };

            Assert.Equal(TravelCommandsEnum.BuySupplies, policy.ChooseTravel(atFort, menu));
            Assert.Equal(TravelCommandsEnum.ContinueOnTrail, policy.ChooseTravel(atFort, menu));
        }
    }
}
