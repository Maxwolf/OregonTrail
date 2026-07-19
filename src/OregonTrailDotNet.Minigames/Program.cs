using OregonTrailDotNet.Minigames.Audio;
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

            // Fail loudly and early rather than showing a screen full of magenta "missing texture" checkerboards.
            if (!Assets.Ready)
            {
                Console.WriteLine("Could not find the extracted DOS artwork, which every minigame is drawn with.");
                Console.WriteLine();
                Console.WriteLine("Looked for a 'legacy/art' folder (with 'dos/rgba' and 'dos/mcga' inside) walking up from:");
                Console.WriteLine("  " + AppContext.BaseDirectory);
                Console.WriteLine("  " + Environment.CurrentDirectory);
                Console.WriteLine($"Found: {Assets.ArtRoot ?? "(nothing)"}");
                Console.WriteLine();
                Console.WriteLine("The DOS sprites are cut by legacy/tools/dos_sprites.py; run that, then try again,");
                Console.WriteLine("or start this from inside the repository.");
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
