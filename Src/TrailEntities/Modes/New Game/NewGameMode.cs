using TrailCommon;

namespace TrailEntities
{
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
            AddCommand(StartGame, NewGameCommands.StartGame, "Starts a new journey on the trail!");

            // Start off by inputting first user name (party leader).
            ChooseNames();
        }

        public override ModeType ModeType
        {
            get { return ModeType.NewGame; }
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public override bool AcceptsInput
        {
            get { return true; }
        }

        public void ChooseNames()
        {
            CurrentState = new InputPlayerNameState(0, "Party leader name?", this, NewGameInfo);
        }

        /// <summary>
        ///     Default values for new game.
        /// </summary>
        public NewGameInfo NewGameInfo { get; set; }

        public void ChooseProfession()
        {
            CurrentState = new SelectProfessionState(this, NewGameInfo);
        }

        public void BuyInitialItems()
        {
            CurrentState = new BuyInitialItemsState(this, NewGameInfo);
        }

        public void StartGame()
        {
            // Tells running simulation to take data we have fed it and begin trip down the trail path.
            GameSimulationApp.Instance.StartGame();
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