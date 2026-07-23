using OregonTrailDotNet.Presentation;
using OregonTrailDotNet.Window.MainMenu;
using OregonTrailDotNet.Window.MainMenu.Names;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Pins the two DOS picture banners: the title lettering over the main menu and the family over the
    ///     party-naming screens. With presentation off (every headless host — the bot reads the menu and types
    ///     names into these forms) the renders are the plain text they always were; with it on the picture rides
    ///     above the SAME text — banner + plain, byte for byte — except the menu, where the picture IS the title
    ///     and replaces the plain-text masthead. (Banner content is renderer-dependent — a headless host may draw
    ///     without ANSI — so frames are compared against the banner string, never against escape codes.)
    /// </summary>
    public class BannerGateTests : SimulationTestBase
    {
        public override void Dispose()
        {
            GameSimulationApp.PresentationEnabled = false;
            base.Dispose();
        }

        private static (InputPlayerNames Input, ConfirmPlayerNames Confirm) BuildNamingForms()
        {
            var window = new MainMenu(GameSimulationApp.Instance);
            var input = new InputPlayerNames(window);
            input.OnFormPostCreate();
            var confirm = new ConfirmPlayerNames(window);
            confirm.OnFormPostCreate();
            return (input, confirm);
        }

        [Theory]
        [InlineData("family.png", 320, 126)]
        [InlineData("banner.png", 320, 63)]
        public void TheBannerPictures_AreEmbedded_AndDecode(string key, int width, int height)
        {
            // A missing or undecodable asset comes back as the 320x200 missing-texture checkerboard; the real
            // strips are shorter, so the dimensions prove the embedded PNGs actually decoded.
            var picture = Art.Load(key);
            Assert.Equal(width, picture.Width);
            Assert.Equal(height, picture.Height);
        }

        [Fact]
        public void FlagOff_TheNamingScreens_ArePlainText()
        {
            var (input, confirm) = BuildNamingForms();

            var question = input.OnRenderForm();
            Assert.DoesNotContain('', question);
            Assert.Contains("wagon leader", question);

            var readback = confirm.OnRenderForm();
            Assert.DoesNotContain('', readback);
            Assert.Contains("Are these names correct", readback);
        }

        [Fact]
        public void FlagOn_TheFamily_RidesAboveTheUnchangedQuestions()
        {
            var (input, confirm) = BuildNamingForms();

            // The plain text first, then the same forms with the flag on: the picture must sit ABOVE an
            // unchanged question — banner + plain text, byte for byte — so the words the player answers (and
            // the bot scrapes) survive intact.
            var plainQuestion = input.OnRenderForm();
            var plainReadback = confirm.OnRenderForm();

            GameSimulationApp.PresentationEnabled = true;
            var banner = Banners.Family(reservedRows: 12);
            Assert.False(string.IsNullOrWhiteSpace(banner));

            Assert.Equal(banner + plainQuestion, input.OnRenderForm());
            Assert.Equal(banner + plainReadback, confirm.OnRenderForm());
        }

        [Fact]
        public void FlagOff_TheMainMenu_KeepsItsTextTitle()
        {
            var window = new MainMenu(GameSimulationApp.Instance);
            window.OnWindowPostCreate();

            var menu = window.OnRenderWindow();
            Assert.Contains("The Oregon Trail", menu);
            Assert.Contains("You may:", menu);
            Assert.Contains("Travel the trail", menu);
        }

        [Fact]
        public void FlagOn_TheTitleBanner_ReplacesTheTextTitle()
        {
            GameSimulationApp.PresentationEnabled = true;

            var window = new MainMenu(GameSimulationApp.Instance);
            window.OnWindowPostCreate();

            // The picture IS the title, so the plain-text masthead goes; the menu beneath survives whole.
            var menu = window.OnRenderWindow();
            var banner = Banners.Title(reservedRows: 12);
            Assert.False(string.IsNullOrWhiteSpace(banner));
            Assert.StartsWith(banner, menu);
            Assert.DoesNotContain("The Oregon Trail", menu);
            Assert.Contains("You may:", menu);
            Assert.Contains("Travel the trail", menu);
        }
    }
}
