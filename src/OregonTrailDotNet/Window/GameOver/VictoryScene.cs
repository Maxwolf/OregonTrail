using System;
using System.Text;
using OregonTrailDotNet.Presentation;
using OregonTrailDotNet.Presentation.Audio;
using WolfCurses.Graphics;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Window.GameOver
{
    /// <summary>
    ///     Victory: the Willamette Valley card with its tune (<c>WIN:601</c> plays the loaded score at the valley)
    ///     over the congratulations text — the graphical sibling of <see cref="GameWin" />. Dismissal hands off to
    ///     the untouched <see cref="FinalPoints" /> scoring table.
    /// </summary>
    [ParentWindow(typeof(GameOver))]
    public sealed class VictoryScene : SceneForm<GameOverInfo>
    {
        private OriginalStop _stop;
        private PixelBuffer _picture;

        /// <summary>Initializes a new instance of the <see cref="VictoryScene" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public VictoryScene(IWindow window) : base(window)
        {
        }

        /// <inheritdoc />
        protected override bool Animated => false;

        /// <inheritdoc />
        protected override bool DismissOnAnyKey => true;

        /// <inheritdoc />
        protected override int ReservedRows => 11;

        /// <inheritdoc />
        protected override bool CenterPicture => false;

        /// <summary>The valley's tune — the score the original's WIN screen plays.</summary>
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
            var game = GameSimulationApp.Instance;
            var text = new StringBuilder();
            text.AppendLine($"{Environment.NewLine}Congratulations! You have made it to Oregon!");
            text.AppendLine($"Your journey took {game.Time.TotalDays:N0} days.");
            text.AppendLine($"You traveled {game.Vehicle.Odometer:N0} miles.");

            if (_picture != null)
                text.AppendLine(AnsiImage.FromPixels(_picture).ToAnsi(PictureOptions()));

            text.Append(Footer(string.Empty));
            return text.ToString();
        }

        /// <inheritdoc />
        protected override void OnDismiss()
        {
            Music.Stop();
            SetForm(typeof(FinalPoints));
        }
    }
}
