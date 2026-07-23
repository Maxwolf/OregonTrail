using System;
using System.Collections.Generic;
using OregonTrailDotNet.Entity;
using OregonTrailDotNet.Entity.Vehicle;
using OregonTrailDotNet.Module.Director;
using OregonTrailDotNet.Presentation;
using OregonTrailDotNet.Window.Travel.Command;
using OregonTrailDotNet.Window.Travel.Dialog;
using WolfCurses.Graphics;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Window.Travel.Scene
{
    /// <summary>
    ///     The travel screen: the original's one animated tableau, assembled exactly as the 1985 game assembled it —
    ///     black sky, horizon strip hanging above the ground box, the ox team walking in the black band between,
    ///     the wagon nailed to its pixel while the world slides past, and the roadside miniature of the next
    ///     landmark arriving level with the wagon as the leg runs out. The graphical sibling of
    ///     <see cref="ContinueOnTrail" />: the simulation underneath is the identical <see cref="DriveTick" />, one
    ///     day per simulation tick, and the animation eases the world toward the simulation's real distance between
    ///     day boundaries so the legs stride with ground covered rather than with the clock.
    /// </summary>
    [ParentWindow(typeof(Travel))]
    public sealed class DriveScene : SceneForm<TravelInfo>
    {
        private PixelBuffer _backdrop;
        private PixelBuffer _sceneryArt;
        private string _destinationName;
        private TravelTerrainEnum _terrain;
        private TravelWeatherEnum _weather;
        private double _pixelsPerMile;
        private int _sceneryRestX;
        private double _displayMiles;
        private int _sceneryX;
        private int _walkFrame = 1;
        private int _seenEventTurn = -1;
        private EventIconEnum? _groundIcon;
        private double _groundIconX;

        /// <summary>Initializes a new instance of the <see cref="DriveScene" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public DriveScene(IWindow window) : base(window)
        {
        }

        /// <inheritdoc />
        protected override int DefaultTicksPerSecond => 20;

        /// <summary>The picture is the whole output; only the game's two status lines and the frame's newline sit outside it.</summary>
        protected override int ReservedRows => 4;

        /// <inheritdoc />
        protected override bool CenterPicture => false;

        /// <inheritdoc />
        protected override void Build()
        {
            // Departing is the drive form's duty whichever sibling runs — fort penalty, Departed status, the
            // mountain-pass stuck roll — and it must happen before the first day ticks.
            DriveTick.Depart();

            BuildLeg();
        }

        /// <summary>
        ///     One simulated day, exactly once per simulation tick — the same cadence and the same
        ///     <see cref="DriveTick" /> the text form drives with.
        /// </summary>
        protected override void OnSimulationTick()
        {
            switch (DriveTick.Run())
            {
                case DriveTick.Result.Disabled:
                    SetForm(typeof(UnableToContinue));
                    return;
                case DriveTick.Result.GraveCrossed:
                    SetForm(typeof(TombstoneQuestion));
                    return;
            }

            // The day may have changed the weather's ground colour; rebuild the cached backdrop only when it did.
            var weather = CurrentWeather();
            if (weather != _weather)
            {
                _weather = weather;
                _backdrop = TravelArt.BuildBackdrop(_terrain, _weather);
                Invalidate();
            }

            // A freshly executed ground event plants its picture in the world at the original's spot; from there
            // the wagon rolls past it. Sky events stay put — weather hangs over the scene, it is not passed by.
            if (SceneEvents.LastEventTurn != _seenEventTurn && SceneEvents.LastEventName != null)
            {
                _seenEventTurn = SceneEvents.LastEventTurn;
                var icon = MapIcon(SceneEvents.LastEventName);
                if (icon.HasValue && DosFrames.EventIconStandsOnGround(icon.Value))
                {
                    _groundIcon = icon;
                    var (spotX, _) = DosFrames.EventIconSpot(icon.Value);
                    _groundIconX = spotX * TravelGame.ScreenWidth / Art.Apple2Width;
                }
            }
        }

        /// <summary>
        ///     One animation tick: ease the displayed distance toward the simulation's real distance remaining. The
        ///     step is capped at two strides a tick so the three-frame walk cycle can never alias into stillness —
        ///     the trap minigames.md documents — which on the shortest legs trades a moment of lag for legs that
        ///     always read.
        /// </summary>
        protected override void Advance()
        {
            var game = GameSimulationApp.Instance;
            var target = (double) game.Trail.DistanceToNextLocation;
            var gap = _displayMiles - target;
            if (gap <= 0 || game.Vehicle.Status != VehicleStatusEnum.Moving)
                return;

            // Two strides a tick keeps the walk cycle honest; the gap/8 term lets a whole simulated day's jump
            // catch up within about half a second, so the scenery closes its final stretch to the wagon before the
            // arrival screen can interrupt the approach.
            var step = Math.Min(gap, Math.Max(2 * TravelGame.StepPixels / _pixelsPerMile, gap / 8.0));
            _displayMiles -= step;

            var before = _sceneryX;
            RecomputeSceneryX();
            var strides = Math.Max(0, (_sceneryX - before) / TravelGame.StepPixels);
            for (var stride = 0; stride < strides; stride++)
                _walkFrame = _walkFrame % 3 + 1;

            // A planted ground event falls behind with the ground covered — geared to the strides like the legs,
            // a few times ground speed so passing it takes about a day of travel rather than a week. It moves the
            // way this screen's world moves: left to right, the direction the scenery already slides, so the wagon
            // overtakes it and it exits past the right edge.
            if (_groundIcon.HasValue && strides > 0)
            {
                _groundIconX += strides * 6;
                if (_groundIconX > TravelGame.ScreenWidth)
                    _groundIcon = null;
            }
        }

        /// <inheritdoc />
        protected override string Compose()
        {
            var game = GameSimulationApp.Instance;
            var frame = _backdrop.Crop(0, 0, _backdrop.Width, _backdrop.Height);

            // The roadside piece — the next landmark approaching, seated on the ground line.
            if (_sceneryArt != null)
                frame.DrawImage(_sceneryArt, _sceneryX - _sceneryArt.Width, TravelGame.GroundY - _sceneryArt.Height);

            // The team: the walk cycle while rolling, the broken-wagon frame while disabled, the burning frame for
            // the day a fire just swept the wagon. Seated on the ground line rather than a fixed row, because the
            // damage frames are taller than the walking ones.
            var wagonFrame = game.Vehicle.Status == VehicleStatusEnum.Disabled ? TravelArt.BrokenWagon
                : EventIsFresh() && SceneEvents.LastEventName == nameof(OregonTrailDotNet.Event.Vehicle.VehicleFire)
                    ? TravelArt.BurningWagon
                    : _walkFrame;
            var wagon = Art.Dos("travelox", wagonFrame);
            frame.DrawImage(wagon, TravelGame.WagonX, TravelGame.GroundY - wagon.Height);

            // The day's event pictures, blitted last so a storm cloud sits over the horizon strip — the original
            // draws its event slots after the scene for the same reason. Sky pictures hang fixed for the day the
            // weather happened; ground pictures were planted in the world and slide past as the wagon rolls on.
            var sky = CurrentSkyIcon();
            if (sky.HasValue)
            {
                var art = DosFrames.EventIcon(sky.Value);
                var (spotX, spotY) = DosFrames.EventIconSpot(sky.Value);
                frame.DrawImage(art, spotX * TravelGame.ScreenWidth / Art.Apple2Width,
                    spotY * TravelGame.ScreenHeight / Art.Apple2Height);
            }

            if (_groundIcon.HasValue)
            {
                var art = DosFrames.EventIcon(_groundIcon.Value);
                frame.DrawImage(art, (int) Math.Round(_groundIconX), TravelGame.GroundY - art.Height);
            }

            TravelArt.DrawPrompt(frame, "Press ENTER to size up the situation");

            var date = game.Time.Date;
            var food = game.Vehicle.Inventory[EntitiesEnum.Food];
            TravelArt.DrawStatusPanel(frame, new List<(string, string)>
            {
                ("Date:", $"{date.Month} {date.Day}, {date.Year}"),
                ("Weather:", $"{game.Trail.CurrentLocation.Weather}"),
                ("Health:", $"{game.Vehicle.PassengerHealthStatus}"),
                ("Food:", $"{food?.TotalWeight ?? 0} pounds"),
                ("Next landmark:", $"{game.Trail.DistanceToNextLocation} miles"),
                ("Miles traveled:", $"{game.Vehicle.Odometer} miles")
            });

            return AnsiImage.FromPixels(frame).ToAnsi(PictureOptions());
        }

        /// <inheritdoc />
        public override void OnInputBufferReturned(string input)
        {
            // Parity with the text drive form: an empty ENTER pulls up and sizes up the situation; typed text is
            // ignored.
            if (!string.IsNullOrEmpty(input))
                return;

            DriveTick.Stop();
            ClearForm();
        }

        /// <inheritdoc />
        protected override void OnEscape()
        {
            DriveTick.Stop();
            ClearForm();
        }

        /// <summary>Sets up the leg toward the next landmark: its scenery piece, terrain side, and scroll gearing.</summary>
        private void BuildLeg()
        {
            var game = GameSimulationApp.Instance;
            var next = game.Trail.NextLocation;
            _destinationName = next?.Name;

            var stop = _destinationName != null ? OriginalTrail.ForLocation(_destinationName) : null;
            _terrain = stop is { Mountains: false } ? TravelTerrainEnum.Plains : TravelTerrainEnum.Mountains;
            _sceneryArt = stop is { ScenerySpriteId: > 0 } ? Art.Dos("scenery", stop.ScenerySpriteId) : null;

            // Every piece rests at the same solved position: right edge just left of the wagon, so arriving reads
            // as pulling up alongside the landmark. (OriginalTrail's per-leg SceneryRestX is the raw L% reference
            // column, NOT a position — using it parked the scenery a third of a screen short of the team.)
            _sceneryRestX = TravelGame.SceneryRestX;

            // The leg's real length: a fork branch's own road overrides the leg the fork set on arrival, exactly
            // as LocationFork does when it splices the branch in.
            var legTotal = next != null && next.LegDistance > 0
                ? next.LegDistance
                : game.Trail.CurrentLocation.TotalDistance;
            var sceneryWidth = _sceneryArt?.Width ?? 80;
            _pixelsPerMile = (_sceneryRestX + sceneryWidth) / Math.Max(1.0, legTotal);

            _displayMiles = game.Trail.DistanceToNextLocation;
            RecomputeSceneryX();

            _weather = CurrentWeather();
            _backdrop = TravelArt.BuildBackdrop(_terrain, _weather);
        }

        /// <summary>`IX = FX - D*C2`, quantised to whole strides so the world only ever moves in steps.</summary>
        private void RecomputeSceneryX()
        {
            var exact = _sceneryRestX - _displayMiles * _pixelsPerMile;
            _sceneryX = (int) Math.Floor(exact / TravelGame.StepPixels) * TravelGame.StepPixels;
        }

        /// <summary>An event picture stays up for the day it happened and the day after, then comes down.</summary>
        private static bool EventIsFresh() =>
            SceneEvents.LastEventName != null &&
            GameSimulationApp.Instance.TotalTurns <= SceneEvents.LastEventTurn + 1;

        /// <summary>The fixed sky picture for a freshly executed weather event, or null.</summary>
        private static EventIconEnum? CurrentSkyIcon()
        {
            if (!EventIsFresh())
                return null;

            var icon = MapIcon(SceneEvents.LastEventName);
            return icon.HasValue && !DosFrames.EventIconStandsOnGround(icon.Value) ? icon : null;
        }

        /// <summary>
        ///     The event picture for an executed event, or null. The mapping is by subject: the original's events
        ///     sheet carries seven pictures and the clone's roster covers six of them — the thief gets the
        ///     goods-carrying figure, the helpful Indians the one with the bundle.
        /// </summary>
        private static EventIconEnum? MapIcon(string eventName) => eventName switch
        {
            "Blizzard" => EventIconEnum.Blizzard,
            "HailStorm" => EventIconEnum.HailStorm,
            "SevereWeather" => EventIconEnum.Thunderstorm,
            "FindBerries" => EventIconEnum.WildFruit,
            "FindFruit" => EventIconEnum.WildFruit,
            "StuckInMountains" => EventIconEnum.SnowBound,
            "IndiansHelp" => EventIconEnum.Traveller,
            "Thief" => EventIconEnum.Trader,
            _ => null
        };

        /// <summary>
        ///     The original's one-line weather formula mapped onto the live simulation: white ground under snow,
        ///     the port's dry sand when the country has stayed dry, green otherwise.
        /// </summary>
        private static TravelWeatherEnum CurrentWeather()
        {
            var game = GameSimulationApp.Instance;
            var word = game.Trail.CurrentLocation.Weather.ToString();
            if (word.Contains("Snow", StringComparison.OrdinalIgnoreCase) ||
                word.Contains("Flurr", StringComparison.OrdinalIgnoreCase) ||
                word.Contains("Blizzard", StringComparison.OrdinalIgnoreCase) ||
                word.Contains("Freezing", StringComparison.OrdinalIgnoreCase))
                return TravelWeatherEnum.Snow;

            return game.Climate?.Wetness <= 0.2 ? TravelWeatherEnum.Arid : TravelWeatherEnum.Temperate;
        }
    }
}
