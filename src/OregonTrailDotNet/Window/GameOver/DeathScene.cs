using System;
using System.Text;
using OregonTrailDotNet.Entity.Person;
using OregonTrailDotNet.Presentation;
using WolfCurses.Graphics;
using WolfCurses.Utility;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Window.GameOver
{
    /// <summary>
    ///     The party's end: the wrecked wagon composed large on a dark field over the cause of death, distance, and
    ///     remaining supplies — the graphical sibling of <see cref="GameFail" />. There is no dedicated 1985 death
    ///     card, so the art is the DOS travel sheet's broken-wagon frame; and deliberately no music — Taps belongs
    ///     to the Graveyard stone that immediately follows, and playing it twice back-to-back would cheapen it.
    /// </summary>
    [ParentWindow(typeof(GameOver))]
    public sealed class DeathScene : SceneForm<GameOverInfo>
    {
        private PixelBuffer _picture;

        /// <summary>Initializes a new instance of the <see cref="DeathScene" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public DeathScene(IWindow window) : base(window)
        {
        }

        /// <inheritdoc />
        protected override bool Animated => false;

        /// <inheritdoc />
        protected override bool DismissOnAnyKey => true;

        /// <inheritdoc />
        protected override int ReservedRows => 19;

        /// <inheritdoc />
        protected override bool CenterPicture => false;

        /// <inheritdoc />
        protected override string FooterControlHints => "Press ENTER to continue";

        /// <inheritdoc />
        protected override void Build()
        {
            // The broken wagon (travelox #4), doubled and centred on a dark field so it reads as a scene rather
            // than a cut sprite.
            var wreck = Art.Dos("travelox", TravelArt.BrokenWagon);
            var doubled = wreck.Resize(wreck.Width * 2, wreck.Height * 2);

            var field = new PixelBuffer(320, doubled.Height + 40);
            for (var y = 0; y < field.Height; y++)
                for (var x = 0; x < field.Width; x++)
                    field.SetPixel(x, y, Palette.Black);

            field.DrawImage(doubled, (field.Width - doubled.Width) / 2, 20);
            _picture = field;
        }

        /// <inheritdoc />
        protected override string Compose()
        {
            var game = GameSimulationApp.Instance;
            var text = new StringBuilder();
            text.AppendLine($"{Environment.NewLine}Your party has perished.");

            // Explain the cause of death when the party leader has a recorded one.
            var leader = game.Vehicle.PassengerLeader;
            if ((leader != null) && (leader.Cause != CauseOfDeathEnum.Unknown))
                text.AppendLine($"{leader.Name} {leader.Cause.ToDescriptionAttribute()}.");

            text.AppendLine($"You traveled {game.Vehicle.Odometer:N0} miles.");
            text.AppendLine(AnsiImage.FromPixels(_picture).ToAnsi(PictureOptions()));
            text.AppendLine("Remaining supplies:");
            text.AppendLine(SupplyPanel.Build(includeCash: true));
            text.Append(Footer(string.Empty));
            return text.ToString();
        }

        /// <inheritdoc />
        protected override void OnDismiss()
        {
            // On to the graveyard where the player can leave an epitaph; the graveyard flow resets the game.
            GameSimulationApp.Instance.WindowManager.Add(typeof(Graveyard.Graveyard));
        }
    }
}
