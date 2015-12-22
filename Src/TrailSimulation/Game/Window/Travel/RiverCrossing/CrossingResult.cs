// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrossingResult.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace TrailSimulation.Game
{
    using System;
    using System.Text;
    using Core;
    using Event;

    /// <summary>
    ///     Displays the final crossing result for the river crossing location. No matter what choice the player made, what
    ///     events happen along the way, this final screen will be shown to let the user know how the last leg of the journey
    ///     went. It is possible to get stuck in the mud, however most of the messages are safe and just let the user know they
    ///     finally made it across.
    /// </summary>
    [ParentWindow(GameWindow.Travel)]
    public sealed class CrossingResult : InputForm<TravelInfo>
    {
        /// <summary>
        ///     The crossing result.
        /// </summary>
        private StringBuilder _crossingResult;

        /// <summary>Initializes a new instance of the <see cref="CrossingResult"/> class.
        ///     This constructor will be used by the other one</summary>
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
                        _crossingResult.AppendLine("stuck.");
                    }
                    else
                    {
                        // Triggers event for muddy shore that makes player lose a day.
                        GameSimulationApp.Instance.EventDirector.TriggerEvent(null, typeof (StuckInMud));
                    }
                    break;
                case RiverCrossChoice.Float:
                    _crossingResult.AppendLine($"{Environment.NewLine}You had no trouble");
                    _crossingResult.AppendLine("floating the wagon");
                    _crossingResult.AppendLine($"across.{Environment.NewLine}");
                    break;
                case RiverCrossChoice.Ferry:
                    _crossingResult.AppendLine($"{Environment.NewLine}The ferry got your party");
                    _crossingResult.AppendLine($"and wagon safely across.{Environment.NewLine}");
                    break;
                case RiverCrossChoice.Indian:
                    _crossingResult.AppendLine($"{Environment.NewLine}The Indian helped your");
                    _crossingResult.AppendLine($"wagon safely across.{Environment.NewLine}");
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

        /// <summary>Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.</summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
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