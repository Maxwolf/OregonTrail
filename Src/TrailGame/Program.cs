using System;
using System.Threading;
using TrailEntities;

namespace TrailGame
{
    internal class Program
    {
        /// <summary>
        ///     Primary game simulation and control input system.
        /// </summary>
        private static GameSimulationApp _game;

        /// <summary>
        ///     Create game simulation server or client depending on what the user wants.
        /// </summary>
        private static void Main()
        {
            // Create console with title, no cursor, make ctrl-c act as input.
            Console.Title = "Oregon Trail Clone";
            Console.CursorVisible = false;
            Console.CancelKeyPress += Console_CancelKeyPress;

            // Create game simulation.
            _game = new GameSimulationApp();
            var returnedLine = string.Empty;

            // Prevent console session from closing.
            while (!_game.IsClosing)
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
                                _game.SendCommand(returnedLineTrimmed);
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
                Console.Write("\r{0}", _game);
                Thread.Sleep(1);
            }
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            // Destroy the simulation.
            _game.Destroy();

            // Stop the operating system from killing the entire process.
            e.Cancel = true;

            // Clear the screen, set cursor to upper-left corner.
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Goodbye...");
        }
    }
}