using System;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Facilitates the ability to control the entire simulation with the passes interface reference. Server simulation
    ///     keeps track of all currently loaded game modes and will only tick the top-most one so they can be stacked and clear
    ///     out until there are none.
    /// </summary>
    public abstract class GameMode : IMode
    {
        public const string GAMEMODE_DEFAULT_TUI = "[DEFAULT GAME MODE TEXT USER INTERFACE]";
        public const string GAMEMODE_EMPTY_TUI = "[NO GAME MODE ATTACHED]";

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameMode" /> class.
        /// </summary>
        protected GameMode(IGameSimulation game)
        {
            Game = game;
        }

        public virtual string GetTUI()
        {
            return GAMEMODE_DEFAULT_TUI;
        }

        public abstract ModeType Mode { get; }

        public IGameSimulation Game { get; }

        public virtual void TickMode()
        {
        }

        /// <summary>
        ///     Fired by messaging system or user interface that wants to interact with the simulation by sending string command
        ///     that should be able to be parsed into a valid command that can be run on the current game mode.
        /// </summary>
        /// <param name="returnedLine">Passed in command from controller, text was trimmed but nothing more.</param>
        public void SendCommand(string returnedLine)
        {
            if (!string.IsNullOrEmpty(returnedLine) ||
                !string.IsNullOrWhiteSpace(returnedLine))
            {
                OnReceiveCommand(returnedLine);
            }
        }

        /// <summary>
        ///     Fired by the currently ticking and active game mode in the simulation. Implementation is left entirely up to
        ///     concrete handlers for game mode.
        /// </summary>
        /// <param name="returnedLine">Passed in command from controller, was already checking if null, empty, or whitespace.</param>
        protected virtual void OnReceiveCommand(string returnedLine)
        {
            Console.WriteLine(returnedLine);
        }

        public virtual void OnModeRemoved()
        {
            // Move along, nothing to see here...
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return Mode.ToString();
        }
    }
}