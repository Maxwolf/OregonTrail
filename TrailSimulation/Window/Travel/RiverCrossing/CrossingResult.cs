// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 01/01/2016@3:27 AM

namespace TrailSimulation.Window.Travel.RiverCrossing
{
    using System;
    using System.Text;
    using Dialog;
    using Event.Vehicle;
    using WolfCurses.Window;
    using WolfCurses.Window.Form;
    using WolfCurses.Window.Form.Input;

    /// <summary>
    ///     Displays the final crossing result for the river crossing location. No matter what choice the player made, what
    ///     events happen along the way, this final screen will be shown to let the user know how the last leg of the journey
    ///     went. It is possible to get stuck in the mud, however most of the messages are safe and just let the user know they
    ///     finally made it across.
    /// </summary>
    [ParentWindow(typeof (Travel))]
    public sealed class CrossingResult : InputForm<TravelInfo>
    {
        /// <summary>
        ///     The crossing result.
        /// </summary>
        private StringBuilder _crossingResult;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CrossingResult" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        public CrossingResult(IWindow window) : base(window)
        {
            _crossingResult = new StringBuilder();
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        /// <returns>
        ///     The text user interface.<see cref="string" />.
        /// </returns>
        protected override string OnDialogPrompt()
        {
            // Clear any previous crossing result prompt.
            _crossingResult.Clear();

            // Depending on crossing type we will say different things about the crossing.
            switch (UserData.River.CrossingType)
            {
                case RiverCrossChoice.Ford:
                    if (GameSimulationApp.Instance.Random.NextBool())
                    {
                        // No loss in time, but warning to let the player know it's dangerous.
                        _crossingResult.AppendLine($"{Environment.NewLine}It was a muddy crossing,");
                        _crossingResult.AppendLine("but you did not get");
                        _crossingResult.AppendLine($"stuck.{Environment.NewLine}");
                    }
                    else
                    {
                        // Triggers event for muddy shore that makes player lose a day, forces end of crossing also.
                        FinishCrossing();
                        GameSimulationApp.Instance.EventDirector.TriggerEvent(GameSimulationApp.Instance.Vehicle,
                            typeof (StuckInMud));
                    }

                    break;
                case RiverCrossChoice.Float:
                    if (UserData.River.DisasterHappened)
                    {
                        _crossingResult.AppendLine($"{Environment.NewLine}Your party relieved");
                        _crossingResult.AppendLine("to reach other side after");
                        _crossingResult.AppendLine($"trouble floating across.{Environment.NewLine}");
                    }
                    else
                    {
                        _crossingResult.AppendLine($"{Environment.NewLine}You had no trouble");
                        _crossingResult.AppendLine("floating the wagon");
                        _crossingResult.AppendLine($"across.{Environment.NewLine}");
                    }

                    break;
                case RiverCrossChoice.Ferry:
                    if (UserData.River.DisasterHappened)
                    {
                        _crossingResult.AppendLine($"{Environment.NewLine}The ferry operator");
                        _crossingResult.AppendLine("apologizes for the");
                        _crossingResult.AppendLine($"rough ride.{Environment.NewLine}");
                    }
                    else
                    {
                        _crossingResult.AppendLine($"{Environment.NewLine}The ferry got your party");
                        _crossingResult.AppendLine($"and wagon safely across.{Environment.NewLine}");
                    }

                    break;
                case RiverCrossChoice.Indian:
                    if (UserData.River.DisasterHappened)
                    {
                        _crossingResult.AppendLine($"{Environment.NewLine}The Indian runs away");
                        _crossingResult.AppendLine("as soon as you");
                        _crossingResult.AppendLine($"reach the shore.{Environment.NewLine}");
                    }
                    else
                    {
                        _crossingResult.AppendLine($"{Environment.NewLine}The Indian helped your");
                        _crossingResult.AppendLine($"wagon safely across.{Environment.NewLine}");
                    }

                    break;
                case RiverCrossChoice.None:
                case RiverCrossChoice.WaitForWeather:
                case RiverCrossChoice.GetMoreInformation:
                    throw new InvalidOperationException(
                        $"Invalid river crossing result choice {UserData.River.CrossingType}.");
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Render the crossing result to text user interface.
            return _crossingResult.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            FinishCrossing();
        }

        /// <summary>
        ///     Cleans up any remaining data about this river the player just crossed.
        /// </summary>
        private void FinishCrossing()
        {
            // Destroy the river data now that we are done with it.
            UserData.DestroyRiver();

            // River crossing takes you a day.
            GameSimulationApp.Instance.TakeTurn(false);

            // Start going there...
            SetForm(typeof (LocationDepart));
        }
    }
}