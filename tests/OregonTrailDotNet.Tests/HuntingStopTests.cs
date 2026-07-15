using System.Reflection;
using OregonTrailDotNet.Window.Travel;
using OregonTrailDotNet.Window.Travel.Hunt;
using Xunit;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Covers the ability to leave a hunt early instead of being forced to wait out the whole session: typing a stop word
    ///     (or pressing Escape, which the game loop routes to <see cref="Hunting.StopHunting" />) jumps straight to the results
    ///     screen with whatever food was already bagged, while ordinary input keeps the hunt going.
    /// </summary>
    public class HuntingStopTests : SimulationTestBase
    {
        private static (Travel Window, Hunting Form) StartHunt()
        {
            var window = new Travel(GameSimulationApp.Instance);

            // The window's UserData (shared with its forms) is protected; reach it to seed a live hunt session.
            var userData = (TravelInfo) window.GetType().BaseType!
                .GetProperty("UserData", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!
                .GetValue(window)!;
            userData.GenerateHunt();

            var form = new Hunting(window);
            return (window, form);
        }

        [Fact]
        public void Hunting_AcceptsInputAtAnyTime_SoThePlayerCanAlwaysQuit()
        {
            var (_, form) = StartHunt();

            Assert.True(form.InputFillsBuffer);
            Assert.True(form.AllowInput);
        }

        [Theory]
        [InlineData("stop")]
        [InlineData("QUIT")]
        [InlineData("Done")]
        [InlineData("leave")]
        [InlineData("exit")]
        [InlineData("q")]
        public void TypingAStopWord_EndsTheHuntImmediately(string word)
        {
            var (window, form) = StartHunt();

            form.OnInputBufferReturned(word);

            Assert.IsType<HuntingResult>(window.CurrentForm);
        }

        [Fact]
        public void StopHunting_UsedByTheEscapeKey_EndsTheHunt()
        {
            var (window, form) = StartHunt();

            form.StopHunting();

            Assert.IsType<HuntingResult>(window.CurrentForm);
        }

        [Fact]
        public void EmptyInput_DoesNotEndTheHunt()
        {
            // An empty ENTER is what the headless bot submits between animals; it must never end the hunt.
            var (window, form) = StartHunt();

            form.OnInputBufferReturned("");

            Assert.Null(window.CurrentForm);
        }
    }
}
