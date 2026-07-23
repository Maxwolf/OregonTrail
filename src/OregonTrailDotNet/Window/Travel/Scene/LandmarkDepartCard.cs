using System;
using System.Text;
using OregonTrailDotNet.Presentation;
using OregonTrailDotNet.Presentation.Audio;
using WolfCurses.Graphics;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Window.Travel.Scene
{
    /// <summary>
    ///     The departure card: the graphical sibling of <see cref="Dialog.LocationDepart" />, chosen by
    ///     <see cref="TravelInfo.DepartFormType" /> when presentation is on. Shows the same "From X it is N miles
    ///     to Y" text over the current stop's card while its tune plays — the original's <c>:2200</c> placement,
    ///     the second of the two moments the 1985 game put music on the trail.
    /// </summary>
    [ParentWindow(typeof(Travel))]
    public sealed class LandmarkDepartCard : SceneForm<TravelInfo>
    {
        private OriginalStop _stop;
        private PixelBuffer _picture;

        /// <summary>Initializes a new instance of the <see cref="LandmarkDepartCard" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public LandmarkDepartCard(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Whether the current stop has a card to depart under; without one (the Columbia and Barlow branches)
        ///     the plain text form runs instead.
        /// </summary>
        internal static bool CanShow =>
            (OriginalTrail.ForLocation(GameSimulationApp.Instance.Trail.CurrentLocation?.Name)?.CardArt ?? -1) >= 0;

        /// <inheritdoc />
        protected override bool Animated => false;

        /// <inheritdoc />
        protected override bool DismissOnAnyKey => true;

        /// <inheritdoc />
        protected override int ReservedRows => 8;

        /// <inheritdoc />
        protected override bool CenterPicture => false;

        /// <summary>
        ///     The departure card plays the loaded score — the current stop's tune. Unlike arrival, Independence
        ///     keeps its cue here: <c>song-00</c> is the opening's tune, and setting out from Independence is the
        ///     opening.
        /// </summary>
        protected override string MusicCue =>
            _stop != null && _stop.MusicSlug != null ? $"landmarks/{_stop.MusicSlug}" : null;

        /// <inheritdoc />
        protected override string FooterControlHints => "Press ENTER to continue";

        /// <inheritdoc />
        protected override void Build()
        {
            var game = GameSimulationApp.Instance;
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
            // The same words LocationDepart prints, so the two siblings are interchangeable.
            var game = GameSimulationApp.Instance;
            var text = new StringBuilder();
            text.AppendLine(
                $"{Environment.NewLine}From {game.Trail.CurrentLocation.Name} it is {game.Trail.DistanceToNextLocation}");
            text.AppendLine($"miles to {game.Trail.NextLocation.Name}");

            if (_picture != null)
                text.AppendLine(AnsiImage.FromPixels(_picture).ToAnsi(PictureOptions()));

            text.Append(Footer(string.Empty));
            return text.ToString();
        }

        /// <inheritdoc />
        protected override void OnDismiss()
        {
            Music.Stop();
            SetForm(TravelInfo.DriveFormType);
        }
    }
}
