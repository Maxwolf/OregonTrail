using System;
using System.Text;
using WolfCurses.Window;
using WolfCurses.Window.Form;
using WolfCurses.Window.Form.Input;

namespace OregonTrailDotNet.Window.Travel.Scene
{
    /// <summary>
    ///     The real-time hunt's instruction card — the graphical sibling of <see cref="Hunt.Help.HuntingPrompt" />,
    ///     describing the controls the 1985 original taught on its own instruction screen: the ring of keys around L
    ///     (and the keypad) aim outright, the arrows turn a step at a time, SPACE fires the one bullet the rifle
    ///     keeps in the air, and RETURN starts and stops walking.
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

        /// <summary>
        ///     Which form opens a hunt: the real-time hunt's instructions when presentation is on, the word-typing
        ///     hunt's for every headless host. The no-ammo refusal is shared and upstream of this choice.
        /// </summary>
        internal static Type FormType =>
            GameSimulationApp.PresentationEnabled ? typeof(HuntSceneHelp) : typeof(Hunt.Help.HuntingPrompt);

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
