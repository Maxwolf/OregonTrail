using System;
using System.Text;
using OregonTrailDotNet.Presentation;
using OregonTrailDotNet.Presentation.Audio;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Window.Travel.Scene
{
    /// <summary>
    ///     The departure screen with its tune: the graphical sibling of <see cref="Dialog.LocationDepart" />, chosen
    ///     by <see cref="TravelInfo.DepartFormType" /> when presentation is on. Same "From X it is N miles to Y"
    ///     words, plus the current stop's tune — the original's <c>:2200</c> placement. Deliberately no picture: the
    ///     cards belong to the look-around arrival (and the opening at Independence), not to setting out.
    /// </summary>
    [ParentWindow(typeof(Travel))]
    public sealed class LandmarkDepartCard : SceneForm<TravelInfo>
    {
        private OriginalStop _stop;

        /// <summary>Initializes a new instance of the <see cref="LandmarkDepartCard" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public LandmarkDepartCard(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Whether the current stop has a tune to depart under; without one (the Columbia and Barlow branches)
        ///     the plain text form runs instead.
        /// </summary>
        internal static bool CanShow =>
            OriginalTrail.ForLocation(GameSimulationApp.Instance.Trail.CurrentLocation?.Name)?.MusicSlug != null;

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
            _stop = OriginalTrail.ForLocation(GameSimulationApp.Instance.Trail.CurrentLocation?.Name);
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
