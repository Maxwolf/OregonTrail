using System;
using System.Text;
using TrailSimulation.Core;
using TrailSimulation.Entity;
using TrailSimulation.Event;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Runs the player over the river based on the crossing information. Depending on what happens a message will be
    ///     printed to the screen explaining what happened before defaulting back to travel game Windows.
    /// </summary>
    [ParentWindow(SimulationModule.Travel)]
    public sealed class CrossingResult : Form<TravelInfo>
    {
        /// <summary>
        ///     String builder that will hold all the data about our river crossing as it occurs.
        /// </summary>
        private StringBuilder _crossingResult;

        /// <summary>
        ///     Determines if this state has performed it's duties and helped get the players and their vehicle across the river.
        /// </summary>
        private bool _finishedCrossingRiver;

        /// <summary>
        ///     Determines if the wagon has already flooded manually because river was to deep.
        /// </summary>
        private bool _hasFlooded;

        /// <summary>
        ///     Animated sway bar that prints out as text, ping-pongs back and fourth between left and right side, moved by
        ///     stepping it with tick.
        /// </summary>
        private MarqueeBar _marqueeBar;

        /// <summary>
        ///     Defines the current amount of feet we have crossed of the river, this will tick up to the total length of the
        ///     river.
        /// </summary>
        private int _riverCrossingOfTotalWidth;

        /// <summary>
        ///     Holds the text related to animated sway bar, each tick of simulation steps it.
        /// </summary>
        private string _swayBarText;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public CrossingResult(IWindow gameMode) : base(gameMode)
        {
            // Create the string builder for holding all our text about river crossing as it happens.
            _crossingResult = new StringBuilder();

            // Animated sway bar.
            _marqueeBar = new MarqueeBar();
            _swayBarText = _marqueeBar.Step();

            // Sets the crossing percentage to zero.
            _riverCrossingOfTotalWidth = 0;
            _finishedCrossingRiver = false;
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public override bool InputFillsBuffer
        {
            // Input buffer is never filled because player cannot make choices here.
            get { return false; }
        }

        /// <summary>
        ///     Determines if this dialog state is allowed to receive any input at all, even empty line returns. This is useful for
        ///     preventing the player from leaving a particular dialog until you are ready or finished processing some data.
        /// </summary>
        public override bool AllowInput
        {
            get { return _finishedCrossingRiver; }
        }

        /// <summary>
        ///     Fired after the state has been completely attached to the simulation letting the state know it can browse the user
        ///     data and other properties below it.
        /// </summary>
        public override void OnFormPostCreate()
        {
            base.OnFormPostCreate();

            // Park the vehicle if it is not somehow by now.
            GameSimulationApp.Instance.Vehicle.Status = VehicleStatus.Stopped;

            // Remove the monies from the player for ferry trip.
            var oldMoney = GameSimulationApp.Instance.Vehicle.Inventory[Entities.Cash];
            GameSimulationApp.Instance.Vehicle.Inventory[Entities.Cash] =
                new SimItem(oldMoney, (int) (oldMoney.TotalValue - UserData.River.FerryCost));

            // Clear out the cost for the ferry since it has been paid for now.
            UserData.River.FerryCost = 0;
        }

        /// <summary>
        ///     Returns a text only representation of the current game Windows state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderForm()
        {
            // Clears the string buffer for this render pass.
            _crossingResult.Clear();

            // Ping-pong progress bar to show that we are moving.
            _crossingResult.AppendLine($"{Environment.NewLine}{_swayBarText}");

            // Shows basic status of vehicle and total river crossing percentage.
            _crossingResult.AppendLine("--------------------------------");
            _crossingResult.AppendLine($"{GameSimulationApp.Instance.Trail.CurrentLocation.Name}");
            _crossingResult.AppendLine($"{GameSimulationApp.Instance.Time.Date}");
            _crossingResult.AppendLine(
                $"Weather: {GameSimulationApp.Instance.Climate.CurrentWeather.ToDescriptionAttribute()}");
            _crossingResult.AppendLine($"Health: {GameSimulationApp.Instance.Vehicle.RepairLevel}");
            _crossingResult.AppendLine($"Crossing By: {UserData.River.CrossingType}");
            _crossingResult.AppendLine(
                $"River width: {UserData.River.RiverWidth.ToString("N0")} feet");
            _crossingResult.AppendLine(
                $"River crossed: {_riverCrossingOfTotalWidth.ToString("N0")} feet");
            _crossingResult.AppendLine("--------------------------------");

            if (_finishedCrossingRiver)
                _crossingResult.AppendLine(InputManager.PRESS_ENTER);

            return _crossingResult.ToString();
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
        public override void OnTick(bool systemTick)
        {
            base.OnTick(systemTick);

            // Skip system ticks.
            if (systemTick)
                return;

            // Stop crossing if we have finished.
            if (_finishedCrossingRiver)
                return;

            // Advance the progress bar, step it to next phase.
            _swayBarText = _marqueeBar.Step();

            // Increment the amount we have floated over the river.
            _riverCrossingOfTotalWidth += GameSimulationApp.Instance.Random.Next(1, (UserData.River.RiverWidth/4));

            // Check to see if we will finish crossing river before crossing more.
            if (_riverCrossingOfTotalWidth >= UserData.River.RiverWidth)
            {
                _riverCrossingOfTotalWidth = UserData.River.RiverWidth;
                _finishedCrossingRiver = true;
                return;
            }

            // Attempt to throw a random event related to some failure happening with river crossing.
            switch (UserData.River.CrossingType)
            {
                case RiverCrossChoice.None:
                    break;
                case RiverCrossChoice.Ford:
                    // If river is deeper than a few feet and you ford it you will get flooded every time at least once.
                    if (UserData.River.RiverDepth > 3 && !_hasFlooded &&
                        _riverCrossingOfTotalWidth >= (UserData.River.RiverWidth/2))
                    {
                        _hasFlooded = true;
                        GameSimulationApp.Instance.EventDirector.TriggerEvent(GameSimulationApp.Instance.Vehicle,
                            typeof (VehicleWashedOutEvent));
                        return;
                    }

                    GameSimulationApp.Instance.EventDirector.TriggerEventByType(GameSimulationApp.Instance.Vehicle,
                        EventCategory.RiverFord);
                    break;
                case RiverCrossChoice.Float:
                    GameSimulationApp.Instance.EventDirector.TriggerEventByType(GameSimulationApp.Instance.Vehicle,
                        EventCategory.RiverFloat);
                    break;
                case RiverCrossChoice.Ferry:
                    break;
                case RiverCrossChoice.WaitForWeather:
                    break;
                case RiverCrossChoice.GetMoreInformation:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Fired when the game Windows current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game Windows.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Skip if we are still crossing the river.
            if (_riverCrossingOfTotalWidth < UserData.River.RiverWidth)
                return;

            // River crossing takes you a day.
            GameSimulationApp.Instance.TakeTurn();

            // Start going there...
            SetForm(typeof (LocationDepart));
        }
    }
}