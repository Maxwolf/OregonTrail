using System;
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
        ///     Used when the player is traveling on the trail between locations. Also known as drive state in travel game mode.
        /// </summary>
        public string DriveStatus
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
                driveStatus.Append($"--------------------------------{Environment.NewLine}");
                driveStatus.Append($"Date: {GameSimulationApp.Instance.Time.Date}{Environment.NewLine}");
                driveStatus.Append($"Weather: {GameSimulationApp.Instance.Climate.CurrentWeather}{Environment.NewLine}");
                driveStatus.Append($"Health: {GameSimulationApp.Instance.Vehicle.RepairStatus}{Environment.NewLine}");
                driveStatus.Append($"Food: {foodStatus}{Environment.NewLine}");
                driveStatus.Append(
                    $"Next landmark: {GameSimulationApp.Instance.Trail.DistanceToNextLocation} miles{Environment.NewLine}");
                driveStatus.Append(
                    $"Miles traveled: {GameSimulationApp.Instance.Vehicle.Odometer} miles{Environment.NewLine}");
                driveStatus.Append($"--------------------------------{Environment.NewLine}");
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
        public string TravelStatus
        {
            get
            {
                var showLocationName = GameSimulationApp.Instance.Trail.ReachedNextPoint;
                var locationStatus = new StringBuilder();
                locationStatus.AppendLine($"--------------------------------");

                // Only add the location name if we are on the next point, otherwise we should not show this.
                if (showLocationName)
                    locationStatus.AppendLine(GameSimulationApp.Instance.Trail.CurrentLocation.Name);

                locationStatus.AppendLine($"{GameSimulationApp.Instance.Time.Date}");
                locationStatus.AppendLine($"--------------------------------");
                locationStatus.AppendLine($"Weather: {GameSimulationApp.Instance.Climate.CurrentWeather}");
                locationStatus.AppendLine($"Health: {GameSimulationApp.Instance.Vehicle.RepairStatus}");
                locationStatus.AppendLine($"Pace: {GameSimulationApp.Instance.Vehicle.Pace}");
                locationStatus.AppendLine($"Rations: {GameSimulationApp.Instance.Vehicle.Ration}");
                locationStatus.AppendLine($"--------------------------------");
                return locationStatus.ToString();
            }
        }
    }
}