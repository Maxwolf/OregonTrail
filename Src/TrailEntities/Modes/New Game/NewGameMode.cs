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
            NewGameInfo = new NewGameInfo();

            // Menu items.
            AddCommand(ChooseNames, NewGameCommands.ChooseNames, "Pick names for your party.");
            AddCommand(ChooseProfession, NewGameCommands.ChooseProfession, "Pick party leader profession.");
            AddCommand(BuyInitialItems, NewGameCommands.BuyInitialItems, "Buy initial items for journey.");
            AddCommand(StartGame, NewGameCommands.StartGame, "Starts a new journey on the trail!");
        }

        public override ModeType ModeType
        {
            get { return ModeType.NewGame; }
        }

        public void ChooseNames()
        {
            CurrentState = new ChooseNamesState(this, NewGameInfo);
        }

        /// <summary>
        ///     Default values for new game.
        /// </summary>
        public NewGameInfo NewGameInfo { get; set; }

        public void ChooseProfession()
        {
            CurrentState = new ChooseProfessionState(this, NewGameInfo);
        }

        public void BuyInitialItems()
        {
            CurrentState = new BuyInitialItemsState(this, NewGameInfo);
        }

        public void StartGame()
        {
            CurrentState = new StartGameState(this, NewGameInfo);
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