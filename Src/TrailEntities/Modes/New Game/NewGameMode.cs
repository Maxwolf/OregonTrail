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
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.NewGameMode" /> class.
        /// </summary>
        public NewGameMode()
        {
            // Basic information to start a new simulation.
            NewGameInfo = new NewGameInfo();

            // Menu items for creating new game.
            AddCommand(ChooseNames, NewGameCommands.ChooseNames, "Pick names for your party.");
            AddCommand(ChooseProfession, NewGameCommands.ChooseProfession, "Pick party leader profession.");
            AddCommand(BuyInitialItems, NewGameCommands.BuyInitialItems, "Buy initial items for journey.");
            AddCommand(ChooseStartMonth, NewGameCommands.ChooseStartMonth, "Decide when to set off on the trail.");
            AddCommand(StartGame, NewGameCommands.StartGame, "Starts a new journey on the trail!");

            // Since the user data has not been modified yet we are going to attach a state to start this process right away.
            ChooseNames();
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
        ///     Fired when the active game mode has been changed, this allows any underlying mode to know about a change in
        ///     simulation.
        /// </summary>
        /// <param name="modeType">Current mode which the simulation is changing to.</param>
        public override void OnModeChanged(ModeType modeType)
        {
            base.OnModeChanged(modeType);

            // If the user data player count is less than or equal to zero then just start off with name input.
            if (!NewGameInfo.Modified)
                ChooseNames();
        }

        /// <summary>
        ///     Allows the user to input the four unique names for each party member in their group. If they don't want to enter a
        ///     name they can just press enter and a random name will be selected from a small array of names in that state.
        /// </summary>
        public void ChooseNames()
        {
            CurrentState = new InputPlayerNameState(0, "Party leader name?", this, NewGameInfo);
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
            GameSimulationApp.Instance.StartGame(NewGameInfo);
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