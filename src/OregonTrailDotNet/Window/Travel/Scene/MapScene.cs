using System;
using System.Collections.Generic;
using OregonTrailDotNet.Entity.Location;
using OregonTrailDotNet.Entity.Location.Point;
using OregonTrailDotNet.Presentation;
using WolfCurses.Graphics;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Window.Travel.Scene
{
    /// <summary>
    ///     The trail map: the DOS port's 640-wide map picture with the party's actual route plotted over it — the
    ///     finished polyline through every location reached (fork branches and all, since the visited list encodes
    ///     which road was taken) and the current leg growing out of the last stop. The graphical sibling of
    ///     <see cref="Command.LookAtMap" />, silent like the original's map screen.
    /// </summary>
    [ParentWindow(typeof(Travel))]
    public sealed class MapScene : SceneForm<TravelInfo>
    {
        private PixelBuffer _plotted;

        /// <summary>Initializes a new instance of the <see cref="MapScene" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public MapScene(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Which form shows the map. Both dispatch sites — the travel menu and the fork's "see the map" — route
        ///     through here.
        /// </summary>
        internal static Type FormType =>
            GameSimulationApp.PresentationEnabled ? typeof(MapScene) : typeof(Command.LookAtMap);

        /// <inheritdoc />
        protected override bool Animated => false;

        /// <inheritdoc />
        protected override bool DismissOnAnyKey => true;

        /// <summary>
        ///     Counted exactly, the map has no rows to spare: the two status lines the game prepends to every frame,
        ///     the newline ending the picture, and the two footer rows.
        /// </summary>
        protected override int ReservedRows => 5;

        /// <inheritdoc />
        protected override bool CenterPicture => false;

        /// <inheritdoc />
        protected override string FooterControlHints => "Press ENTER to continue";

        /// <inheritdoc />
        protected override void Build()
        {
            var trail = GameSimulationApp.Instance.Trail;

            // Every location reached so far, in trail order — the mutable list already contains any fork branch
            // that was spliced in, so the polyline bends the way this run actually went. Unresolvable names (the
            // debug trail) simply drop out of the line.
            var visited = new List<(int X, int Y)>();
            foreach (var location in trail.Locations)
            {
                if (location.Status == LocationStatusEnum.Unreached)
                    continue;

                var stop = OriginalTrail.ForLocation(location.Name);
                if (stop != null)
                    visited.Add((stop.MapX, stop.MapY));
            }

            // The leg in progress: from the current location toward the next, scaled by how much of the road is
            // behind the wagon. A branch's own road length overrides the leg the fork set on arrival, exactly as
            // LocationFork does when it splices the branch in.
            (int X, int Y)? legFrom = null, legTo = null;
            double progress = 0;
            var current = OriginalTrail.ForLocation(trail.CurrentLocation?.Name);
            var next = trail.NextLocation != null ? OriginalTrail.ForLocation(trail.NextLocation.Name) : null;
            if (current != null && next != null)
            {
                legFrom = (current.MapX, current.MapY);
                legTo = (next.MapX, next.MapY);

                var legTotal = trail.NextLocation.LegDistance > 0
                    ? trail.NextLocation.LegDistance
                    : trail.CurrentLocation.TotalDistance;
                if (legTotal > 0)
                    progress = 1.0 - (double) trail.DistanceToNextLocation / legTotal;
            }

            var map = Art.Load("map.png");
            _plotted = new PixelBuffer(map.Width, map.Height);
            _plotted.DrawImage(map, 0, 0);
            MapArt.PlotRoute(_plotted, visited, legFrom, legTo, progress);
        }

        /// <inheritdoc />
        protected override string Compose()
        {
            // Native 640 width, stretched vertically to the console's shape — squeezing to 320 first costs half
            // the horizontal detail (which is half the lettering), and pre-scaling to the cell grid blurs it; the
            // renderer resolves finer than one pixel per cell. All four traps are minigames.md's, solved once.
            int columns, rows;
            try
            {
                columns = Math.Max(1, Console.WindowWidth);
                rows = Math.Max(4, Console.WindowHeight - ReservedRows);
            }
            catch (Exception)
            {
                columns = 80;
                rows = 24 - ReservedRows;
            }

            var height = Math.Max(_plotted.Height, _plotted.Width * (rows * 2) / columns);
            var stretched = MapArt.FitTo(_plotted, _plotted.Width, height);

            return AnsiImage.FromPixels(stretched).ToAnsi(PictureOptions()) + Environment.NewLine +
                   Footer(string.Empty);
        }

        /// <inheritdoc />
        protected override void OnDismiss()
        {
            // Exit parity with LookAtMap: back to the menu, or back to the fork that sent us here.
            if (GameSimulationApp.Instance.Trail.CurrentLocation is ForkInRoad)
                SetForm(typeof(Dialog.LocationFork));
            else
                ClearForm();
        }
    }
}
