using System;
using System.Linq;
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
        ///     Used when the player is traveling on the trail between locations. Also known as drive state in travel game mode.
        /// </summary>
        public string DriveStatus
        {
            get
            {
                // Get the current food item from vehicle inventory.
                var foodItem = GameSimulationApp.Instance.Vehicle.Inventory.FirstOrDefault(i => i is Food);

                // Set default food status text, update to actual food item total weight if it exists.
                var foodStatus = "0 pounds";
                if (foodItem != null)
                    foodStatus = $"{foodItem.TotalWeight} pounds";

                // Build up the status for the vehicle as it moves through the simulation.
                var driveStatus = new StringBuilder();
                driveStatus.Append($"--------------------------------{Environment.NewLine}");
                driveStatus.Append($"Date: {GameSimulationApp.Instance.Time.Date}{Environment.NewLine}");
                driveStatus.Append($"Weather: {GameSimulationApp.Instance.Climate.CurrentWeather}{Environment.NewLine}");
                driveStatus.Append($"Health: {GameSimulationApp.Instance.Vehicle.RepairStatus}{Environment.NewLine}");
                driveStatus.Append($"Food: {foodStatus}{Environment.NewLine}");
                driveStatus.Append(
                    $"Next landmark: {GameSimulationApp.Instance.Trail.DistanceToNextPoint} miles{Environment.NewLine}");
                driveStatus.Append(
                    $"Miles traveled: {GameSimulationApp.Instance.Vehicle.Odometer} miles{Environment.NewLine}");
                driveStatus.Append($"--------------------------------{Environment.NewLine}");
                return driveStatus.ToString();
            }
        }

        /// <summary>
        ///     Used when the player decides to stop their vehicle midway between two location points on the trail. When this
        ///     happens we do not want to display the location name since there are not at one and considered "on the trail"
        ///     however the simulation already knows what the next place is and everything about it so we just don't show it while
        ///     we tick down the distance the players vehicle must move to get there in the simulation.
        /// </summary>
        public static string CurrentTravelStatus
        {
            get
            {
                var travelStatus = new StringBuilder();
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

        /// <summary>
        ///     Used when the player stops at a location on the trail, or the travel game mode with no attached state. The
        ///     difference this state has from others is showing the name of the location, when between points we don't show this
        ///     since we already know the next point but don't want the player to know that.
        /// </summary>
        public static string CurrentLocationStatus
        {
            get
            {
                var locationStatus = new StringBuilder();
                locationStatus.Append($"--------------------------------{Environment.NewLine}");
                locationStatus.Append(
                    $"{GameSimulationApp.Instance.Trail.GetCurrentPointOfInterest()?.Name}{Environment.NewLine}");
                locationStatus.Append($"{GameSimulationApp.Instance.Time.Date}{Environment.NewLine}");
                locationStatus.Append($"--------------------------------{Environment.NewLine}");
                locationStatus.Append($"Weather: {GameSimulationApp.Instance.Climate.CurrentWeather}{Environment.NewLine}");
                locationStatus.Append($"Health: {GameSimulationApp.Instance.Vehicle.RepairStatus}{Environment.NewLine}");
                locationStatus.Append($"Pace: {GameSimulationApp.Instance.Vehicle.Pace}{Environment.NewLine}");
                locationStatus.Append($"Rations: {GameSimulationApp.Instance.Vehicle.Ration}{Environment.NewLine}");
                locationStatus.Append($"--------------------------------{Environment.NewLine}");
                return locationStatus.ToString();
            }
        }
    }
}