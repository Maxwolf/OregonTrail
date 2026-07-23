using OregonTrailDotNet.Presentation;
using OregonTrailDotNet.Presentation.Audio;
using WolfCurses.Graphics;

namespace OregonTrailDotNet.Minigames
{
    /// <summary>
    ///     Entry point for the minigame workbench. Same shape as the game's own <c>Program.Main</c>: nothing here draws
    ///     frames or reads keys, because WolfCurses does both — the scene graph presents every changed frame to the
    ///     console itself, and the input manager drains the keyboard each tick and routes it to the focused form.
    /// </summary>
    internal static class Program
    {
        private static int Main(string[] args)
        {
            Console.Title = "Oregon Trail — Minigame Workbench";
            Console.CursorVisible = false;
            Console.CancelKeyPress += (_, e) =>
            {
                Music.Shutdown();
                MinigamesApp.Instance?.Destroy();
                e.Cancel = true;
            };

            // Fail loudly rather than showing a screen full of magenta "missing texture" checkerboards. The art is
            // embedded in OregonTrailDotNet.Assets, so this only trips on a broken build, not a misplaced folder.
            if (!Art.Ready)
            {
                Console.WriteLine("The embedded artwork is missing from this build of OregonTrailDotNet.Assets.");
                Console.WriteLine("This is a packaging problem, not something a file in the right place would fix.");
                Console.WriteLine();
                Console.WriteLine("Press ANY KEY to close...");
                Console.ReadKey(true);
                return 1;
            }

            // Overrule the terminal's own answer when asked to, so a picture can be compared across renderers.
            // (Creating the simulation below probes the terminal and picks the best one by itself.)
            if (args.Contains("--halfblock")) ImageRenderers.Default = new HalfBlockImageRenderer();
            else if (args.Contains("--sixel")) ImageRenderers.Default = new SixelImageRenderer();
            else if (args.Contains("--kitty")) ImageRenderers.Default = new KittyImageRenderer();

            MinigamesApp.Create();

            while (MinigamesApp.Instance != null)
            {
                MinigamesApp.Instance.OnTick(true);
                Thread.Sleep(1);
            }

            // Hands the sound device back. Without this a tune that is still playing carries on after the workbench
            // has gone, since waveOut plays from a buffer the driver owns and does not care that we have exited.
            Music.Shutdown();

            Console.Clear();
            Console.WriteLine("Workbench closed.");
            return 0;
        }
    }
}
