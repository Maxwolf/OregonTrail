using System.Text.RegularExpressions;
using OregonTrailDotNet.Window.Travel.Hunt;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Guards the redesigned hunting screen's contract with the headless training bot: no matter how the human-facing
    ///     layout changes, while an animal is being aimed at the body must still carry the exact "Type the word '&lt;word&gt;'"
    ///     token that <c>ScreenRecognizer.TypeWordRx</c> scrapes to learn which word to type. If this breaks, the bot
    ///     silently stops shooting.
    /// </summary>
    public class HuntScreenTests : SimulationTestBase
    {
        // Mirrors bot/OregonTrailDotNet.Bot/Game/ScreenRecognizer.cs:38 exactly.
        private static readonly Regex TypeWordRx = new(@"Type the word '([A-Za-z]+)'");

        /// <summary>
        ///     Prey spawns and target selection are random, so keep starting fresh hunts and ticking each toward the clock
        ///     running out until one puts an animal in the player's sights. Returns NULL only if that never happened, which
        ///     across this many attempts is astronomically unlikely.
        /// </summary>
        private static HuntManager HuntWithTarget()
        {
            for (var attempt = 0; attempt < 500; attempt++)
            {
                var hunt = new HuntManager();
                for (var t = 0; t < HuntManager.HUNTINGTIME && !hunt.PreyAvailable; t++)
                    hunt.OnTick(false, false);

                if (hunt.PreyAvailable)
                    return hunt;
            }

            return null;
        }

        [Fact]
        public void HuntBody_WhileAiming_KeepsTheWordTokenTheBotScrapes()
        {
            var hunt = HuntWithTarget();
            Assert.NotNull(hunt);

            var screen = hunt!.HuntInfo;

            var match = TypeWordRx.Match(screen);
            Assert.True(match.Success, $"bot word token missing from hunt body:\n{screen}");
            Assert.Equal(hunt.ShootingWord.ToString(), match.Groups[1].Value, ignoreCase: true);
        }
    }
}
