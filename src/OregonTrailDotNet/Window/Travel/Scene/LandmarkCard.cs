using System;
using OregonTrailDotNet.Presentation;
using OregonTrailDotNet.Presentation.Audio;
using WolfCurses.Graphics;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Window.Travel.Scene
{
    /// <summary>
    ///     The landmark card: the stop's original MECC artwork with the name-over-date caption box, shown only when
    ///     the player answers Yes to "Would you like to look around?" — the exact placement of the 1985 original,
    ///     which plays the stop's tune under the picture until a key lands. Reached solely through
    ///     <see cref="Dialog.LocationArrive" /> when <see cref="GameSimulationApp.PresentationEnabled" /> is on, so
    ///     headless hosts never see it.
    /// </summary>
    [ParentWindow(typeof(Travel))]
    public sealed class LandmarkCard : SceneForm<TravelInfo>
    {
        private OriginalStop _stop;
        private PixelBuffer _picture;

        /// <summary>Initializes a new instance of the <see cref="LandmarkCard" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public LandmarkCard(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Whether looking around here has a card to show: presentation on, and the current location resolves to
        ///     an original stop that has one (the Columbia and Barlow branches do not). When false the caller keeps
        ///     today's behavior — straight back to the travel menu.
        /// </summary>
        internal static bool ShouldShow =>
            GameSimulationApp.PresentationEnabled &&
            (OriginalTrail.ForLocation(GameSimulationApp.Instance.Trail.CurrentLocation?.Name)?.CardArt ?? -1) >= 0;

        /// <inheritdoc />
        protected override bool Animated => false;

        /// <inheritdoc />
        protected override bool DismissOnAnyKey => true;

        /// <inheritdoc />
        protected override int ReservedRows => 5;

        /// <inheritdoc />
        protected override bool CenterPicture => false;

        /// <summary>
        ///     The stop's tune — except at Independence, whose score belongs to the opening, not the stop: the
        ///     original's <c>:1005</c> loader is guarded by <c>IF LM THEN</c>.
        /// </summary>
        protected override string MusicCue =>
            _stop != null && _stop.Index > 0 && _stop.MusicSlug != null ? $"landmarks/{_stop.MusicSlug}" : null;

        /// <inheritdoc />
        protected override string FooterControlHints => "Press ENTER to continue";

        /// <inheritdoc />
        protected override void Build()
        {
            var game = GameSimulationApp.Instance;
            _stop = OriginalTrail.ForLocation(game.Trail.CurrentLocation?.Name);
            if (_stop == null || _stop.CardArt < 0)
                return;

            // The caption is the original's: Z$ (name, article stripped) over TD$ (the arrival date) — the
            // simulation value the workbench slideshow deliberately left out because it had no journey to date.
            var date = game.Time.Date;
            _picture = LandmarkArt.WithCaption(
                LandmarkArt.Card(_stop.CardArt),
                LandmarkArt.CaptionName(_stop.OriginalName),
                $"{date.Month} {date.Day}, {date.Year}");
        }

        /// <inheritdoc />
        protected override string Compose()
        {
            // Fail soft: no resolvable card means nothing to show — the next key dismisses back to the menu.
            if (_picture == null)
                return $"{Environment.NewLine}Press ENTER to continue.";

            return AnsiImage.FromPixels(_picture).ToAnsi(PictureOptions()) + Environment.NewLine +
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
