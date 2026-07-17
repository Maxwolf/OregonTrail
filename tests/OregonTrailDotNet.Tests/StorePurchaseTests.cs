using System.Collections.Generic;
using System.Reflection;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Item;
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

        [Fact]
        public void PurchaseItems_WhenBillExceedsBalance_DoesNotThrowAndDoesNotOverspend()
        {
            // $8 cannot cover a 20-round box of ammo (20 x $2 = $40 at the start of the trail).
            StartWithBalance(8);

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
            StartWithBalance(8);

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
            // pending quantity — otherwise raising a 15-oxen order to 18 reads "You can afford 5" and wipes the order.
            StartWithBalance(1600);

            var window = new TravelWindow(GameSimulationApp.Instance);
            var userData = UserDataOf(window);
            userData.Store.AddItem(Parts.Oxen, 15);
            userData.Store.SelectedItem = Parts.Oxen;

            var form = new StorePurchase(window);
            form.OnFormPostCreate();

            Assert.Contains("afford 20 oxen", form.OnRenderForm());

            form.OnInputBufferReturned("18");
            Assert.Equal(18, userData.Store.Transactions[EntitiesEnum.Animal].Quantity);
        }

        [Fact]
        public void MissingImportantItems_RequiresAFullYokeOfTwoOxen()
        {
            // Matt's General Store must not let the party leave with fewer than 2 oxen (a $40 yoke) — the 1985 game's
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
