using System.Linq;
using System.Text.RegularExpressions;
using OregonTrailDotNet.Entity.Location;
using OregonTrailDotNet.Window.Travel;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Pins the fort menu against the bug a player caught at Fort Hall — and against the deeper contract under
    ///     it: the printed number IS the command's enum value, which the bot's trained policies encode, so the
    ///     fort menu must print exactly 1-7 then 9 and 10 (HuntForFood's 8 deliberately absent) and in that order.
    ///     A "tidy" renumbering that closed the gap at 8 would render a perfectly ascending menu while silently
    ///     breaking every trained policy, so the exact numbers are the assertion, not just their order.
    /// </summary>
    public class TravelMenuOrderTests : SimulationTestBase
    {
        [Fact]
        public void AtAFort_TheMenu_PrintsTheEnumValues_InOrder()
        {
            Game.TakeTurn(false);
            var guard = 0;
            while (Game.Trail.CurrentLocation.Name != "Fort Kearney" && guard++ < 10)
                Game.Trail.ArriveAtNextLocation();

            var fort = Game.Trail.CurrentLocation;
            Assert.Equal(LocationStatusEnum.Arrived, fort.Status);
            Assert.True(fort.ShoppingAllowed);
            Assert.True(fort.ChattingAllowed);

            var window = new Travel(GameSimulationApp.Instance);
            window.OnWindowPostCreate();
            Assert.Null(window.CurrentForm);

            // Menu entries are "N. Description"; matching the number-dot-word shape anywhere in the block keeps
            // the parse independent of how the framework laid the menu out for the console it happens to be
            // attached to (a short console reflows nine items into columns).
            var menu = window.OnRenderWindow();
            var numbers = Regex.Matches(menu, @"(\d+)\.\s+[A-Za-z]")
                .Select(match => int.Parse(match.Groups[1].Value))
                .ToList();

            // The fingerprint: continue, supplies, map, pace, rations, rest, trade — then buy 9 and talk 10,
            // with hunting's 8 absent (no hunting inside a fort).
            Assert.Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 9, 10 }, numbers.OrderBy(n => n).ToArray());

            // And in a single-column layout (any normal console), the printed order is the ascending order —
            // the Fort Hall regression printed 7, 10, 9.
            var lineStarts = Regex.Matches(menu, @"^\s*(\d+)\.\s+[A-Za-z]", RegexOptions.Multiline)
                .Select(match => int.Parse(match.Groups[1].Value))
                .ToList();
            if (lineStarts.Count == numbers.Count)
                Assert.Equal(lineStarts.OrderBy(n => n).ToList(), lineStarts);
        }
    }
}
