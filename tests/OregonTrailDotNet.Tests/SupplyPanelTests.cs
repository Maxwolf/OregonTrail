using OregonTrailDotNet.Window;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Guards the shared <see cref="SupplyPanel" /> that the check-supplies, trading, and death screens all render
    ///     through after those three screens' duplicated inventory tables were consolidated into one helper. The consolidation
    ///     has a single behavioral knob — <c>includeCash</c> — because the status and death screens list the party's remaining
    ///     money while the trading screen must not (cash is not part of what is being bartered). These tests pin that split so
    ///     a future edit cannot silently flip a call site and start advertising the party's cash at the trading post, or drop
    ///     the money readout from the status and death screens.
    /// </summary>
    public class SupplyPanelTests : SimulationTestBase
    {
        [Fact]
        public void Build_WithCash_ListsMoneyLeftAlongsideTheInventory()
        {
            var panel = SupplyPanel.Build(includeCash: true);

            Assert.Contains("money left", panel);     // the cash row the status/death screens want
            Assert.Contains("oxen", panel);           // a representative supply row is still present
            Assert.Contains("pounds of food", panel);
            Assert.Contains("Item Name", panel);      // the WolfCurses ToStringTable header
            Assert.Contains("Amount", panel);
        }

        [Fact]
        public void Build_WithoutCash_OmitsMoneyLeftButKeepsTheInventory()
        {
            var panel = SupplyPanel.Build(includeCash: false);

            Assert.DoesNotContain("money left", panel); // the trading screen never advertises cash
            Assert.Contains("oxen", panel);             // the actual supplies are still listed
            Assert.Contains("bullets", panel);
        }
    }
}
