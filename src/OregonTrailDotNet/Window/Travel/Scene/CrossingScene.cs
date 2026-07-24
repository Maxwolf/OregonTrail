using System;
using System.Linq;
using OregonTrailDotNet.Presentation;
using OregonTrailDotNet.Presentation.Audio;
using OregonTrailDotNet.Window.Travel.RiverCrossing;
using WolfCurses.Graphics;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Window.Travel.Scene
{
    /// <summary>
    ///     The river crossing drawn as <c>CROSS.LIB</c> drew it: a framed picture of the water with one vehicle
    ///     sprite, the near bank turning to water as the party pulls away, and then the outcome — the far shore
    ///     fanning into view on a clean crossing, the wreck swapped in place or the swamping wedge over a ford when
    ///     the river took something. The graphical sibling of <see cref="CrossingTick" />, driving the identical
    ///     <see cref="CrossingSimulation" /> underneath; the branch is the original's <c>Z&gt;0</c> — on what was
    ///     actually <b>lost</b>, not on which event fired, so a loss-free scare lands like a clean crossing.
    ///     <para>
    ///         A midstream disaster plays in the right order: the simulation defers its event so the scene can show
    ///         the wagon in trouble first — the wreck swap or the swamping sweep — and only then fires the event
    ///         window with its message; when that clears, the crossing carries on, wrecked if something was really
    ///         lost and righted if it was only a scare.
    ///     </para>
    /// </summary>
    [ParentWindow(typeof(Travel))]
    public sealed class CrossingScene : SceneForm<TravelInfo>
    {
        /// <summary>Where a midstream disaster is in its show-then-tell sequence.</summary>
        private enum DisasterPhaseEnum
        {
            /// <summary>No disaster has struck.</summary>
            None,

            /// <summary>The trouble is on screen — the sweep or the wreck — and the simulation holds its breath.</summary>
            Playing,

            /// <summary>The event window is up with the message; waiting for the player to clear it.</summary>
            Fired,

            /// <summary>The message is read and the crossing has carried on.</summary>
            Settled
        }

        private CrossingSimulation _sim;
        private int _itemsBefore;
        private int _livingBefore;
        private bool _lost;
        private bool _midstreamLost;
        private DisasterPhaseEnum _disaster = DisasterPhaseEnum.None;
        private double _disasterProgress;
        private double _displayCrossing;
        private double _outcomeProgress;
        private bool _outcomeShown;

        /// <summary>Initializes a new instance of the <see cref="CrossingScene" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public CrossingScene(IWindow window) : base(window)
        {
        }

        /// <inheritdoc />
        protected override int DefaultTicksPerSecond => 20;

        /// <inheritdoc />
        protected override int ReservedRows => 5;

        /// <inheritdoc />
        protected override bool CenterPicture => false;

        /// <summary>Keys mean nothing midstream; once the outcome has played, any key moves to the result.</summary>
        protected override bool DismissOnAnyKey => _outcomeShown;

        /// <inheritdoc />
        protected override string FooterControlHints => "Press ENTER to continue";

        /// <inheritdoc />
        protected override void Build()
        {
            _sim = new CrossingSimulation(UserData.River);
            _sim.DeferMidstreamDisasters = true;
            _sim.Begin();

            // The Z>0 baseline, taken after payment so an agreed fare never reads as river damage.
            _itemsBefore = InventoryCount();
            _livingBefore = GameSimulationApp.Instance.Vehicle.PassengerLivingCount;
        }

        /// <inheritdoc />
        protected override void OnSimulationTick()
        {
            if (_sim == null)
                return;

            switch (_disaster)
            {
                case DisasterPhaseEnum.Playing:
                    // The trouble is still on screen; the river waits for the animation.
                    return;
                case DisasterPhaseEnum.Fired:
                    // Ticking again means the event window has been read and closed. Settle what it actually cost,
                    // then land the wagon: after a midstream disaster the party has suffered enough, and a wrecked
                    // wagon paddling on for the rest of the river reads wrong anyway — the far shore is next.
                    _midstreamLost = InventoryCount() < _itemsBefore ||
                                     GameSimulationApp.Instance.Vehicle.PassengerLivingCount < _livingBefore;
                    _disaster = DisasterPhaseEnum.Settled;
                    _sim.FinishNow();
                    _lost = _midstreamLost;
                    return;
            }

            if (_sim.Finished)
                return;

            _sim.Tick();

            // A ferry or guide accident executes its event inside the tick — that path never defers, so there
            // is no disaster beat to carry the sound and the crash lands with the event window instead. The DOS
            // port crashed ferry sinkings exactly like float ones (docs/legacy-sounds.md §1.2).
            if (_sim.AccidentThisTick)
                Sfx.Crash();

            // A midstream disaster: hold the simulation and let the scene show it before the message.
            if (_sim.PendingDisaster != null)
            {
                _disaster = DisasterPhaseEnum.Playing;
                _disasterProgress = 0;

                // Only a ford or a float defers, and only the float goes visibly under — the crash sounds over
                // the wreck sprite this branch swaps in, never over a fording swamp, which the DOS port left
                // silent (docs/legacy-sounds.md §1.2).
                if (UserData.River.CrossingType != RiverCrossChoiceEnum.Ford)
                    Sfx.Crash();

                return;
            }

            // The far bank: settle the original's Z>0 question from what actually changed while crossing.
            if (_sim.Finished)
                _lost = InventoryCount() < _itemsBefore ||
                        GameSimulationApp.Instance.Vehicle.PassengerLivingCount < _livingBefore;
        }

        /// <inheritdoc />
        protected override void Advance()
        {
            // The disaster beat: sweep the trouble across the picture, then hand over to the event window.
            if (_disaster == DisasterPhaseEnum.Playing)
            {
                _disasterProgress = Math.Min(1.0, _disasterProgress + 1.0 / 24);
                if (_disasterProgress < 1.0)
                    return;

                var pending = _sim.PendingDisaster;
                _sim.PendingDisaster = null;
                _disaster = DisasterPhaseEnum.Fired;
                if (pending != null)
                    GameSimulationApp.Instance.EventDirector.TriggerEvent(GameSimulationApp.Instance.Vehicle,
                        pending);
                return;
            }

            // The displayed crossing chases the simulation's real progress on the animation clock, so the bank
            // turns to water smoothly instead of jumping a quarter of the river once a second.
            var width = Math.Max(1, UserData.River.RiverWidth);
            var target = _sim == null ? 0 : Math.Min(1.0, (double) _sim.FeetCrossed / width);
            var gap = target - _displayCrossing;
            if (gap > 0)
                _displayCrossing += Math.Min(gap, Math.Max(0.008, gap * 0.15));

            // The outcome sweep plays once the water is visibly behind the wagon: the far-bank fan over about
            // three seconds on a clean crossing, or just a short beat when the damage is already on screen.
            if (_sim is not { Finished: true } || _outcomeShown || _displayCrossing < 0.995)
                return;

            _outcomeProgress = Math.Min(1.0, _outcomeProgress + (_lost ? 1.0 / 20 : 1.0 / 60));
            if (_outcomeProgress >= 1.0)
                _outcomeShown = true;
        }

        /// <inheritdoc />
        protected override string Compose()
        {
            var river = UserData.River;
            var frame = new PixelBuffer(RiverCrossingGame.ScreenWidth, RiverCrossingGame.ScreenHeight);
            CrossingArt.Fill(frame, 0, 0, frame.Width, frame.Height, Palette.Black);

            // The white picture frame, the water, and the near bank fully in earth — the standing scene.
            CrossingArt.Fill(frame, RiverCrossingGame.FrameX1, RiverCrossingGame.FrameY1,
                RiverCrossingGame.FrameX2 - RiverCrossingGame.FrameX1,
                RiverCrossingGame.FrameY2 - RiverCrossingGame.FrameY1, Palette.White);
            CrossingArt.Fill(frame, RiverCrossingGame.ToX(58), RiverCrossingGame.ToY(24),
                RiverCrossingGame.ToX(58 + 161) - RiverCrossingGame.ToX(58),
                RiverCrossingGame.ToY(24 + 113) - RiverCrossingGame.ToY(24), Palette.Water);
            CrossingArt.NearBank(frame, 1.0, Palette.Sand);

            // The near bank turns to water as the party pulls away.
            CrossingArt.NearBank(frame, _displayCrossing, Palette.Water);

            // One vehicle sprite — the whole of "which picture": the guide has no sprite of his own and borrows
            // the float or the ford by depth, exactly as :50140 does.
            var fording = river.CrossingType == RiverCrossChoiceEnum.Ford;
            var wrecked = !fording && ShowTrouble(persistedOnly: false);
            var sprite = wrecked ? CrossingArt.WreckSprite : VehicleSprite(river);
            var vehicle = Art.Dos("float", sprite);
            frame.DrawImage(vehicle, RiverCrossingGame.VehicleX, RiverCrossingGame.VehicleY);

            // A fording wagon in trouble is not replaced — the water fans over it where it stands.
            if (fording)
            {
                if (_disaster == DisasterPhaseEnum.Playing)
                    CrossingArt.SwampWedge(frame, _disasterProgress, Palette.Water);
                else if (ShowTrouble(persistedOnly: true))
                    CrossingArt.SwampWedge(frame, 1.0, Palette.Water);
            }

            // The far shore fans into view only on a crossing that lost nothing — the original's Z>0 branch.
            if (_sim is { Finished: true } && !_lost)
                CrossingArt.FarBank(frame, _outcomeProgress, Palette.Sand);

            var picture = AnsiImage.FromPixels(frame).ToAnsi(PictureOptions());
            return _outcomeShown
                ? picture + Environment.NewLine + Footer(string.Empty)
                : picture;
        }

        /// <inheritdoc />
        protected override void OnDismiss() => SetForm(typeof(CrossingResult));

        /// <inheritdoc />
        protected override void OnEscape()
        {
            // There is no abandoning a river midstream; once the outcome has played ESC is just another key.
            if (_outcomeShown)
                OnDismiss();
        }

        /// <summary>
        ///     Whether the wagon shows its trouble right now. During the beat and the message it always does; once
        ///     the message settles (or the crossing ends) only a real loss keeps the damage on screen — a scare
        ///     rights itself and carries on.
        /// </summary>
        private bool ShowTrouble(bool persistedOnly)
        {
            if (!persistedOnly && _disaster is DisasterPhaseEnum.Playing or DisasterPhaseEnum.Fired)
                return true;

            if (_disaster is DisasterPhaseEnum.Fired && persistedOnly)
                return true;

            if (_disaster == DisasterPhaseEnum.Settled && _midstreamLost)
                return true;

            return _sim is { Finished: true } && _lost;
        }

        /// <summary>Everything countable in the wagon, the coarse total the loss delta is measured against.</summary>
        private static int InventoryCount() =>
            GameSimulationApp.Instance.Vehicle.Inventory.Values.Sum(item => item.Quantity);

        /// <summary>`I` — ford 1, float 2, ferry 3; the Indian guide borrows by depth (`:50140`).</summary>
        private static int VehicleSprite(RiverGenerator river) => river.CrossingType switch
        {
            RiverCrossChoiceEnum.Ford => CrossingArt.FordSprite,
            RiverCrossChoiceEnum.Float => CrossingArt.FloatSprite,
            RiverCrossChoiceEnum.Ferry => CrossingArt.FerrySprite,
            RiverCrossChoiceEnum.Indian => river.RiverDepth > 2.4
                ? CrossingArt.FloatSprite
                : CrossingArt.FordSprite,
            _ => CrossingArt.FordSprite
        };
    }
}
