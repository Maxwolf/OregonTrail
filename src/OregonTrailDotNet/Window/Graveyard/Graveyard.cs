// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using WolfCurses;
using WolfCurses.Core;
using WolfCurses.Window;

namespace OregonTrailDotNet.Window.Graveyard
{
    /// <summary>
    ///     Displays the name of a previous player whom traveled the trail and died at a given mile marker. There is also an
    ///     optional epitaph that can be displayed. These tombstones are saved per trail, and can be reset from main menu.
    /// </summary>
    public sealed class Graveyard : Window<TombstoneCommandsEnum, TombstoneInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Window{TCommands,TData}" /> class.
        /// </summary>
        /// <param name="simUnit">Core simulation which is controlling the form factory.</param>
        // ReSharper disable once UnusedMember.Global
        public Graveyard(SimulationApp simUnit) : base(simUnit)
        {
        }

        /// <summary>
        ///     Resets the input prompt so a context-specific prompt set by one form does not leak into the next.
        /// </summary>
        protected override void OnFormChange()
        {
            base.OnFormChange();
            PromptText = SceneGraph.PROMPT_TEXT_DEFAULT;

            // A scene form's tune must not outlive it: SetForm has no teardown, so stop Taps whenever a plain form
            // (the epitaph editor, the confirm dialog) takes over — the same guard Travel carries.
            if (GameSimulationApp.PresentationEnabled &&
                CurrentForm is not OregonTrailDotNet.Presentation.SceneForm<TombstoneInfo>)
                OregonTrailDotNet.Presentation.Audio.Music.Stop();
        }

        /// <summary>
        ///     Called after the Windows has been added to list of modes and made active.
        /// </summary>
        public override void OnWindowPostCreate()
        {
            base.OnWindowPostCreate();

            // Depending on the living status of passengers in current player vehicle we will attach a different
            // form; with presentation on, both are the stone scenes (the Apple II tombstone with Taps).
            SetForm(GameSimulationApp.Instance.Vehicle.PassengerLivingCount <= 0
                ? EpitaphQuestionScene.QuestionFormType
                : TombstoneScene.ViewFormType);
        }
    }
}