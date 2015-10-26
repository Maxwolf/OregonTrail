using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Allows the user to completely configure the simulation before they start off on the trail path. It will offer up
    ///     ability to choose names, professions, buy initial items, and starting month. The final thing it offers is ability
    ///     to change any of these values before actually starting the game as a final confirmation.
    /// </summary>
    public sealed class NewGameMode : GameMode<NewGameCommands>, INewGameMode
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
            ChooseProfession();
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
        ///     Allows the user to input the four unique names for each party member in their group. If they don't want to enter a
        ///     name they can just press enter and a random name will be selected from a small array of names in that state.
        /// </summary>
        public void ChooseNames()
        {
            CurrentState = new InputPlayerNameState(0, this, NewGameInfo);
        }

        /// <summary>
        ///     Offers up selection of starting month which helps determine difficulty in regards to climate, temperature, random
        ///     events, grazing for animals, etc.
        /// </summary>
        public void ChooseStartMonth()
        {
            CurrentState = new SelectStartingMonthState(this, NewGameInfo);
        }

        /// <summary>
        ///     Default values for new game.
        /// </summary>
        public NewGameInfo NewGameInfo { get; set; }

        /// <summary>
        ///     Offers up selection of leader professions that affect the entire vehicle, party members, and starting monies.
        /// </summary>
        public void ChooseProfession()
        {
            CurrentState = new SelectProfessionState(this, NewGameInfo);
        }

        /// <summary>
        ///     Attaches a new game mode on top of this one allowing the player to use the initial starting monies they get from
        ///     profession selection to buy the items they will start the simulation with.
        /// </summary>
        public void BuyInitialItems()
        {
            CurrentState = new BuyInitialItemsState(this, NewGameInfo);
        }

        /// <summary>
        ///     Tells running simulation to take data we have fed it and begin trip down the trail path.
        /// </summary>
        public void StartGame()
        {
            // Remove the new game mode since it has done everything it can for us now.
            RemoveModeNextTick();

            // Pass the new game info user data object to the simulation one final time.
            GameSimulationApp.Instance.SetData(NewGameInfo);
        }

        /// <summary>
        ///     Fired when this game mode is removed from the list of available and ticked modes in the simulation.
        /// </summary>
        public override void OnModeRemoved()
        {
            NewGameInfo = null;
        }
    }
}