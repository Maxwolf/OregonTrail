using System.Collections.Generic;
using System.Reflection;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Item;
using OregonTrailDotNet.Entity.Location;
using OregonTrailDotNet.Entity.Person;
using OregonTrailDotNet.Module.Time;
using OregonTrailDotNet.Window.MainMenu;
using OregonTrailDotNet.Window.Travel;
using OregonTrailDotNet.Window.Travel.Store;
using Xunit;
using TravelWindow = OregonTrailDotNet.Window.Travel.Travel;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Regression coverage for the store overbuy crash: ammunition is sold in a minimum lot of 20 (a "box"), and the
    ///     SimItem copy constructor silently clamps a smaller requested quantity up to that minimum. The affordability quote
    ///     and checkout must both account for that so a broke player is never quoted a purchase that then gets charged for
    ///     the whole lot and crashes checkout.
    /// </summary>
    public sealed class StorePurchaseTests : SimulationTestBase
    {
        private static void StartWithBalance(int monies)
        {
            Game.SetStartInfo(new NewGameInfo
            {
                PlayerNames = new List<string> {"Alice"},
                PlayerProfession = ProfessionEnum.Farmer,
                StartingMonies = monies,
                StartingMonth = MonthEnum.April
            });
        }

        private static TravelInfo UserDataOf(TravelWindow window) =>
            (TravelInfo) window.GetType().BaseType!
                .GetProperty("UserData", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!
                .GetValue(window)!;

        /// <summary>
        ///     Moves the party out of Independence and onto the trail, so the store under test is a fort counter rather
        ///     than Matt's. It is the location's STATUS that tells the two apart, not its index - the trail comes back
        ///     round to Independence's index the moment the party departs.
        /// </summary>
        private static void LeaveIndependence()
        {
            Game.Trail.CurrentLocation.Status = LocationStatusEnum.Arrived;
        }

        [Fact]
        public void PurchaseItems_WhenBillExceedsBalance_DoesNotThrowAndDoesNotOverspend()
        {
            // $1 cannot cover a 20-round box of ammo (20 x $0.10 = $2.00 at the start of the trail).
            StartWithBalance(1);

            var store = new StoreGenerator();
            store.AddItem(Resources.Bullets, 1); // copy ctor clamps the quantity up to the minimum lot of 20

            var balanceBefore = Game.Vehicle.Balance;

            var thrown = Record.Exception(() => store.PurchaseItems());

            Assert.Null(thrown); // must not crash
            Assert.Equal(balanceBefore, Game.Vehicle.Balance); // Vehicle.Purchase refuses the unaffordable lot
            Assert.Equal(0, Game.Vehicle.Inventory[EntitiesEnum.Ammo].Quantity); // nothing was added
        }

        [Fact]
        public void StorePurchase_QuotesZero_AndRejectsBuy_ForAnUnaffordableAmmoBox()
        {
            // $1 buys only ten bullets' worth, short of the twenty-round minimum lot.
            StartWithBalance(1);

            var window = new TravelWindow(GameSimulationApp.Instance);
            var userData = UserDataOf(window);
            userData.Store.SelectedItem = Resources.Bullets;

            var form = new StorePurchase(window);
            form.OnFormPostCreate();

            // The quote must be honest: you cannot afford a single box, so it reads zero rather than "1".
            Assert.Contains("afford 0", form.OnRenderForm());

            var balanceBefore = Game.Vehicle.Balance;

            // Trying to buy one anyway is rejected without crashing or spending.
            var thrown = Record.Exception(() => form.OnInputBufferReturned("1"));

            Assert.Null(thrown);
            Assert.Equal(balanceBefore, Game.Vehicle.Balance);
            Assert.Equal(0, Game.Vehicle.Inventory[EntitiesEnum.Ammo].Quantity);
        }

        [Fact]
        public void StorePurchase_QuotesAffordableAmmo_WhenThePlayerHasMoney()
        {
            // With real money the box is affordable, so the quote is non-zero and buying deducts and stocks ammo.
            StartWithBalance(1000);

            var window = new TravelWindow(GameSimulationApp.Instance);
            var userData = UserDataOf(window);
            userData.Store.SelectedItem = Resources.Bullets;

            var form = new StorePurchase(window);
            form.OnFormPostCreate();

            Assert.DoesNotContain("afford 0 ", form.OnRenderForm());
        }

        [Fact]
        public void StorePurchase_ReopeningAPendingItem_QuotesFullCapacity()
        {
            // StoreGenerator.AddItem REPLACES a pending order for the same item, so re-opening the purchase screen must
            // quote against the full remaining capacity (and the money the old order reserved), not double-count the
            // pending quantity - otherwise raising a 7-yoke order to 9 reads "You can afford 2" and wipes the order.
            StartWithBalance(1600);

            var window = new TravelWindow(GameSimulationApp.Instance);
            var userData = UserDataOf(window);
            userData.Store.AddItem(Parts.Oxen, 14);
            userData.Store.SelectedItem = Parts.Oxen;

            var form = new StorePurchase(window);
            form.OnFormPostCreate();

            // Quoted in the unit Matt sells: yoke. The wagon's twenty-ox ceiling is ten yoke, but the original's
            // one-character field only ever accepted nine at a time, so nine is the quote.
            Assert.Contains("afford 9 yoke", form.OnRenderForm());

            // Nine yoke is eighteen oxen - the receipt keeps counting individual animals.
            form.OnInputBufferReturned("9");
            Assert.Equal(18, userData.Store.Transactions[EntitiesEnum.Animal].Quantity);
        }

        [Fact]
        public void StorePurchase_CannotBuyASingleOx()
        {
            // The bug this guards: oxen were sold one at a time, so a player could order 1, or 3, or any odd number.
            // Matt sells yokes - "There are 2 oxen in a yoke" - so every order is an even number of animals.
            StartWithBalance(1600);

            var window = new TravelWindow(GameSimulationApp.Instance);
            var userData = UserDataOf(window);
            userData.Store.SelectedItem = Parts.Oxen;

            var form = new StorePurchase(window);
            form.OnFormPostCreate();

            // The screen asks for yoke, not oxen, and quotes Matt's $40 a yoke.
            var screen = form.OnRenderForm();
            Assert.Contains("How many yoke do you want?", screen);
            Assert.Contains("2 oxen in a yoke", screen);
            Assert.Contains("$40.00 a yoke", screen);

            // "1" is one yoke: two animals. There is no answer that produces one ox.
            form.OnInputBufferReturned("1");
            Assert.Equal(2, userData.Store.Transactions[EntitiesEnum.Animal].Quantity);
        }

        [Fact]
        public void StorePurchase_AtAFort_SellsOxenSingly_NotByTheYoke()
        {
            // The two stores are different counters in the original. Matt's is the outfitter and sells oxen the way a
            // teamster does, by the yoke; a fort is a price list, and a party that has lost one animal replaces one
            // animal. So the yoke exists at Independence and nowhere else.
            StartWithBalance(1600);
            LeaveIndependence();

            var window = new TravelWindow(GameSimulationApp.Instance);
            var userData = UserDataOf(window);
            userData.Store.SelectedItem = Parts.Oxen;

            var form = new StorePurchase(window);
            form.OnFormPostCreate();

            var screen = form.OnRenderForm();
            Assert.Contains("How many oxen do you want?", screen);
            Assert.DoesNotContain("yoke", screen);

            // No advice and no running tab out on the trail either - Matt is the only shopkeeper in the game.
            Assert.DoesNotContain("I recommend", screen);
            Assert.DoesNotContain("Bill so far", screen);

            // And one really is one. A fort buys immediately rather than running a tab, so the animal lands in the
            // wagon and the receipt is flushed - which is the other half of what makes this a different counter.
            var oxenBefore = Game.Vehicle.Inventory[EntitiesEnum.Animal].Quantity;
            form.OnInputBufferReturned("1");
            Assert.Equal(oxenBefore + 1, Game.Vehicle.Inventory[EntitiesEnum.Animal].Quantity);
        }

        [Fact]
        public void StorePurchase_AtAFort_HasAWiderQuantityFieldThanMatts()
        {
            // Matt's fields are tight - one character for oxen, two for ammunition. The forts give three (four for
            // food), so the ninety-nine-box ceiling at Independence is not the ceiling for the whole game.
            StartWithBalance(1600);
            LeaveIndependence();

            var window = new TravelWindow(GameSimulationApp.Instance);
            var userData = UserDataOf(window);
            userData.Store.SelectedItem = Resources.Bullets;

            var form = new StorePurchase(window);
            form.OnFormPostCreate();

            // Ammunition is still sold by the box at a fort; only the field width changed. $1,600 at the marked-up
            // fort price buys well over 99 boxes, so a quote above 99 proves the wider field.
            var screen = form.OnRenderForm();
            Assert.Contains("boxes do you want?", screen);

            var ammoBefore = Game.Vehicle.Inventory[EntitiesEnum.Ammo].Quantity;
            form.OnInputBufferReturned("150");
            Assert.Equal(ammoBefore + 3000, Game.Vehicle.Inventory[EntitiesEnum.Ammo].Quantity);
        }

        [Fact]
        public void StorePurchase_AmmunitionIsSoldByTheTwentyRoundBox_CappedAtNinetyNine()
        {
            // The original's counter sold boxes ("I sell ammunition in boxes of 20 bullets. Each box costs $2.00.")
            // through a two-character field, so a single purchase topped out at 99 boxes - 1,980 bullets.
            StartWithBalance(1600);

            var window = new TravelWindow(GameSimulationApp.Instance);
            var userData = UserDataOf(window);
            userData.Store.SelectedItem = Resources.Bullets;

            var form = new StorePurchase(window);
            form.OnFormPostCreate();

            var screen = form.OnRenderForm();
            Assert.Contains("How many boxes do you want?", screen);
            Assert.Contains("boxes of 20", screen);

            // $1,600 would buy 800 boxes; the field allows 99.
            Assert.Contains("afford 99 boxes", screen);

            // Ten boxes is two hundred bullets - inventory keeps counting single rounds.
            form.OnInputBufferReturned("10");
            Assert.Equal(200, userData.Store.Transactions[EntitiesEnum.Ammo].Quantity);
        }

        [Fact]
        public void StorePurchase_AnOverLargeOrder_IsRefusedWithAReason_AndKeepsThePendingOrder()
        {
            // The bug this guards: any quantity above the limit silently dumped the player back to the store menu AND
            // reset their pending order for that item, with nothing on screen to say why. Fat-fingering a zero onto a
            // carefully chosen food order threw the order away.
            StartWithBalance(400);

            var window = new TravelWindow(GameSimulationApp.Instance);
            var userData = UserDataOf(window);
            userData.Store.AddItem(Resources.Food, 200);
            userData.Store.SelectedItem = Resources.Food;

            var form = new StorePurchase(window);
            form.OnFormPostCreate();

            // 5,000 lb fits the four-character field the original gave food, but $400 only buys 2,000.
            form.OnInputBufferReturned("5000");

            // Still on the purchase screen, told why, and the 200 lb order is untouched.
            Assert.Contains("cannot afford", form.OnRenderForm());
            Assert.Equal(200, userData.Store.Transactions[EntitiesEnum.Food].Quantity);

            // An answer too wide for the field is refused on its own terms, and still keeps the order.
            form.OnInputBufferReturned("999999");
            Assert.Contains("at a time", form.OnRenderForm());
            Assert.Equal(200, userData.Store.Transactions[EntitiesEnum.Food].Quantity);
        }

        [Fact]
        public void MissingImportantItems_RequiresAFullYokeOfTwoOxen()
        {
            // Matt's General Store must not let the party leave with fewer than 2 oxen (a $40 yoke) - the 1985 game's
            // forced minimum spend, which pins the farmer's best leftover cash at $360 and the score ceiling at 13,860.
            StartWithBalance(400);

            var store = new StoreGenerator();
            Assert.True(store.MissingImportantItems); // no oxen at all

            store.AddItem(Parts.Oxen, 1);
            Assert.True(store.MissingImportantItems); // a single ox cannot pull the wagon out of Independence

            store.AddItem(Parts.Oxen, 2);
            Assert.False(store.MissingImportantItems); // a full yoke satisfies the gate
        }
    }
}
