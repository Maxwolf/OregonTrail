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
    ///     "Would you like to write an epitaph?" asked over the stone itself, the way the original took the epitaph
    ///     on the tombstone screen with Taps already playing. The graphical sibling of
    ///     <see cref="EpitaphQuestion" />; the epitaph *entry* stays the plain text editor either way.
    /// </summary>
    [ParentWindow(typeof(Graveyard))]
    public sealed class EpitaphQuestionScene : SceneForm<TombstoneInfo>
    {
        private PixelBuffer _picture;

        /// <summary>Initializes a new instance of the <see cref="EpitaphQuestionScene" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public EpitaphQuestionScene(IWindow window) : base(window)
        {
        }

        /// <summary>Which form asks the epitaph question.</summary>
        internal static Type QuestionFormType =>
            GameSimulationApp.PresentationEnabled ? typeof(EpitaphQuestionScene) : typeof(EpitaphQuestion);

        /// <inheritdoc />
        protected override bool Animated => false;

        /// <inheritdoc />
        protected override int ReservedRows => 7;

        /// <inheritdoc />
        protected override bool CenterPicture => false;

        /// <summary>Taps plays from the moment the stone appears, exactly as <c>TOMB.LIB:50010</c> has it.</summary>
        protected override string MusicCue => "tombstone";

        /// <inheritdoc />
        protected override string FooterControlHints => "Y write an epitaph   N leave the stone as it is";

        /// <inheritdoc />
        protected override void Build()
        {
            var stone = Art.Apple2Backdrop("tombstone.png");
            var canvas = new PixelBuffer(stone.Width, stone.Height, (byte[]) stone.Data.Clone());

            // The stone before any epitaph: "Here lies" and the name, nothing else yet.
            TombstoneArt.Draw(canvas,
                TombstoneArt.Inscription(UserData.Tombstone?.PlayerName ?? string.Empty, string.Empty, out _),
                TombstoneArt.WindowX, TombstoneArt.WindowY, Palette.Apple2.Black);
            _picture = canvas;
        }

        /// <inheritdoc />
        protected override string Compose()
        {
            var text = new StringBuilder();
            text.AppendLine(AnsiImage.FromPixels(_picture).ToAnsi(PictureOptions()));
            text.AppendLine("Would you like to write an epitaph? Y/N");
            text.Append(Footer(string.Empty));
            return text.ToString();
        }

        /// <inheritdoc />
        protected override void OnSectionKey(ConsoleKey key)
        {
            if (key == ConsoleKey.Y)
            {
                // On to the text editor; the window's form-change guard stops Taps while typing, and the finished
                // stone re-asserts it.
                SetForm(typeof(EpitaphEditor));
                return;
            }

            DeclineEpitaph();
        }

        /// <inheritdoc />
        public override void OnInputBufferReturned(string input)
        {
            // A bare ENTER — or a typed "y"/"n" the player submitted out of habit — answers the question too.
            if (input != null && input.Trim().StartsWith("y", StringComparison.OrdinalIgnoreCase))
            {
                SetForm(typeof(EpitaphEditor));
                return;
            }

            DeclineEpitaph();
        }

        /// <inheritdoc />
        protected override void OnEscape() => DeclineEpitaph();

        /// <summary>Anything but a yes keeps the stone as it stands — the original's Custom-counts-as-No reading.</summary>
        private void DeclineEpitaph()
        {
            GameSimulationApp.Instance.Tombstone.Add(UserData.Tombstone);
            SetForm(TombstoneScene.ViewFormType);
        }
    }
}
