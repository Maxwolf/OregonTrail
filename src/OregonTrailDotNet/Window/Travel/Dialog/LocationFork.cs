// Created by Maxwolf (bigmaxwolf.com) 
// Timestamp 01/03/2016@1:50 AM

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OregonTrailDotNet.Entity.Location;
using OregonTrailDotNet.Entity.Location.Point;
using OregonTrailDotNet.Window.Travel.Command;
using OregonTrailDotNet.Window.Travel.Toll;
using WolfCurses.Window;
using WolfCurses.Window.Form;

namespace OregonTrailDotNet.Window.Travel.Dialog
{
    /// <summary>
    ///     Defines a location that has the player make a choice about the next location they want to travel to, it is not a
    ///     linear choice and depends on the player telling the simulation which way to fork down the path. The decisions are
    ///     pear shaped in the sense any fork will eventually lead back to the same path.
    /// </summary>
    [ParentWindow(typeof(Travel))]
    public sealed class LocationFork : Form<TravelInfo>
    {
        /// <summary>
        ///     Holds representation of the fork in the road as a decision for the player to make.
        /// </summary>
        private readonly StringBuilder _forkPrompt;

        /// <summary>
        ///     Defines the skip choices as they will be selected from the fork form. The purpose for this is because we want the
        ///     index for selecting them to start at one not zero.
        /// </summary>
        private Dictionary<int, Location> _skipChoices;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LocationFork" /> class.
        ///     This constructor will be used by the other one
        /// </summary>
        /// <param name="window">The window.</param>
        // ReSharper disable once UnusedMember.Global
        public LocationFork(IWindow window) : base(window)
        {
            _forkPrompt = new StringBuilder();
        }

        /// <summary>
        ///     Fired after the state has been completely attached to the simulation letting the state know it can browse the user
        ///     data and other properties below it.
        /// </summary>
        public override void OnFormPostCreate()
        {
            base.OnFormPostCreate();

            // Cast the current location as a fork in the road.
            var forkInRoad = GameSimulationApp.Instance.Trail.CurrentLocation as ForkInRoad;
            if (forkInRoad == null)
                throw new InvalidCastException("Unable to cast current location to fork in the road.");

            // Create a dictionary that represents all the choices with index starting at one not zero.
            _skipChoices = new Dictionary<int, Location>();
            for (var index = 0; index < forkInRoad.SkipChoices.Count; index++)
            {
                var skipChoice = forkInRoad.SkipChoices[index];
                _skipChoices.Add(index + 1, skipChoice);
            }
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
            // Clear the string builder and being building a new fork in the road based on current location skip choices.
            _forkPrompt.Clear();
            _forkPrompt.AppendLine($"{Environment.NewLine}The trail divides here. You may:{Environment.NewLine}");

            foreach (var skipChoice in _skipChoices)
                if (skipChoice.Key == _skipChoices.Last().Key)
                {
                    // Final skip choice and special option normally done when sizing up situation.
                    _forkPrompt.AppendLine($"  {skipChoice.Key}. head for {DestinationName(skipChoice.Value)}");
                    _forkPrompt.Append($"  {skipChoice.Key + 1}. see the map");
                }
                else
                {
                    // Standard skip location entry for the list.
                    _forkPrompt.AppendLine($"  {skipChoice.Key}. head for {DestinationName(skipChoice.Value)}");
                }

            // Rendering of the fork in the road as text user interface.
            return _forkPrompt.ToString();
        }

        /// <summary>
        ///     Name to show for a fork choice. A null choice is not a detour at all but the main trail itself, so it is named
        ///     after whatever location already comes next rather than after a branch that does not exist.
        /// </summary>
        /// <param name="skipChoice">The branch to name, or null for staying on the main trail.</param>
        /// <returns>Location name to render against this choice.</returns>
        private static string DestinationName(Location skipChoice)
        {
            if (skipChoice != null)
                return skipChoice.Name;

            var trail = GameSimulationApp.Instance.Trail;
            var nextIndex = trail.LocationIndex + 1;
            return nextIndex < trail.Locations.Count ? trail.Locations[nextIndex].Name : "the trail ahead";
        }

        /// <summary>Fired when the game Windows current state is not null and input buffer does not match any known command.</summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Parse the user input buffer as integer.
            if (!int.TryParse(input, out var parsedInputNumber))
                return;

            // Number must be greater than zero.
            if (parsedInputNumber <= 0)
                return;

            // Dictionary of skip choices must contain key with input number.
            if (_skipChoices.ContainsKey(parsedInputNumber))
            {
                var chosen = _skipChoices[parsedInputNumber];

                // Check if the selected fork is a toll road (that changes things).
                if (chosen is TollRoad tollRoad)
                {
                    // Creates a toll and adds location we would like to fork to.
                    UserData.GenerateToll(tollRoad);
                    SetForm(typeof(TollRoadQuestion));
                }
                else
                {
                    // Committing to a crossing that scores the party where they stand freezes their health here and now,
                    // before the journey to it can undo the rest they took to earn it.
                    if (chosen is Entity.Location.Point.RiverCrossing river && river.LocksPartyHealth)
                        GameSimulationApp.Instance.Vehicle.LockPartyHealth();

                    // A null choice means staying on the main trail, so there is no detour to splice in - the party just
                    // carries on to the location that already follows this one, over the leg the fork itself describes.
                    if (chosen != null)
                    {
                        GameSimulationApp.Instance.Trail.InsertLocation(chosen);

                        // The roads out of a fork are not the same length, so the leg set on arrival here is only right
                        // for staying on the main trail. Taking a branch means travelling that branch's own road.
                        if (chosen.LegDistance > 0)
                            GameSimulationApp.Instance.Trail.SetDistanceToNextLocation(chosen.LegDistance);
                    }

                    // Start going there...
                    SetForm(TravelInfo.DepartFormType);
                }
            }
            else
            {
                // Invalid selection will result in looking at the map screen.
                SetForm(Scene.MapScene.FormType);
            }
        }
    }
}