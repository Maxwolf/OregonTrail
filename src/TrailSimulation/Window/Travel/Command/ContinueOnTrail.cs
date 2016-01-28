// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/01/2016@7:40 PM

namespace TrailSimulation
{
    using System;
    using System.Text;
    using WolfCurses;
    using WolfCurses.Control;
    using WolfCurses.Form;

    /// <summary>
    ///     Attached to the travel Windows when the player requests to continue on the trail. This shows a ping-pong progress
    ///     bar moving back and fourth which lets the player know they are moving. Stats are also shown from the travel info
    ///     object, if any random events occur they will be selected from this state.
    /// </summary>
    [ParentWindow(typeof (Travel))]
    public sealed class ContinueOnTrail : Form<TravelInfo>
    {
        /// <summary>
        ///     Holds the current drive state, since we can size up the situation at any time.
        /// </summary>
        private StringBuilder _drive;

        /// <summary>
        ///     Animated sway bar that prints out as text, ping-pongs back and fourth between left and right side, moved by
        ///     stepping it with tick.
        /// </summary>
        private MarqueeBar _marqueeBar;

        /// <summary>
        ///     Holds the text related to animated sway bar, each tick of simulation steps it.
        /// </summary>
        private string _swayBarText;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContinueOnTrail" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        public ContinueOnTrail(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public override bool InputFillsBuffer
        {
            get { return false; }
        }

        /// <summary>
        ///     Fired after the state has been completely attached to the simulation letting the state know it can browse the user
        ///     data and other properties below it.
        /// </summary>
        public override void OnFormPostCreate()
        {
            base.OnFormPostCreate();

            // Get instance of game simulation for easy reading.
            var game = GameSimulationApp.Instance;

            // We don't create it in the constructor, will update with ticks.
            _drive = new StringBuilder();

            // Animated sway bar.
            _marqueeBar = new MarqueeBar();
            _swayBarText = _marqueeBar.Step();

            // Vehicle has departed the current location for the next one but you can only depart once.
            if (game.Trail.DistanceToNextLocation > 0 &&
                game.Trail.CurrentLocation.Status == LocationStatus.Arrived)
                game.Trail.CurrentLocation.Status = LocationStatus.Departed;
        }

        /// <summary>
        ///     Returns a text only representation of the current game Windows state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public override string OnRenderForm()
        {
            // Clear whatever was in the string builder last tick.
            _drive.Clear();

            // Ping-pong progress bar to show that we are moving.
            _drive.AppendLine($"{Environment.NewLine}{_swayBarText}");

            // Basic information about simulation.
            _drive.AppendLine(TravelInfo.DriveStatus);

            // Don't add the RETURN KEY text here if we are not actually at a point.
            _drive.Append("Press ENTER to size up the situation");

            // Wait for user input, event, or reaching next location...
            return _drive.ToString();
        }

        /// <summary>
        ///     Called when the simulation is ticked by underlying operating system, game engine, or potato. Each of these system
        ///     ticks is called at unpredictable rates, however if not a system tick that means the simulation has processed enough
        ///     of them to fire off event for fixed interval that is set in the core simulation by constant in milliseconds.
        /// </summary>
        /// <remarks>Default is one second or 1000ms.</remarks>
        /// <param name="systemTick">
        ///     TRUE if ticked unpredictably by underlying operating system, game engine, or potato. FALSE if
        ///     pulsed by game simulation at fixed interval.
        /// </param>
        /// <param name="skipDay">
        ///     Determines if the simulation has force ticked without advancing time or down the trail. Used by
        ///     special events that want to simulate passage of time without actually any actual time moving by.
        /// </param>
        public override void OnTick(bool systemTick, bool skipDay)
        {
            base.OnTick(systemTick, skipDay);

            // Only game simulation ticks please.
            if (systemTick)
                return;

            // Get instance of game simulation for easy reading.
            var game = GameSimulationApp.Instance;

            // Checks if the player has animals to pull their vehicle.
            game.Vehicle.Status = game.Vehicle.Inventory[Entities.Animal].Quantity <= 0
                ? VehicleStatus.Stuck
                : VehicleStatus.Moving;

            // Determine if we should continue down the trail based on current vehicle status.
            switch (game.Vehicle.Status)
            {
                case VehicleStatus.Stopped:
                    return;
                case VehicleStatus.Stuck:
                case VehicleStatus.Broken:
                    // Stuck or broken vehicles are unable to continue the journey.
                    SetForm(typeof (UnableToContinue));
                    break;
                case VehicleStatus.Moving:
                    // Check if there is a tombstone here, if so we attach question form that asks if we stop or not.
                    _swayBarText = _marqueeBar.Step();
                    if (game.Tombstone.ContainsTombstone(game.Vehicle.Odometer) && !game.Trail.CurrentLocation.ArrivalFlag)
                    {
                        SetForm(typeof (TombstoneQuestion));
                        return;
                    }

                    // Processes the next turn in the game simulation.
                    game.TakeTurn(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>Fired when the game Windows current state is not null and input buffer does not match any known command.</summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game Windows.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Can only stop the simulation if it is actually running.
            if (!string.IsNullOrEmpty(input))
                return;

            // Stop ticks and close this state.
            GameSimulationApp.Instance.Vehicle.Status = VehicleStatus.Stopped;
            ClearForm();
        }
    }
}