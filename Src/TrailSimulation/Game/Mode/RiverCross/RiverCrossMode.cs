using System;
using System.Text;
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
        ///     Defines the current game mode the inheriting class is going to take responsibility for when attached to the
        ///     simulation.
        /// </summary>
        public override Mode Mode
        {
            get { return Mode.RiverCrossing; }
        }

        /// <summary>
        ///     Attached a state on top of the river crossing mode to explain what the different options mean and how they work.
        /// </summary>
        private void GetMoreInformation()
        {
            SetState(typeof (FordRiverHelpState));
        }

        /// <summary>
        ///     Waits for a day still ticking events but waiting to see if weather will improve and make crossing easier.
        /// </summary>
        private void WaitForWeather()
        {
            SetState(typeof (CampByRiverState));
        }

        /// <summary>
        ///     Prompts to pay monies for a ferry operator that will take the vehicle across the river without the danger of user
        ///     trying it themselves.
        /// </summary>
        private void UseFerry()
        {
            UserData.CrossingType = RiverCrossChoice.Ferry;
            SetState(typeof (UseFerryConfirmState));
        }

        /// <summary>
        ///     Attempts to float the vehicle over the river to the other side, there is a much higher chance for bad things to
        ///     happen.
        /// </summary>
        private void CaulkVehicle()
        {
            UserData.CrossingType = RiverCrossChoice.Caulk;
            SetState(typeof (CrossingResultState));
        }

        /// <summary>
        ///     Rides directly into the river without any special precautions, if it is greater than three feet of water the
        ///     vehicle will be submerged and highly damaged.
        /// </summary>
        private void FordRiver()
        {
            UserData.CrossingType = RiverCrossChoice.Ford;
            SetState(typeof (CrossingResultState));
        }

        /// <summary>
        ///     Called after the mode has been added to list of modes and made active.
        /// </summary>
        public override void OnModePostCreate()
        {
            // Header text for above menu comes from river crossing info object.
            var headerText = new StringBuilder();
            headerText.AppendLine("--------------------------------");
            headerText.AppendLine($"{GameSimulationApp.Instance.Trail.CurrentLocation.Name}");
            headerText.AppendLine($"{GameSimulationApp.Instance.Time.Date}");
            headerText.AppendLine("--------------------------------");
            headerText.AppendLine($"Weather: {GameSimulationApp.Instance.Climate.CurrentWeather}");
            headerText.AppendLine($"River width: {UserData.RiverWidth} feet");
            headerText.AppendLine($"River depth: {UserData.RiverDepth} feet");
            headerText.AppendLine("--------------------------------");
            headerText.Append("You may:");
            MenuHeader = headerText.ToString();

            // Add all of the commands for crossing a river.
            AddCommand(FordRiver, RiverCrossCommands.FordRiver);
            AddCommand(CaulkVehicle, RiverCrossCommands.CaulkVehicle);
            AddCommand(UseFerry, RiverCrossCommands.UseFerry);
            AddCommand(WaitForWeather, RiverCrossCommands.WaitForWeather);
            AddCommand(GetMoreInformation, RiverCrossCommands.GetMoreInformation);

            // Add the state that explains the player is at a river crossing and what is expected of them.
            SetState(typeof (RiverPromptState));
        }

        /// <summary>
        ///     Called when the mode manager in simulation makes this mode the currently active game mode. Depending on order of
        ///     modes this might not get called until the mode is actually ticked by the simulation.
        /// </summary>
        public override void OnModeActivate()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Fired when the simulation adds a game mode that is not this mode. Used to execute code in other modes that are not
        ///     the active mode anymore one last time.
        /// </summary>
        public override void OnModeAdded()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Fired when this game mode is removed from the list of available and ticked modes in the simulation.
        /// </summary>
        /// <param name="mode"></param>
        protected override void OnModeRemoved(Mode mode)
        {
            throw new NotImplementedException();
        }
    }
}