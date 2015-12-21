// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RiverCross.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace TrailSimulation.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Core;

    /// <summary>
    ///     Manages a boolean event where the player needs to make a choice before they can move onto the next location on the
    ///     trail. Depending on the outcome of this event the player party may lose items, people, or parts depending on how
    ///     bad it is.
    /// </summary>
    [ParentWindow(GameWindow.Travel)]
    public sealed class RiverCross : Form<TravelInfo>
    {
        /// <summary>
        ///     Reference mappings for all the choices the player can make on this form. Since we add and remove items depending on
        ///     type of river crossing in the middle of the choices we need to have a correct mapping of integers to their
        ///     respective enumeration values. There is another dictionary for mapping enumeration values to actions.
        /// </summary>
        private Dictionary<string, RiverCrossChoice> _choiceMappings;

        /// <summary>
        ///     Holds reference to all of the choices that can be made in the river, since the are dynamic and change based on the
        ///     location we are visiting this dictionary facilitates the ability for lookups and checking of inputted key for
        ///     validity. If key is found then the appropriate action can be invoked.
        /// </summary>
        private Dictionary<RiverCrossChoice, Action> _riverActions;

        /// <summary>
        ///     Holds all the information about the river and crossing decisions so it only needs to be constructed once at
        ///     startup.
        /// </summary>
        private StringBuilder _riverInfo;

        /// <summary>Initializes a new instance of the <see cref="RiverCross"/> class.
        ///     This constructor will be used by the other one</summary>
        /// <param name="window">The window.</param>
        public RiverCross(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Fired after the state has been completely attached to the simulation letting the state know it can browse the user
        ///     data and other properties below it.
        /// </summary>
        public override void OnFormPostCreate()
        {
            base.OnFormPostCreate();

            // Grab instance of the game simulation.
            var game = GameSimulationApp.Instance;

            // Re-create the mappings and text information on post create each time.
            _choiceMappings = new Dictionary<string, RiverCrossChoice>();
            _riverActions = new Dictionary<RiverCrossChoice, Action>();
            _riverInfo = new StringBuilder();

            // Header text for above menu comes from river crossing info object.
            _riverInfo.AppendLine("--------------------------------");
            _riverInfo.AppendLine($"{game.Trail.CurrentLocation.Name}");
            _riverInfo.AppendLine($"{game.Time.Date}");
            _riverInfo.AppendLine("--------------------------------");
            _riverInfo.AppendLine(
                $"Weather: {game.Trail.CurrentLocation.Weather.ToDescriptionAttribute()}");
            _riverInfo.AppendLine($"River width: {UserData.River.RiverWidth.ToString("N0")} feet");
            _riverInfo.AppendLine($"River depth: {UserData.River.RiverDepth.ToString("N0")} feet");
            _riverInfo.AppendLine("--------------------------------");
            _riverInfo.AppendLine($"You may:{Environment.NewLine}");

            // Loop through all the river choice commands and print them out for the state.
            var choices = new List<RiverCrossChoice>(Enum.GetValues(typeof (RiverCrossChoice)).Cast<RiverCrossChoice>());
            for (var index = 1; index < choices.Count; index++)
            {
                // Get the current river choice enumeration value we casted into list.
                var riverChoice = choices[index];

                // Add the mapping for text user interface mapping to enumeration value for action invoking below.
                _choiceMappings.Add(index.ToString(), riverChoice);

                // Last line should not print new line.
                if (index == (choices.Count - 1))
                {
                    _riverInfo.Append(index + ". " + riverChoice.ToDescriptionAttribute());
                }
                else
                {
                    _riverInfo.AppendLine(index + ". " + riverChoice.ToDescriptionAttribute());
                }

                // Depending on selection made we will decide on what to do.
                switch (riverChoice)
                {
                    case RiverCrossChoice.Ford:
                        _riverActions.Add(riverChoice, delegate
                        {
                            // Driving straight into the river and hoping you don't drown.
                            UserData.River.CrossingType = RiverCrossChoice.Ford;
                            SetForm(typeof (CrossingTick));
                        });
                        break;
                    case RiverCrossChoice.Float:
                        _riverActions.Add(riverChoice, delegate
                        {
                            // Floating wagon manually without any help.
                            UserData.River.CrossingType = RiverCrossChoice.Float;
                            SetForm(typeof (CrossingTick));
                        });
                        break;
                    case RiverCrossChoice.Ferry:
                        _riverActions.Add(riverChoice, delegate
                        {
                            // Ferry operator charges money and time before player can cross.
                            UserData.River.CrossingType = RiverCrossChoice.Ferry;
                            SetForm(typeof (UseFerryConfirm));
                        });
                        break;
                    case RiverCrossChoice.Indian:
                        _riverActions.Add(riverChoice, delegate
                        {
                            // Indian guide helps float wagon across river for sets of clothing.
                            UserData.River.CrossingType = RiverCrossChoice.Indian;
                            SetForm(typeof (IndianGuidePrompt));
                        });
                        break;
                    case RiverCrossChoice.WaitForWeather:
                        _riverActions.Add(riverChoice, delegate
                        {
                            // Resting by a river only increments a single day at a time.
                            UserData.DaysToRest = 1;
                            UserData.River.CrossingType = RiverCrossChoice.WaitForWeather;
                            SetForm(typeof (Resting));
                        });
                        break;
                    case RiverCrossChoice.GetMoreInformation:
                        _riverActions.Add(riverChoice, delegate
                        {
                            UserData.River.CrossingType = RiverCrossChoice.GetMoreInformation;
                            SetForm(typeof (FordRiverHelp));
                        });
                        break;
                    case RiverCrossChoice.None:
                        throw new ArgumentException(
                            "Unable to use river cross choice NONE as a selection since it is only intended for initialization.");
                    default:
                        throw new ArgumentOutOfRangeException(nameof(riverChoice), 
                            "Unable to cast river cross choice into a valid selection for river crossing.");
                }
            }
        }

        /// <summary>
        ///     Returns a text only representation of the current game Windows state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        /// <returns>
        ///     The river crossing text user interface.<see cref="_riverInfo" />.
        /// </returns>
        public override string OnRenderForm()
        {
            return _riverInfo.ToString();
        }

        /// <summary>Fired when the game Windows current state is not null and input buffer does not match any known command.</summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game Windows.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Skip if the input is null or empty.
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
                return;

            // Skip if the input does not match any known mapping for river choice.
            if (!_choiceMappings.ContainsKey(input))
                return;

            // Check if the river cross choice exists in the dictionary of choices valid for this river crossing location.
            if (!_riverActions.ContainsKey(_choiceMappings[input]))
                return;

            // Invoke the anonymous delegate method that was created when this form was attached.
            _riverActions[_choiceMappings[input]].Invoke();
        }
    }
}