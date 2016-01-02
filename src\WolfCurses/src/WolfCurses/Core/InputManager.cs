// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@2:38 PM

namespace SimUnit
{
    using System.Collections.Generic;

    /// <summary>
    ///     Deals with keep track of input to the simulation via whatever form that may end up taking. The default
    ///     implementation is a text user interface (TUI) which allows for the currently accepted commands to be seen and only
    ///     then accepted.
    /// </summary>
    public sealed class InputManager : Module
    {
        /// <summary>
        ///     Holds a constant representation of the string telling the user to press enter key to continue so we don't repeat
        ///     ourselves.
        /// </summary>
        public const string PRESSENTER = "Press ENTER KEY to continue";

        /// <summary>
        ///     Reference to simulation that is controlling the input manager.
        /// </summary>
        private readonly SimulationApp _simUnit;

        /// <summary>
        ///     Holds a series of commands that need to be executed in the order they come out of the collection.
        /// </summary>
        private Queue<string> _commandQueue;

        /// <summary>
        ///     Initializes a new instance of the <see cref="InputManager" /> class.
        /// </summary>
        /// <param name="simUnit">Core simulation which is controlling the window manager.</param>
        public InputManager(SimulationApp simUnit)
        {
            _simUnit = simUnit;
            _commandQueue = new Queue<string>();
            InputBuffer = string.Empty;
        }

        /// <summary>
        ///     Input buffer that we will use to hold characters until need to send them to simulation.
        /// </summary>
        public string InputBuffer { get; private set; }

        /// <summary>
        ///     Fired when the simulation is closing and needs to clear out any data structures that it created so the program can
        ///     exit cleanly.
        /// </summary>
        public override void Destroy()
        {
            // Clear the input buffer.
            InputBuffer = string.Empty;

            // Clear the command queue.
            _commandQueue.Clear();
            _commandQueue = null;
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
        public override void OnTick(bool systemTick, bool skipDay = false)
        {
            // Skip if there are no commands to tick.
            if (_commandQueue.Count <= 0)
                return;

            // Dequeue the next command to send and pass along to currently active game Windows if it exists.
            _simUnit.WindowManager.FocusedWindow?.SendCommand(_commandQueue.Dequeue());
        }

        /// <summary>
        ///     Clears the input buffer and submits whatever was in there to the simulation for processing. Implementation is left
        ///     up the game simulation itself entirely.
        /// </summary>
        public void SendInputBufferAsCommand()
        {
            // Trim the result of the input so no extra whitespace at front or end exists.
            var lineBufferTrimmed = InputBuffer.Trim();

            // Destroy the input buffer if we are not accepting commands but return is pressed anyway.
            if (!_simUnit.WindowManager.AcceptingInput)
                InputBuffer = string.Empty;

            // Send trimmed line buffer to game simulation, if not accepting input we just pass along empty string.
            AddCommandToQueue(lineBufferTrimmed);

            // Always forcefully clear the input buffer after returning it, this makes it ready for more input.
            InputBuffer = string.Empty;
        }

        /// <summary>
        ///     Fired when the simulation receives an individual character from then input system. Depending on what it is we will
        ///     do something, or not!
        /// </summary>
        /// <param name="addedKeyString">String character converted into a string representation of itself.</param>
        private void OnCharacterAddedToInputBuffer(string addedKeyString)
        {
            // Disable passing along input buffer if the simulation is not currently accepting input from the user.
            if (!_simUnit.WindowManager.AcceptingInput)
                return;

            // Add the character to the end of the input buffer.
            InputBuffer += addedKeyString;
        }

        /// <summary>
        ///     Populates an internal input buffer for the simulation that is used to eventually return a possible command string
        ///     to active game Windows.
        /// </summary>
        /// <param name="keyChar">The key Char.</param>
        public void AddCharToInputBuffer(char keyChar)
        {
            // Filter to prevent non-characters like delete, insert, scroll lock, etc.
            if (!char.IsLetter(keyChar) && !char.IsNumber(keyChar))
                return;

            // Convert character to string representation if itself.
            var addedKeyString = char.ToString(keyChar);
            OnCharacterAddedToInputBuffer(addedKeyString);
        }

        /// <summary>
        ///     Removes the last character from input buffer if greater than zero.
        /// </summary>
        public void RemoveLastCharOfInputBuffer()
        {
            if (InputBuffer.Length > 0)
                InputBuffer = InputBuffer.Remove(InputBuffer.Length - 1);
        }

        /// <summary>
        ///     Fired by messaging system or user interface that wants to interact with the simulation by sending string command
        ///     that should be able to be parsed into a valid command that can be run on the current game Windows.
        /// </summary>
        /// <param name="returnedLine">Passed in command from controller, text was trimmed but nothing more.</param>
        private void AddCommandToQueue(string returnedLine)
        {
            // Trim the input.
            var trimmedInput = returnedLine.Trim();

            // Skip if we already entered the same command, simulation is state based... no need for flooding.
            if (_commandQueue.Contains(trimmedInput))
                return;

            // Adds the command to queue to be passed to simulation when input manager is ticked.
            _commandQueue.Enqueue(trimmedInput);
        }

        /// <summary>
        ///     Removes any text data from the input buffer resetting it to an empty string again.
        /// </summary>
        public void ClearBuffer()
        {
            InputBuffer = string.Empty;
        }
    }
}