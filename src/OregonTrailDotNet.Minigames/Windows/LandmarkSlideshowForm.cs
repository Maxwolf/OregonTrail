using System.Text;
using OregonTrailDotNet.Minigames.Audio;
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
    public sealed class LandmarkSlideshowForm : SceneForm
    {
        private const int SecondsPerSlide = 3;

        /// <summary>
        ///     The <c>LM$</c> table from <c>legacy/source/VAR.BIN.txt</c>, in index order, with its own flag column
        ///     (1 = fort, 2 = river crossing, 0 = neither) — which doubles as a check on the artwork, since a card
        ///     flagged as a fort had better show one.
        /// </summary>
        private static readonly Stop[] Stops =
        [
            new(0, "Independence", "settlement", "l00-independence.png"),
            new(1, "The Kansas River crossing", "river", "l01-kansas-river.png"),
            new(2, "The Big Blue River crossing", "river", "l02-big-blue-river.png"),
            new(3, "Fort Kearney", "fort", "l03-fort-kearney.png"),
            new(4, "Chimney Rock", "landmark", "l04-chimney-rock.png"),
            new(5, "Fort Laramie", "fort", "l05-fort-laramie.png"),
            new(6, "Independence Rock", "landmark", "l06-independence-rock.png"),
            new(7, "South Pass", "fork", "l07-south-pass.png"),
            new(8, "Fort Bridger", "fort", "l08-fort-bridger.png"),
            new(9, "The Green River crossing", "river", "l09-green-river.png"),
            new(10, "Soda Springs", "landmark", "l10-soda-springs.png"),
            new(11, "Fort Hall", "fort", "l11-fort-hall.png"),
            new(12, "The Snake River crossing", "river", "l12-snake-river.png"),
            new(13, "Fort Boise", "fort", "l13-fort-boise.png"),
            new(14, "The Blue Mountains", "fork", "l14-blue-mountains.png"),
            new(15, "Fort Walla Walla", "fort", "l15-fort-walla-walla.png"),
            new(16, "The Dalles", "fork", "l16-the-dalles.png"),
            new(17, "The Willamette Valley", "journey's end", "l17-willamette-valley.png")
        ];

        private int _slide;
        private int _ticksOnSlide;
        private bool _running = true;
        private bool _dos = true;

        /// <summary>Initializes a new instance of the <see cref="LandmarkSlideshowForm" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public LandmarkSlideshowForm(IWindow window) : base(window)
        {
        }

        /// <inheritdoc />
        protected override int ReservedRows => 9;

        /// <summary>
        ///     The card's own tune, from whichever port's card is on screen.
        ///     <para>
        ///         This pairing is the original's, and it is computed rather than chosen: on arrival the 1985 loader
        ///         builds the score's filename out of the landmark index exactly as it builds the picture's
        ///         (<c>OREGON TRAIL.txt:1005</c>: <c>Z$ = "MS" + STR$(LM) + ".BIN"</c> against <c>:300</c>'s
        ///         <c>V$ = "L" + STR$(LM) + ".PCK"</c>), then plays it under the picture while the card is up. So
        ///         score <c>MS4</c> belongs to picture <c>L4</c> by construction, and the eighteen DOS songs sit in
        ///         the same order — the same eighteen melodies, re-registered for the PC speaker.
        ///     </para>
        ///     <para>
        ///         Following the <c>A</c> key means the comparison works on the ear as well as the eye: the same stop
        ///         switches between the 1985 card with its Apple II score and the 1990 redraw with its DOS one, which
        ///         is where you can hear the re-registration — mostly two octaves up, several stops also re-keyed.
        ///     </para>
        /// </summary>
        protected override string? MusicCue
        {
            get
            {
                var stop = Stops[_slide];
                return _dos ? $"dos/song-{stop.Slug}" : $"apple2/ms{stop.Slug}";
            }
        }

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
                case ConsoleKey.A:
                    // Flip between the two ports' art for the same stop, which is the quickest way to see how much
                    // the 1990 redraw actually changed. The 1985 set is optional, so this does nothing without it.
                    if (!Assets.HasApple2)
                        return;

                    _dos = !_dos;
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
            var picture = _dos
                ? Assets.Load($"dos/mcga/p{stop.Index}.png")
                : Assets.Load($"apple2/{stop.Apple2File}");

            var source = _dos ? $"DOS MCGA  p{stop.Index}.png" : $"Apple II  {stop.Apple2File}";

            var text = new StringBuilder();
            text.AppendLine();
            text.AppendLine($"TRAIL STOPS — {_slide + 1} of {Stops.Length}   {source}   " +
                            $"{picture.Width}x{picture.Height}   {(_running ? "playing" : "paused")}");
            text.AppendLine(_dos
                ? "The 1985 loader picks this card by index: V$ = \"L\" + STR$(LM) + \".PCK\"  (OREGON TRAIL.txt:300)"
                : "Named from the LM$ table in VAR.BIN; picture L<n> is landmark <n> by construction.");
            text.Append(AnsiImage.FromPixels(picture).ToAnsi(PictureOptions()));

            // The name goes under the picture, the way the cards are captioned in the game itself.
            text.AppendLine();
            text.AppendLine($"        {stop.Index,2}.  {stop.Name.ToUpperInvariant()}   ({stop.Kind})");
            text.Append(Footer(Assets.HasApple2
                ? "LEFT/RIGHT step   SPACE play/pause   A Apple II <-> DOS   HOME first"
                : "LEFT/RIGHT step   SPACE play/pause   HOME first   (no Apple II set to compare against)"));
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
        /// <param name="Apple2File">The extracted Apple II card for the same index.</param>
        private sealed record Stop(int Index, string Name, string Kind, string Apple2File)
        {
            /// <summary>
            ///     The stop's name as the extractors spell it — <c>04-chimney-rock</c>. Both music folders and the
            ///     Apple II art are named on this stem (<c>ms04-chimney-rock</c>, <c>song-04-chimney-rock</c>,
            ///     <c>l04-chimney-rock.png</c>), so it is taken off the picture rather than written out a third time
            ///     and left to drift.
            /// </summary>
            public string Slug => Apple2File[1..^".png".Length];
        }
    }
}
