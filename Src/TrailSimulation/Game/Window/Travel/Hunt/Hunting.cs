// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/25/2015@7:56 PM

namespace TrailSimulation.Game
{
    using System;
    using System.Text;
    using Core;

    /// <summary>
    ///     Used to allow the players party to hunt for wild animals, shooting bullet items into the animals will successfully
    ///     kill them and when the round is over the amount of meat is determined by what animals are killed. The player party
    ///     can only take back up to one hundred pounds of whatever the value was back to the wagon regardless of what it was.
    /// </summary>
    [ParentWindow(GameWindow.Travel)]
    public sealed class Hunting : Form<TravelInfo>
    {
        /// <summary>
        ///     Representation of hunting form for text user interface.
        /// </summary>
        private StringBuilder _huntPrompt;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Hunting" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        public Hunting(IWindow window) : base(window)
        {
            // Creates the text reference to hold string data.
            _huntPrompt = new StringBuilder();
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public override bool InputFillsBuffer
        {
            get { return UserData.Hunt.PreyAvailable; }
        }

        /// <summary>
        ///     Determines if this dialog state is allowed to receive any input at all, even empty line returns. This is useful for
        ///     preventing the player from leaving a particular dialog until you are ready or finished processing some data.
        /// </summary>
        public override bool AllowInput
        {
            get { return UserData.Hunt.PreyAvailable; }
        }

        /// <summary>
        ///     Called when the simulation is ticked by underlying operating system, game engine, or potato. Each of these system
        ///     ticks is called at unpredictable rates, however if not a system tick that means the simulation has processed enough
        ///     of them to fire off event for fixed interval that is set in the core simulation by constant in milliseconds.
        /// </summary>
        /// <remarks>Default is one second or 1000ms.</remarks>
        /// <param name="systemTick">
        ///     TRUE if ticked unpredictably by underlying operating system, game engine, or potato. FALSE if
        ///     pulsed by game simulation at fixed interval.
        /// </param>
        /// <param name="skipDay">
        ///     Determines if the simulation has force ticked without advancing time or down the trail. Used by
        ///     special events that want to simulate passage of time without actually any actual time moving by.
        /// </param>
        public override void OnTick(bool systemTick, bool skipDay)
        {
            base.OnTick(systemTick, skipDay);

            // Pass the tick to the hunting manager that will control the hunting session for us and keep all data for other forms.
            UserData.Hunt?.OnTick(systemTick, skipDay);
        }

        /// <summary>
        ///     Returns a text only representation of the current game Windows state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        /// <returns>
        ///     The text user interface.<see cref="string" />.
        /// </returns>
        public override string OnRenderForm()
        {
            return UserData.Hunt.HuntInfo;
        }

        /// <summary>Fired when the game Windows current state is not null and input buffer does not match any known command.</summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Check if we have a hunting word right now.
            if (UserData.Hunt.ShootingWord == HuntWord.None)
                return;

            // Skip if the input is null or empty.
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
                return;

            // Attempt to cast string to enum value, can be characters or integer.
            HuntWord huntWord;
            Enum.TryParse(input, out huntWord);

            // Check if the user spelled the shooting word correctly.
            if (input.Equals(huntWord.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
            }
        }
    }
}