using WolfCurses.Window;

namespace OregonTrailDotNet.Bot.Ui
{
    /// <summary>
    ///     Shared user-data for every form in the bot control-panel window stack. Because a
    ///     <see cref="WolfCurses.Window.WindowData" /> instance is discarded when the window is torn down, anything that must
    ///     survive the handoff into a training/watch game session lives on the process-wide
    ///     <see cref="OregonTrailDotNet.Bot.BotContext" /> instead — this object only carries in-flight form state.
    /// </summary>
    public sealed class BotAppData : WindowData
    {
        /// <summary>
        ///     Training-model key chosen on the model-selection screen, carried into the naming screen that finalizes creation.
        /// </summary>
        public string NewProfileModelKey { get; set; } = string.Empty;

        /// <summary>
        ///     Profile the user is currently configuring/selecting. -1 means "none selected yet".
        /// </summary>
        public long ActiveProfileId { get; set; } = -1;

        /// <summary>
        ///     Human readable name of <see cref="ActiveProfileId" /> shown in headers.
        /// </summary>
        public string ActiveProfileName { get; set; } = string.Empty;

        /// <summary>
        ///     Number of candidate genomes evaluated per CEM generation (population size).
        /// </summary>
        public int PopulationSize { get; set; } = 12;

        /// <summary>
        ///     Number of games averaged together to score a single candidate genome (noise smoothing).
        /// </summary>
        public int GamesPerCandidate { get; set; } = 5;

        /// <summary>
        ///     Number of CEM generations to run in a single training batch.
        /// </summary>
        public int Generations { get; set; } = 5;
    }
}
