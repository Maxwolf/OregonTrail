using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Manages a boolean event where the player needs to make a choice before they can move onto the next location on the
    ///     trail. Depending on the outcome of this event the player party may lose items, people, or parts depending on how
    ///     bad it is.
    /// </summary>
    public sealed class RiverCrossMode : ModeProduct<RiverCrossCommands, RiverCrossInfo>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.ModeProduct" /> class.
        /// </summary>
        public RiverCrossMode()
        {
            // Add all of the commands for crossing a river.
            AddCommand(FordRiver, RiverCrossCommands.FordRiver);
            AddCommand(CaulkVehicle, RiverCrossCommands.CaulkVehicle);
            AddCommand(UseFerry, RiverCrossCommands.UseFerry);
            AddCommand(WaitForWeather, RiverCrossCommands.WaitForWeather);
            AddCommand(GetMoreInformation, RiverCrossCommands.GetMoreInformation);

            // Add the state that explains the player is at a river crossing and what is expected of them.
            //CurrentState = new RiverPromptState(this, RiverCrossInfo);
            SetState(typeof (RiverPromptState));
        }

        /// <summary>
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override GameMode GameMode
        {
            get { return GameMode.RiverCrossing; }
        }

        /// <summary>
        ///     Attached a state on top of the river crossing mode to explain what the different options mean and how they work.
        /// </summary>
        private void GetMoreInformation()
        {
            //CurrentState = new FordRiverHelpState(this, RiverCrossInfo);
            SetState(typeof (FordRiverHelpState));
        }

        /// <summary>
        ///     Waits for a day still ticking events but waiting to see if weather will improve and make crossing easier.
        /// </summary>
        private void WaitForWeather()
        {
            //CurrentState = new CampByRiverState(this, RiverCrossInfo);
            SetState(typeof (CampByRiverState));
        }

        /// <summary>
        ///     Prompts to pay monies for a ferry operator that will take the vehicle across the river without the danger of user
        ///     trying it themselves.
        /// </summary>
        private void UseFerry()
        {
            UserData.CrossingType = RiverCrossChoice.Ferry;
            //CurrentState = new UseFerryConfirmState(this, RiverCrossInfo);
            SetState(typeof (UseFerryConfirmState));
        }

        /// <summary>
        ///     Attempts to float the vehicle over the river to the other side, there is a much higher chance for bad things to
        ///     happen.
        /// </summary>
        private void CaulkVehicle()
        {
            UserData.CrossingType = RiverCrossChoice.Caulk;
            //CurrentState = new CrossingResultState(this, RiverCrossInfo);
            SetState(typeof (CrossingResultState));
        }

        /// <summary>
        ///     Rides directly into the river without any special precautions, if it is greater than three feet of water the
        ///     vehicle will be submerged and highly damaged.
        /// </summary>
        private void FordRiver()
        {
            UserData.CrossingType = RiverCrossChoice.Ford;
            //CurrentState = new CrossingResultState(this, RiverCrossInfo);
            SetState(typeof (CrossingResultState));
        }

        /// <summary>
        ///     Fired by game simulation system timers timer which runs on same thread, only fired for active (last added), or
        ///     top-most game mode.
        /// </summary>
        public override void TickMode()
        {
        }

        /// <summary>
        ///     Fired when this game mode is removed from the list of available and ticked modes in the simulation.
        /// </summary>
        /// <param name="gameMode"></param>
        protected override void OnModeRemoved(GameMode gameMode)
        {
        }
    }
}