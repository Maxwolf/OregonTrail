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
            Console.WriteLine("Starting...");
            Console.CursorVisible = false;
            Console.CancelKeyPress += Console_CancelKeyPress;

            // Create game simulation singleton instance, and start it.
            GameSimulationApp.Create();

            // Input buffer that we will use to hold characters until need to send them to simulation.
            var returnedLine = string.Empty;

            // Prevent console session from closing.
            while (GameSimulationApp.Instance != null)
            {
                // Clear the screen and set cursor to upper-left corner.
                Console.Clear();
                Console.SetCursorPosition(0, 0);

                // Check if a key is being pressed, without blocking thread.
                if (Console.KeyAvailable)
                {
                    // Get the key that was pressed, without printing it to console.
                    var key = Console.ReadKey(true);

                    // If enter is pressed, pass whatever we have to simulation.
                    switch (key.Key)
                    {
                        case ConsoleKey.Enter:
                            // Only process key press if something is there.
                            var returnedLineTrimmed = returnedLine.Trim();
                            if (!string.IsNullOrEmpty(returnedLineTrimmed))
                            {
                                // Clear line return for next line.
                                GameSimulationApp.Instance.SendCommand(returnedLineTrimmed);
                                returnedLine = string.Empty;
                            }
                            break;
                        case ConsoleKey.Backspace:
                            // Remove the last character from input buffer if greater than zero.
                            if (returnedLine.Length > 0)
                                returnedLine = returnedLine.Remove(returnedLine.Length - 1);
                            break;
                        default:
                            // Filter to prevent non-characters like delete, insert, scroll lock, etc.
                            if (char.IsLetter(key.KeyChar) || char.IsNumber(key.KeyChar))
                                returnedLine += char.ToString(key.KeyChar);
                            break;
                    }
                }

                // Write all text from objects to screen, prefix the string with return character so cursor resets to upper-left corner.
                Console.Title = "Oregon Trail Clone - [" + GameSimulationApp.Instance.TickPhase + "]";
                Console.Write("\r{0}", GameSimulationApp.Instance.GetTUI());
                Thread.Sleep(1);
            }

            // Make user press any key to close out the simulation completely, this way they know it closed without error.
            Console.Clear();
            Console.WriteLine("Goodbye...");
            Console.ReadLine();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            // Destroy the simulation.
            GameSimulationApp.Instance.Destroy();

            // Stop the operating system from killing the entire process.
            e.Cancel = true;

            // Clear the screen, set cursor to upper-left corner.
            Console.Clear();
            Console.SetCursorPosition(0, 0);
        }
    }
}