using OregonTrailDotNet.Presentation;
using OregonTrailDotNet.Window.MainMenu;
using OregonTrailDotNet.Window.MainMenu.Names;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Pins the family banner above the party-naming screens: with presentation off (every headless host — the
    ///     bot types real names into these forms) the render is the plain text it always was, no ANSI anywhere;
    ///     with it on the picture rides above the SAME text, so the questions the player answers — and the bot
    ///     scrapes — survive underneath unchanged.
    /// </summary>
    public class FamilyArtGateTests : SimulationTestBase
    {
        private const char Escape = '';

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

        [Fact]
        public void TheFamilyPicture_IsEmbedded_AndDecodes()
        {
            // A missing or undecodable asset comes back as the 320x200 missing-texture checkerboard; the real
            // family strip is 320x126, so the dimensions prove the embedded PNG actually decoded.
            var picture = Art.Load("family.png");
            Assert.Equal(320, picture.Width);
            Assert.Equal(126, picture.Height);
        }

        [Fact]
        public void FlagOff_TheNamingScreens_ArePlainText()
        {
            var (input, confirm) = BuildNamingForms();

            var question = input.OnRenderForm();
            Assert.DoesNotContain(Escape, question);
            Assert.Contains("wagon leader", question);

            var readback = confirm.OnRenderForm();
            Assert.DoesNotContain(Escape, readback);
            Assert.Contains("Are these names correct", readback);
        }

        [Fact]
        public void FlagOn_TheFamily_RidesAboveTheUnchangedQuestions()
        {
            var (input, confirm) = BuildNamingForms();

            // The plain text first, then the same forms with the flag on: the picture must sit ABOVE an
            // unchanged question — banner + plain text, byte for byte — so the words the player answers (and
            // the bot scrapes) survive intact. (The banner itself is renderer-dependent — a headless host may
            // draw it without ANSI — so the frame is compared against the banner, not against escape codes.)
            var plainQuestion = input.OnRenderForm();
            var plainReadback = confirm.OnRenderForm();

            GameSimulationApp.PresentationEnabled = true;
            var banner = FamilyArt.Banner(reservedRows: 12);
            Assert.False(string.IsNullOrWhiteSpace(banner));

            Assert.Equal(banner + plainQuestion, input.OnRenderForm());
            Assert.Equal(banner + plainReadback, confirm.OnRenderForm());
        }
    }
}
