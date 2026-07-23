using System;
using System.Text;
using OregonTrailDotNet.Presentation;
using WolfCurses.Graphics;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Window.Travel.Scene
{
    /// <summary>
    ///     The raft's instruction card: The Dalles over its own picture, with FLOAT's instruction text verbatim
    ///     (<c>:1010/:1015</c>) — the original loads <c>L16.PCK</c> as the raft's backdrop the same way. Silent,
    ///     because the raft is: <c>FLOAT</c> plays a loaded score on a collision but never loads one, and the doc
    ///     leaves that loose end silent rather than guessed at.
    /// </summary>
    [ParentWindow(typeof(Travel))]
    public sealed class RaftIntro : SceneForm<TravelInfo>
    {
        private PixelBuffer _picture;

        /// <summary>Initializes a new instance of the <see cref="RaftIntro" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public RaftIntro(IWindow window) : base(window)
        {
        }

        /// <inheritdoc />
        protected override bool Animated => false;

        /// <inheritdoc />
        protected override bool DismissOnAnyKey => true;

        /// <inheritdoc />
        protected override int ReservedRows => 9;

        /// <inheritdoc />
        protected override bool CenterPicture => false;

        /// <inheritdoc />
        protected override string FooterControlHints => "Press ENTER to shove off";

        /// <inheritdoc />
        protected override void Build()
        {
            var game = GameSimulationApp.Instance;
            var stop = OriginalTrail.ForLocation(game.Trail.CurrentLocation?.Name);

            // The Dalles' card (p16) backs the instructions; the caption dates the departure onto the water.
            var date = game.Time.Date;
            _picture = LandmarkArt.WithCaption(
                LandmarkArt.Card(stop is { CardArt: >= 0 } ? stop.CardArt : 16),
                "The Dalles",
                $"{date.Month} {date.Day}, {date.Year}");
        }

        /// <inheritdoc />
        protected override string Compose()
        {
            var text = new StringBuilder();
            text.AppendLine($"{Environment.NewLine}Use the arrow keys to guide your raft");
            text.AppendLine("through the rushing waters of the Columbia River.");
            text.AppendLine($"{Environment.NewLine}After passing the third direction sign, land");
            text.AppendLine("your raft at the trail to the Willamette Valley.");
            text.AppendLine(AnsiImage.FromPixels(_picture).ToAnsi(PictureOptions()));
            text.Append(Footer(string.Empty));
            return text.ToString();
        }

        /// <inheritdoc />
        protected override void OnDismiss() => SetForm(typeof(RaftScene));
    }
}
