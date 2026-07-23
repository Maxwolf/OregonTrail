using OregonTrailDotNet.Presentation;
using System.Text;
using OregonTrailDotNet.Presentation.Audio;
using WolfCurses.Graphics;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Minigames.Windows
{
    /// <summary>
    ///     Every trail-stop card in order, named.
    ///     <para>
    ///         The index-to-picture mapping is not a convention anyone chose — it is computed. The 1985 loader builds
    ///         its filename from the landmark index itself (<c>OREGON TRAIL.txt:300</c>:
    ///         <c>V$ = "L" + STR$(LM) + ".PCK"</c>), so picture <c>L4</c> <i>is</i> landmark 4, and the names below are
    ///         the <c>LM$</c> table read straight out of <c>VAR.BIN</c>. The DOS <c>p0</c>-<c>p17</c> set was checked
    ///         against the same names subject by subject; both ports draw the same eighteen scenes in the same order.
    ///     </para>
    /// </summary>
    [ParentWindow(typeof(MinigamesWindow))]
    public sealed class LandmarkSlideshowForm : WorkbenchSceneForm
    {
        private const int SecondsPerSlide = 3;

        /// <summary>
        ///     The <c>LM$</c> table from <c>legacy/source/VAR.BIN.txt</c>, in index order, with its own flag column
        ///     (1 = fort, 2 = river crossing, 0 = neither) — which doubles as a check on the artwork, since a card
        ///     flagged as a fort had better show one.
        /// </summary>
        private static readonly Stop[] Stops =
        [
            new(0, "Independence", "settlement", "00-independence"),
            new(1, "The Kansas River crossing", "river", "01-kansas-river"),
            new(2, "The Big Blue River crossing", "river", "02-big-blue-river"),
            new(3, "Fort Kearney", "fort", "03-fort-kearney"),
            new(4, "Chimney Rock", "landmark", "04-chimney-rock"),
            new(5, "Fort Laramie", "fort", "05-fort-laramie"),
            new(6, "Independence Rock", "landmark", "06-independence-rock"),
            new(7, "South Pass", "fork", "07-south-pass"),
            new(8, "Fort Bridger", "fort", "08-fort-bridger"),
            new(9, "The Green River crossing", "river", "09-green-river"),
            new(10, "Soda Springs", "landmark", "10-soda-springs"),
            new(11, "Fort Hall", "fort", "11-fort-hall"),
            new(12, "The Snake River crossing", "river", "12-snake-river"),
            new(13, "Fort Boise", "fort", "13-fort-boise"),
            new(14, "The Blue Mountains", "fork", "14-blue-mountains"),
            new(15, "Fort Walla Walla", "fort", "15-fort-walla-walla"),
            new(16, "The Dalles", "fork", "16-the-dalles"),
            new(17, "The Willamette Valley", "journey's end", "17-willamette-valley")
        ];

        private int _slide;
        private int _ticksOnSlide;
        private bool _running = true;

        /// <summary>Initializes a new instance of the <see cref="LandmarkSlideshowForm" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public LandmarkSlideshowForm(IWindow window) : base(window)
        {
        }

        /// <inheritdoc />
        protected override int ReservedRows => 9;

        /// <summary>
        ///     The card's own tune — the DOS port's score for the stop on screen.
        ///     <para>
        ///         This pairing is the original's, and it is computed rather than chosen: on arrival the 1985 loader
        ///         builds the score's filename out of the landmark index exactly as it builds the picture's
        ///         (<c>OREGON TRAIL.txt:1005</c>: <c>Z$ = "MS" + STR$(LM) + ".BIN"</c> against <c>:300</c>'s
        ///         <c>V$ = "L" + STR$(LM) + ".PCK"</c>), then plays it under the picture while the card is up. So
        ///         score <c>MS4</c> belongs to picture <c>L4</c> by construction, and the eighteen DOS songs sit in
        ///         the same order — the same eighteen melodies, re-registered for the PC speaker.
        ///     </para>
        /// </summary>
        protected override string? MusicCue => $"landmarks/{Stops[_slide].Slug}";

        /// <inheritdoc />
        protected override void Build()
        {
        }

        /// <inheritdoc />
        protected override void Advance()
        {
            if (!_running)
                return;

            _ticksOnSlide++;

            // A card holds until its tune has played out, which is what the original does — it puts the picture up,
            // starts the score and waits on the space bar. The fixed dwell is the floor, and it is the whole rule
            // when there is no music: a run of cards flicking past at three seconds each while eighteen sixteen-
            // second tunes restart over the top of one another is not a comparison of anything.
            if (!Music.Finished)
                return;

            if (_ticksOnSlide < TicksPerSecond * SecondsPerSlide)
                return;

            _ticksOnSlide = 0;
            _slide = (_slide + 1) % Stops.Length;
        }

        /// <inheritdoc />
        protected override void OnSectionKey(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.RightArrow:
                    Step(1);
                    return;
                case ConsoleKey.LeftArrow:
                    Step(-1);
                    return;
                case ConsoleKey.Spacebar:
                    _running = !_running;
                    _ticksOnSlide = 0;
                    Invalidate();
                    return;
                case ConsoleKey.Home:
                    _slide = 0;
                    _ticksOnSlide = 0;
                    Invalidate();
                    return;
            }
        }

        /// <inheritdoc />
        protected override string Compose()
        {
            var stop = Stops[_slide];
            var picture = Art.Load($"landmarks/p{stop.Index}.png");
            var source = $"DOS MCGA  p{stop.Index}.png";

            var text = new StringBuilder();
            text.AppendLine();
            text.AppendLine($"TRAIL STOPS — {_slide + 1} of {Stops.Length}   {source}   " +
                            $"{picture.Width}x{picture.Height}   {(_running ? "playing" : "paused")}");
            text.AppendLine(
                "The 1985 loader picks this card by index: V$ = \"L\" + STR$(LM) + \".PCK\"  (OREGON TRAIL.txt:300)");
            text.Append(AnsiImage.FromPixels(picture).ToAnsi(PictureOptions()));

            // The name goes under the picture, the way the cards are captioned in the game itself.
            text.AppendLine();
            text.AppendLine($"        {stop.Index,2}.  {stop.Name.ToUpperInvariant()}   ({stop.Kind})");
            text.Append(Footer("LEFT/RIGHT step   SPACE play/pause   HOME first"));
            return text.ToString();
        }

        private void Step(int delta)
        {
            _slide = (_slide + delta + Stops.Length) % Stops.Length;
            _ticksOnSlide = 0;
            _running = false;
            Invalidate();
        }

        /// <summary>One stop on the trail, as the original's own tables name it.</summary>
        /// <param name="Index">The <c>LM</c> landmark index, which is also the picture number.</param>
        /// <param name="Name">Name from the <c>LM$</c> table in <c>VAR.BIN</c>.</param>
        /// <param name="Kind">What the stop is, from that table's flag column plus the route table's forks.</param>
        /// <param name="Slug">
        ///     The stop's name as the extractors spell it — <c>04-chimney-rock</c>. It names the landmark's tune
        ///     (<c>music/landmarks/04-chimney-rock.json</c>), the one thing still keyed off it.
        /// </param>
        private sealed record Stop(int Index, string Name, string Kind, string Slug);
    }
}
