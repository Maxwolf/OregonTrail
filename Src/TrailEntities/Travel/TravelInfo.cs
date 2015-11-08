using System;
using System.Text;
using TrailEntities.Entity;

namespace TrailEntities.Travel
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

        /// <summary>
        ///     Used when the player is traveling on the trail between locations. Also known as drive state in travel game mode.
        /// </summary>
        public static string DriveStatus
        {
            get
            {
                // Get the current food item from vehicle inventory.
                var foodItem = GameSimApp.Instance.Vehicle.Inventory[SimEntity.Food];

                // Set default food status text, update to actual food item total weight if it exists.
                var foodStatus = "0 pounds";
                if (foodItem != null)
                    foodStatus = $"{foodItem.TotalWeight} pounds";

                // Build up the status for the vehicle as it moves through the simulation.
                var driveStatus = new StringBuilder();
                driveStatus.Append($"--------------------------------{Environment.NewLine}");
                driveStatus.Append($"Date: {GameSimApp.Instance.Time.Date}{Environment.NewLine}");
                driveStatus.Append($"Weather: {GameSimApp.Instance.Climate.CurrentWeather}{Environment.NewLine}");
                driveStatus.Append($"Health: {GameSimApp.Instance.Vehicle.RepairStatus}{Environment.NewLine}");
                driveStatus.Append($"Food: {foodStatus}{Environment.NewLine}");
                driveStatus.Append(
                    $"Next landmark: {GameSimApp.Instance.Trail.DistanceToNextLocation} miles{Environment.NewLine}");
                driveStatus.Append(
                    $"Miles traveled: {GameSimApp.Instance.Vehicle.Odometer} miles{Environment.NewLine}");
                driveStatus.Append($"--------------------------------{Environment.NewLine}");
                return driveStatus.ToString();
            }
        }

        /// <summary>
        ///     Used when the player stops at a location on the trail, or the travel game mode with no attached state. The
        ///     difference this state has from others is showing the name of the location, when between points we don't show this
        ///     since we already know the next point but don't want the player to know that.
        /// </summary>
        public static string TravelStatus(bool showLocationName)
        {
            var locationStatus = new StringBuilder();
            locationStatus.Append($"--------------------------------{Environment.NewLine}");

            // Only add the location name if we are on the next point, otherwise we should not show this.
            if (showLocationName)
            {
                var currentTrailLocation = GameSimApp.Instance.Trail.GetCurrentLocation();
                if (currentTrailLocation != null)
                    locationStatus.AppendLine(currentTrailLocation.Name);
            }

            locationStatus.Append($"{GameSimApp.Instance.Time.Date}{Environment.NewLine}");
            locationStatus.Append($"--------------------------------{Environment.NewLine}");
            locationStatus.Append($"Weather: {GameSimApp.Instance.Climate.CurrentWeather}{Environment.NewLine}");
            locationStatus.Append($"Health: {GameSimApp.Instance.Vehicle.RepairStatus}{Environment.NewLine}");
            locationStatus.Append($"Pace: {GameSimApp.Instance.Vehicle.Pace}{Environment.NewLine}");
            locationStatus.Append($"Rations: {GameSimApp.Instance.Vehicle.Ration}{Environment.NewLine}");
            locationStatus.Append($"--------------------------------{Environment.NewLine}");
            return locationStatus.ToString();
        }
    }
}