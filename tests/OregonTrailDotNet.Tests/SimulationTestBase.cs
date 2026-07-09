using System;
using System.Reflection;

namespace OregonTrailDotNet.Tests
{
    /// <summary>
    ///     Boots the game simulation singleton before each test and destroys it afterwards. The first
    ///     simulation tick fires OnFirstTick which runs Restart(), creating all game modules (time,
    ///     trail, event director, vehicle) and attaching the travel and main menu windows, mirroring
    ///     exactly what Program.Main does on startup.
    /// </summary>
    public abstract class SimulationTestBase : IDisposable
    {
        static SimulationTestBase()
        {
            // WolfCurses discovers windows, forms, and random events by reflecting over the process
            // entry assembly, which inside a test host is the runner rather than the game. Point it
            // at the game assembly so the factories can see every game type.
            Assembly.SetEntryAssembly(typeof(GameSimulationApp).Assembly);
        }

        protected SimulationTestBase()
        {
            // A previously failed test could leave the singleton alive, clear it out first.
            GameSimulationApp.Instance?.Destroy();
            GameSimulationApp.Create();

            // The first tick creates the modules and windows, the second renders the attached
            // windows which is what builds their menu command mappings.
            Game.OnTick(false);
            Game.OnTick(false);
        }

        /// <summary>
        ///     Shorthand for the game simulation singleton instance under test.
        /// </summary>
        protected static GameSimulationApp Game => GameSimulationApp.Instance;

        public void Dispose()
        {
            GameSimulationApp.Instance?.Destroy();
        }

        /// <summary>
        ///     Types the given text into the input buffer and submits it exactly like the player
        ///     pressing ENTER, then ticks the simulation so the command gets dispatched to the
        ///     currently focused window.
        /// </summary>
        protected static void SendCommand(string command)
        {
            foreach (var keyChar in command)
                Game.InputManager.AddCharToInputBuffer(keyChar);

            Game.InputManager.SendInputBufferAsCommand();

            // First tick dispatches the queued command, second renders the resulting screen so the
            // next command finds its menu mappings in place.
            Game.OnTick(false);
            Game.OnTick(false);
        }
    }
}
