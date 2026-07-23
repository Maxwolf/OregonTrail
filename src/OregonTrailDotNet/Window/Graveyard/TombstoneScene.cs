using System;
using System.Text;
using OregonTrailDotNet.Presentation;
using OregonTrailDotNet.Presentation.Audio;
using WolfCurses.Graphics;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Window.Graveyard
{
    /// <summary>
    ///     The gravestone drawn as the original drew it: the Apple II tombstone card (the one screen the DOS port has
    ///     no picture for) with the inscription printed into the stone's text window while Taps plays
    ///     (<c>TOMB.LIB:50010</c> loads stone and score together). The graphical sibling of
    ///     <see cref="TombstoneView" />, covering both its paths — the all-dead end screen and a living party
    ///     reading a grave they came across on the trail.
    /// </summary>
    [ParentWindow(typeof(Graveyard))]
    public sealed class TombstoneScene : SceneForm<TombstoneInfo>
    {
        private PixelBuffer _picture;
        private bool _partyDead;

        /// <summary>Initializes a new instance of the <see cref="TombstoneScene" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public TombstoneScene(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Which form shows a finished gravestone. Every dispatch site that used to name
        ///     <see cref="TombstoneView" /> routes through here so the gate cannot miss one.
        /// </summary>
        internal static Type ViewFormType =>
            GameSimulationApp.PresentationEnabled ? typeof(TombstoneScene) : typeof(TombstoneView);

        /// <inheritdoc />
        protected override bool Animated => false;

        /// <inheritdoc />
        protected override bool DismissOnAnyKey => true;

        /// <inheritdoc />
        protected override int ReservedRows => _partyDead ? 10 : 6;

        /// <inheritdoc />
        protected override bool CenterPicture => false;

        /// <summary>Taps — the Apple II's, since the DOS port has no graveyard music at all.</summary>
        protected override string MusicCue => "tombstone";

        /// <inheritdoc />
        protected override string FooterControlHints => "Press ENTER to continue";

        /// <inheritdoc />
        protected override void Build()
        {
            var game = GameSimulationApp.Instance;
            _partyDead = game.Vehicle.PassengerLivingCount <= 0;

            // A living party is reading a grave they crossed on the trail; a dead one is looking at their own.
            var tombstone = UserData.Tombstone;
            if (!_partyDead)
            {
                tombstone = game.Tombstone.Encountered;
                if (tombstone == null)
                    game.Tombstone.FindTombstone(game.Vehicle.Odometer, out tombstone);
            }

            var stone = Art.Apple2Backdrop("tombstone.png");
            var canvas = new PixelBuffer(stone.Width, stone.Height, (byte[]) stone.Data.Clone());
            var epitaph = tombstone?.Epitaph ?? string.Empty;

            // The game's epitaphs run to 38 characters against the original's 29 — pass the real length so nothing
            // is clipped; the stone's five-row window is the honest bound and Draw clips to it.
            TombstoneArt.Draw(canvas,
                TombstoneArt.Inscription(tombstone?.PlayerName ?? string.Empty, epitaph, out _,
                    Math.Max(TombstoneArt.EpitaphLimit, epitaph.Length)),
                TombstoneArt.WindowX, TombstoneArt.WindowY, Palette.Apple2.Black);
            _picture = canvas;
        }

        /// <inheritdoc />
        protected override string Compose()
        {
            var text = new StringBuilder();
            text.AppendLine(AnsiImage.FromPixels(_picture).ToAnsi(PictureOptions()));

            if (_partyDead)
            {
                text.AppendLine("All the members of your party have died.");
                text.AppendLine($"You traveled {GameSimulationApp.Instance.Vehicle.Odometer:N0} miles.");
            }

            text.Append(Footer(string.Empty));
            return text.ToString();
        }

        /// <inheritdoc />
        protected override void OnDismiss()
        {
            Music.Stop();

            // The dead-party stone is the end of the game; a living party goes back to the trail.
            if (_partyDead)
            {
                UserData.ClearTombstone();
                GameSimulationApp.Instance.Restart();
                return;
            }

            ParentWindow.RemoveWindowNextTick();
        }
    }
}
