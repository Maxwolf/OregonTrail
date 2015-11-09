using TrailEntities.Mode;

namespace TrailEntities.Game.RiverCross
{
    /// <summary>
    ///     Manages a boolean event where the player needs to make a choice before they can move onto the next location on the
    ///     trail. Depending on the outcome of this event the player party may lose items, people, or parts depending on how
    ///     bad it is.
    /// </summary>
    public sealed class RiverCrossGameMode : ModeProduct
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.ModeProduct" /> class.
        /// </summary>
        public RiverCrossGameMode() : base(false)
        {
            // Create new river crossing information object.
            RiverCrossInfo = new RiverCrossInfo();

            // Add all of the commands for crossing a river.
            AddCommand(FordRiver, RiverCrossCommands.FordRiver);
            AddCommand(CaulkVehicle, RiverCrossCommands.CaulkVehicle);
            AddCommand(UseFerry, RiverCrossCommands.UseFerry);
            AddCommand(WaitForWeather, RiverCrossCommands.WaitForWeather);
            AddCommand(GetMoreInformation, RiverCrossCommands.GetMoreInformation);
        }

        /// <summary>
        ///     Defines all of the information like river depth and ferry cost which will be randomly generated when this object is
        ///     created.
        /// </summary>
        private RiverCrossInfo RiverCrossInfo { get; }

        /// <summary>
        ///     Defines the current game gameMode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override GameMode ModeType
        {
            get { return GameMode.RiverCrossing; }
        }

        /// <summary>
        ///     Attached a state on top of the river crossing gameMode to explain what the different options mean and how they work.
        /// </summary>
        private void GetMoreInformation()
        {
            AddState(typeof (FordRiverHelpState));
        }

        /// <summary>
        ///     Waits for a day still ticking events but waiting to see if weather will improve and make crossing easier.
        /// </summary>
        private void WaitForWeather()
        {
            AddState(typeof (CampByRiverState));
        }

        /// <summary>
        ///     Prompts to pay monies for a ferry operator that will take the vehicle across the river without the danger of user
        ///     trying it themselves.
        /// </summary>
        private void UseFerry()
        {
            RiverCrossInfo.CrossingType = RiverCrossChoice.Ferry;
            AddState(typeof (UseFerryConfirmState));
        }

        /// <summary>
        ///     Attempts to float the vehicle over the river to the other side, there is a much higher chance for bad things to
        ///     happen.
        /// </summary>
        private void CaulkVehicle()
        {
            RiverCrossInfo.CrossingType = RiverCrossChoice.Caulk;
            AddState(typeof (CrossingResultState));
        }

        /// <summary>
        ///     Rides directly into the river without any special precautions, if it is greater than three feet of water the
        ///     vehicle will be submerged and highly damaged.
        /// </summary>
        private void FordRiver()
        {
            RiverCrossInfo.CrossingType = RiverCrossChoice.Ford;
            AddState(typeof (CrossingResultState));
        }

        /// <summary>
        ///     Fired by game simulation system timers timer which runs on same thread, only fired for active (last added), or
        ///     top-most game gameMode.
        /// </summary>
        public override void TickMode()
        {
        }

        /// <summary>
        ///     Fired when this game gameMode is removed from the list of available and ticked GameMode in the simulation.
        /// </summary>
        /// <param name="modeType"></param>
        protected override void OnModeRemoved(GameMode modeType)
        {
        }
    }
}