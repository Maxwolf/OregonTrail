using System.Text;
using TrailSimulation.Core;
using TrailSimulation.Entity;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Holds all the information about traveling that we want to know, such as how long we need to go until next point,
    ///     what our current mode is like moving, paused, etc.
    /// </summary>
    public sealed class TravelInfo : ModeInfo
    {
        /// <summary>
        ///     Reference for any river information that we might need to be holding when we encounter one it will be generated and
        ///     this object filled with needed data that can be accessed by the other states as we attach them.
        /// </summary>
        public RiverGenerator RiverInfo { get; internal set; }

        /// <summary>
        ///     Used when the player is traveling on the trail between locations. Also known as drive state in travel game mode.
        /// </summary>
        public static string DriveStatus
        {
            get
            {
                // GetModule the current food item from vehicle inventory.
                var foodItem = GameSimulationApp.Instance.Vehicle.Inventory[SimEntity.Food];

                // Set default food status text, update to actual food item total weight if it exists.
                var foodStatus = "0 pounds";
                if (foodItem != null)
                    foodStatus = $"{foodItem.TotalWeight} pounds";

                // Build up the status for the vehicle as it moves through the simulation.
                var driveStatus = new StringBuilder();
                driveStatus.AppendLine("--------------------------------");
                driveStatus.AppendLine($"Date: {GameSimulationApp.Instance.Time.Date}");
                driveStatus.AppendLine($"Weather: {GameSimulationApp.Instance.Climate.CurrentWeather}");
                driveStatus.AppendLine($"Health: {GameSimulationApp.Instance.Vehicle.RepairLevel}");
                driveStatus.AppendLine($"Food: {foodStatus}");
                driveStatus.AppendLine(
                    $"Next landmark: {GameSimulationApp.Instance.Trail.DistanceToNextLocation} miles");
                driveStatus.AppendLine(
                    $"Miles traveled: {GameSimulationApp.Instance.Vehicle.Odometer} miles");
                driveStatus.AppendLine("--------------------------------");
                return driveStatus.ToString();
            }
        }

        /// <summary>
        ///     Determines how many days of rest the player had, and were simulated both in time and on event system.
        /// </summary>
        public int DaysToRest { get; internal set; }

        /// <summary>
        ///     Used when the player stops at a location on the trail, or the travel game mode with no attached state. The
        ///     difference this state has from others is showing the name of the location, when between points we don't show this
        ///     since we already know the next point but don't want the player to know that.
        /// </summary>
        public static string TravelStatus
        {
            get
            {
                var showLocationName = GameSimulationApp.Instance.Trail.CurrentLocation.Status >
                                       LocationStatus.Unreached;
                var locationStatus = new StringBuilder();
                locationStatus.AppendLine("--------------------------------");

                // Only add the location name if we are on the next point, otherwise we should not show this.
                if (showLocationName)
                    locationStatus.AppendLine(GameSimulationApp.Instance.Trail.CurrentLocation.Name);

                locationStatus.AppendLine($"{GameSimulationApp.Instance.Time.Date}");
                locationStatus.AppendLine("--------------------------------");
                locationStatus.AppendLine($"Weather: {GameSimulationApp.Instance.Climate.CurrentWeather}");
                locationStatus.AppendLine($"Health: {GameSimulationApp.Instance.Vehicle.RepairLevel}");
                locationStatus.AppendLine($"Pace: {GameSimulationApp.Instance.Vehicle.Pace}");
                locationStatus.AppendLine($"Rations: {GameSimulationApp.Instance.Vehicle.Ration}");
                locationStatus.AppendLine("--------------------------------");
                return locationStatus.ToString();
            }
        }
    }
}