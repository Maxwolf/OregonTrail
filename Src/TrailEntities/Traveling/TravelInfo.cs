using System;
using System.Text;

namespace TrailEntities
{
    /// <summary>
    ///     Holds all the information about traveling that we want to know, such as how long we need to go until next point,
    ///     what our current mode is like moving, paused, etc.
    /// </summary>
    public sealed class TravelInfo
    {
        /// <summary>
        ///     Determines if the player has looked around at the location before prompting them with any decision making.
        /// </summary>
        public bool HasLookedAround { get; set; }

        //public bool CanHunt

        /// <summary>
        ///     Defines a string that represents the entire travel status of the vehicle and group that can be shown on all the
        ///     states when player is making decisions about rest, stopping at locations, dealing with random events that pause
        ///     simulation, etc.
        /// </summary>
        public string TravelStatus
        {
            get
            {
                var travelStatus = new StringBuilder();
                travelStatus.Append($"--------------------------------{Environment.NewLine}");
                travelStatus.Append($"{GameSimulationApp.Instance.Trail.GetCurrentPointOfInterest()?.Name}{Environment.NewLine}");
                travelStatus.Append($"{GameSimulationApp.Instance.Time.Date}{Environment.NewLine}");
                travelStatus.Append($"--------------------------------{Environment.NewLine}");
                travelStatus.Append($"Weather: {GameSimulationApp.Instance.Climate.CurrentWeather}{Environment.NewLine}");
                travelStatus.Append($"Health: {GameSimulationApp.Instance.Vehicle.RepairStatus}{Environment.NewLine}");
                travelStatus.Append($"Pace: {GameSimulationApp.Instance.Vehicle.Pace}{Environment.NewLine}");
                travelStatus.Append($"Rations: {GameSimulationApp.Instance.Vehicle.Ration}{Environment.NewLine}");
                travelStatus.Append($"--------------------------------{Environment.NewLine}");
                return travelStatus.ToString();
            }
        }
    }
}