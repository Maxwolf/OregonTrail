// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/01/2016@7:40 PM

using System;
using System.Linq;
using System.Threading;
using OregonTrailDotNet.Presentation.Audio;
using WolfCurses.Graphics;

namespace OregonTrailDotNet
{
    /// <summary>
    ///     Trail Simulation Game - Console Edition
    /// </summary>
    internal static class Program
    {
        /// <summary>
        ///     Example console app for game simulation entry point.
        /// </summary>
        /// <param name="args">Optional renderer override: --halfblock, --sixel, or --kitty.</param>
        public static int Main(string[] args)
        {
            // Create console with title, no cursor, make CTRL-C act as input.
            Console.Title = "Oregon Trail Clone";
            Console.WriteLine("Starting...");
            Console.CursorVisible = false;
            Console.CancelKeyPress += Console_CancelKeyPress;

            // The real game persists high scores and tombstones to game.db next to the executable (headless hosts like the
            // training bot leave this off and stay in-memory).
            GameSimulationApp.PersistenceEnabled = true;

            // The real game also gets the graphical scene forms (original MECC artwork and music at the dramatic moments);
            // headless hosts leave this off so the bot's text scraping and the test suite see the unchanged text forms.
            GameSimulationApp.PresentationEnabled = true;

            // Overrule the terminal's own answer when asked to, so a picture can be compared across renderers.
            // (Creating the simulation below probes the terminal and picks the best one by itself.) Same flags as
            // the minigame workbench.
            if (args.Contains("--halfblock")) ImageRenderers.Default = new HalfBlockImageRenderer();
            else if (args.Contains("--sixel")) ImageRenderers.Default = new SixelImageRenderer();
            else if (args.Contains("--kitty")) ImageRenderers.Default = new KittyImageRenderer();

            // Create game simulation singleton instance, and start it. Constructing it also probes the terminal for the
            // best graphics protocol it can draw with — once, and before any key is read — so it has to come before the
            // tick loop below.
            GameSimulationApp.Create();

            // Nothing here draws the screen or reads the keyboard, and both absences are deliberate. While no one is
            // subscribed to the scene graph's ScreenBufferDirtyEvent, WolfCurses presents every changed frame to this
            // console itself: flicker-free, only the rows that actually changed, one write per update — and it readies the
            // console for ANSI escapes and UTF-8 the first time it draws, which is why the old manual OutputEncoding line
            // is gone with it. Input is automatic in the same way: each tick the InputManager drains the console's key
            // buffer and routes it the standard way — ENTER submits the typed command, BACKSPACE edits it, and every other
            // key both fills the prompt and reaches the focused form. Escape leaving a hunt early now rides that last path
            // (see Hunting.OnKeyPressed) instead of being special-cased here.
            while (GameSimulationApp.Instance != null)
            {
                // Simulation takes any numbers of pulses to determine seconds elapsed.
                GameSimulationApp.Instance.OnTick(true);

                // Do not consume all of the CPU, allow other messages to occur.
                Thread.Sleep(1);
            }

            // The waveOut driver plays from its own buffer and would keep sounding after the loop ends; stop it before
            // the goodbye screen. Harmless when nothing ever played.
            Music.Shutdown();

            // Make user press any key to close out the simulation completely, this way they know it closed without error.
            Console.Clear();
            Console.WriteLine("Goodbye!");
            Console.WriteLine("Press ANY KEY to close this window...");
            Console.ReadKey();
            return 0;
        }

        /// <summary>
        ///     Fired when the user presses CTRL-C on their keyboard, this is only relevant to operating system tick and this view
        ///     of simulation. If moved into another framework like game engine this statement would be removed and just destroy
        ///     the simulation when the engine is destroyed using its overrides.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            // Silence the waveOut device first — its driver-owned buffer outlives the process's playback intent.
            Music.Shutdown();

            // Destroy the simulation, unless it is already gone (CTRL-C pressed at the goodbye prompt).
            GameSimulationApp.Instance?.Destroy();

            // Stop the operating system from killing the entire process.
            e.Cancel = true;
        }
    }
}