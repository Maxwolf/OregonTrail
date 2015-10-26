using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Allows the user to completely configure the simulation before they start off on the trail path. It will offer up
    ///     ability to choose names, professions, buy initial items, and starting month. The final thing it offers is ability
    ///     to change any of these values before actually starting the game as a final confirmation.
    /// </summary>
    public sealed class NewGameMode : GameMode<ModeCommands>, INewGameMode
    {
        public const string LEADER_QUESTION = "What is the first name of the wagon leader?";
        public const string MEMBERS_QUESTION = "What are the first names of the \nthree other members in your party?";

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.NewGameMode" /> class.
        /// </summary>
        public NewGameMode()
        {
            // Basic information to start a new simulation.
            NewGameInfo = new NewGameInfo();

            // Start right away with choosing profession in the new game mode.
            CurrentState = new SelectProfessionState(this, NewGameInfo);
        }

        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override ModeType ModeType
        {
            get { return ModeType.NewGame; }
        }

        /// <summary>
        ///     Default values for new game.
        /// </summary>
        public NewGameInfo NewGameInfo { get; set; }

        /// <summary>
        ///     Fired when this game mode is removed from the list of available and ticked modes in the simulation.
        /// </summary>
        public override void OnModeRemoved()
        {
            NewGameInfo = null;
        }
    }
}