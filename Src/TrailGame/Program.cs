using System;
using System.Threading;
using TrailEntities;

namespace TrailGame
{
    internal class Program
    {
        /// <summary>
        ///     Create game simulation server or client depending on what the user wants.
        /// </summary>
        private static void Main()
        {
            // Create console with title, no cursor, make ctrl-c act as input.
            Console.Title = "Oregon Trail Clone";
            Console.WriteLine("Starting...");
            Console.CursorVisible = false;
            Console.CancelKeyPress += Console_CancelKeyPress;

            // Create game simulation singleton instance, and start it.
            GameSimulationApp.Create();

            // Input buffer that we will use to hold characters until need to send them to simulation.
            var lineBuffer = string.Empty;
            var screenBuffer = string.Empty;

            // Prevent console session from closing.
            while (GameSimulationApp.Instance != null)
            {
                // Check if a key is being pressed, without blocking thread.
                if (Console.KeyAvailable)
                {
                    // Get the key that was pressed, without printing it to console.
                    var key = Console.ReadKey(true);

                    // If enter is pressed, pass whatever we have to simulation.
                    // ReSharper disable once ConvertIfStatementToSwitchStatement
                    if (key.Key == ConsoleKey.Enter)
                    {
                        // Only process key press if something is there.
                        var lineBufferTrimmed = lineBuffer.Trim();
                        if (!string.IsNullOrEmpty(lineBufferTrimmed))
                        {
                            // Send trimmed line buffer to game simulation.
                            if (GameSimulationApp.Instance != null)
                                GameSimulationApp.Instance.SendCommand(lineBufferTrimmed);

                            // Clear line return for next line.
                            lineBuffer = string.Empty;
                        }
                    }
                    else if (key.Key == ConsoleKey.Backspace)
                    {
                        // Remove the last character from input buffer if greater than zero.
                        if (lineBuffer.Length > 0)
                            lineBuffer = lineBuffer.Remove(lineBuffer.Length - 1);
                    }
                    else
                    {
                        // Filter to prevent non-characters like delete, insert, scroll lock, etc.
                        if (char.IsLetter(key.KeyChar) || char.IsNumber(key.KeyChar))
                            lineBuffer += char.ToString(key.KeyChar);
                    }
                }

                // Write all text from objects to screen, prefix the string with return character so cursor resets to upper-left corner.
                if (GameSimulationApp.Instance != null)
                {
                    // Use screen buffer to only write to console when something actually changes to stop flickering from constant ticking.
                    var simulationTUI = GameSimulationApp.Instance.GetTUI();
                    if (!screenBuffer.Equals(simulationTUI, StringComparison.InvariantCultureIgnoreCase))
                    {
                        // Clear the screen and set cursor to upper-left corner.
                        Console.Clear();
                        Console.SetCursorPosition(0, 0);

                        screenBuffer = simulationTUI;
                        Console.Write("\r{0}", screenBuffer);
                    }
                }

                // Do not consume all of the CPU, allow other messages to occur.
                Thread.Sleep(1);
            }

            // Make user press any key to close out the simulation completely, this way they know it closed without error.
            Console.Clear();
            Console.WriteLine("Goodbye!");
            Console.WriteLine("Press ANY key to close this window...");
            Console.ReadLine();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            // Destroy the simulation.
            GameSimulationApp.Instance.Destroy();

            // Stop the operating system from killing the entire process.
            e.Cancel = true;
        }
    }
}