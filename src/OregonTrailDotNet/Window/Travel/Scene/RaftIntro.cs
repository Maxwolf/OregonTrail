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
    ///     because the raft has no melody: <c>FLOAT</c>'s only audio is the descending crash score it hand-pokes
    ///     at init and plays on a collision (docs/legacy-sounds.md §2), which this port renders as the crash
    ///     effect in <see cref="RaftScene" />.
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
            // The Dalles' card backs the instructions because the original's FLOAT loads L16.PCK as the raft's own
            // backdrop — it is literally a raft on this river. No caption box: the party stands at the Columbia,
            // and naming the picture "The Dalles" here read as the wrong place.
            _picture = LandmarkArt.Card(16);
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
