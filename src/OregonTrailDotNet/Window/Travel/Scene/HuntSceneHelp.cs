using System;
using System.Text;
using WolfCurses.Window;
using WolfCurses.Window.Form;
using WolfCurses.Window.Form.Input;

namespace OregonTrailDotNet.Window.Travel.Scene
{
    /// <summary>
    ///     The hunt's instruction card, describing the controls the 1985 original taught on its own instruction
    ///     screen: the ring of keys around L (and the keypad) aim outright, the arrows turn a step at a time, SPACE
    ///     fires the one bullet the rifle keeps in the air, and RETURN starts and stops walking.
    ///     <para>
    ///         There is one hunt and every host plays it. This screen used to be the graphical half of a pair, with
    ///         a word-typing hunt behind the same menu entry for headless hosts — a genuinely different game with its
    ///         own species, its own hit rule and ammunition charged per kill rather than per shot, which meant the
    ///         training bot spent its whole life optimizing an economy no player ever saw. That fork is gone.
    ///     </para>
    /// </summary>
    [ParentWindow(typeof(Travel))]
    public sealed class HuntSceneHelp : InputForm<TravelInfo>
    {
        /// <summary>Initializes a new instance of the <see cref="HuntSceneHelp" /> class.</summary>
        /// <param name="window">The parent window.</param>
        // ReSharper disable once UnusedMember.Global — created by the form factory.
        public HuntSceneHelp(IWindow window) : base(window)
        {
        }

        /// <inheritdoc />
        protected override string OnDialogPrompt()
        {
            var prompt = new StringBuilder();
            prompt.AppendLine($"{Environment.NewLine}HUNTING RULES");
            prompt.AppendLine($"{Environment.NewLine}Aim the rifle with the ARROW keys, the ring of");
            prompt.AppendLine("keys around L (I O P ; / . , K), or the NUMPAD.");
            prompt.AppendLine("The rifle swings a step at a time, the short way");
            prompt.AppendLine($"round.{Environment.NewLine}");
            prompt.AppendLine("SPACE fires - one bullet in the air at a time,");
            prompt.AppendLine("one round spent per shot. RETURN starts and");
            prompt.AppendLine($"stops walking; you walk the way the rifle points.{Environment.NewLine}");
            prompt.AppendLine("You can carry 100 pounds of meat back to the");
            prompt.AppendLine("wagon. ESC ends the hunt early and keeps the bag.");
            return prompt.ToString();
        }

        /// <inheritdoc />
        protected override void OnDialogResponse(DialogResponseEnum reponse)
        {
            SetForm(typeof(HuntScene));
        }
    }
}
