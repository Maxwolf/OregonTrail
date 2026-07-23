using System;
using OregonTrailDotNet.Entity.Vehicle;
using OregonTrailDotNet.Presentation;
using OregonTrailDotNet.Presentation.Audio;
using WolfCurses.Graphics;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Window.Travel.Scene
{
    /// <summary>
    ///     The opening: "Going back to 1848..." over the Independence card while the opening tune plays — the
    ///     graphical sibling of <see cref="Dialog.LocationArrive" />'s first-location prompt, shown once when the
    ///     player leaves Matt's store and the journey truly begins. This is where <c>song-00</c> belongs (the
    ///     original's <c>:1005</c> loader never plays it on a stop; it is the opening's tune), and where the
    ///     Independence picture belongs — arrivals elsewhere show their card after the look-around question instead.
    /// </summary>
    [ParentWindow(typeof(Travel))]
    public sealed class OpeningCard : SceneForm<TravelInfo>
    {
        private OriginalStop _stop;
        private PixelBuffer _picture;

        /// <summary>Initializes a new instance of the <see cref="OpeningCard" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public OpeningCard(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Whether the opening plays graphically: presentation on, standing at the trail's first location, and a
        ///     card to show. Headless hosts always fall through to the plain text prompt.
        /// </summary>
        internal static bool ShouldShow =>
            GameSimulationApp.PresentationEnabled && GameSimulationApp.Instance.Trail.IsFirstLocation &&
            (OriginalTrail.ForLocation(GameSimulationApp.Instance.Trail.CurrentLocation?.Name)?.CardArt ?? -1) >= 0;

        /// <inheritdoc />
        protected override bool Animated => false;

        /// <inheritdoc />
        protected override bool DismissOnAnyKey => true;

        /// <inheritdoc />
        protected override int ReservedRows => 7;

        /// <inheritdoc />
        protected override bool CenterPicture => false;

        /// <summary>The opening's own tune, the one score arrival never plays.</summary>
        protected override string MusicCue =>
            _stop != null && _stop.MusicSlug != null ? $"landmarks/{_stop.MusicSlug}" : null;

        /// <inheritdoc />
        protected override string FooterControlHints => "Press ENTER to continue";

        /// <inheritdoc />
        protected override void Build()
        {
            var game = GameSimulationApp.Instance;

            // Vehicle is stopped while the opening plays, exactly as the text prompt stops it.
            game.Vehicle.Status = VehicleStatusEnum.Stopped;

            _stop = OriginalTrail.ForLocation(game.Trail.CurrentLocation?.Name);
            if (_stop == null || _stop.CardArt < 0)
                return;

            var date = game.Time.Date;
            _picture = LandmarkArt.WithCaption(
                LandmarkArt.Card(_stop.CardArt),
                LandmarkArt.CaptionName(_stop.OriginalName),
                $"{date.Month} {date.Day}, {date.Year}");
        }

        /// <inheritdoc />
        protected override string Compose()
        {
            var text = $"{Environment.NewLine}Going back to {GameSimulationApp.Instance.Time.CurrentYear}...{Environment.NewLine}";

            if (_picture == null)
                return text + Environment.NewLine + "Press ENTER to continue.";

            return text + AnsiImage.FromPixels(_picture).ToAnsi(PictureOptions()) + Environment.NewLine +
                   Footer(string.Empty);
        }

        /// <inheritdoc />
        protected override void OnDismiss()
        {
            Music.Stop();
            ClearForm();
        }
    }
}
