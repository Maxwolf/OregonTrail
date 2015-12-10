using System.Text;
using TrailSimulation.Core;
using TrailSimulation.Entity;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Holds all the information about traveling that we want to know, such as how long we need to go until next point,
    ///     what our current Windows is like moving, paused, etc.
    /// </summary>
    public sealed class TravelInfo : WindowData
    {
        /// <summary>
        ///     Creates default store implementation.
        /// </summary>
        public TravelInfo()
        {
            // Store so player can buy food, clothes, ammo, etc.
            Store = new StoreGenerator();

            // Allows for other players to see deaths of previous players on the trail.
            TombstoneManager = new TombstoneManager();
        }

        /// <summary>
        ///     References all of the tombstones that this trail might have associated with it, also allows adding of tombstones to
        ///     this trail so other players can encounter them in the future.
        /// </summary>
        public TombstoneManager TombstoneManager { get; }

        /// <summary>
        ///     Reference for any river information that we might need to be holding when we encounter one it will be generated and
        ///     this object filled with needed data that can be accessed by the other states as we attach them.
        /// </summary>
        public RiverGenerator River { get; set; }

        /// <summary>
        ///     Keeps track of all the pending transactions that need to be made when the player visits a store.
        /// </summary>
        public StoreGenerator Store { get; }

        /// <summary>
        ///     Used when the player is traveling on the trail between locations. Also known as drive state in travel game Windows.
        /// </summary>
        public static string DriveStatus
        {
            get
            {
                // GetModule the current food item from vehicle inventory.
                var foodItem = GameSimulationApp.Instance.Vehicle.Inventory[Entities.Food];

                // Set default food status text, update to actual food item total weight if it exists.
                var foodStatus = "0 pounds";
                if (foodItem != null)
                    foodStatus = $"{foodItem.TotalWeight} pounds";

                // Build up the status for the vehicle as it moves through the simulation.
                var driveStatus = new StringBuilder();
                driveStatus.AppendLine("--------------------------------");
                driveStatus.AppendLine($"Date: {GameSimulationApp.Instance.Time.Date}");
                driveStatus.AppendLine(
                    $"Weather: {GameSimulationApp.Instance.Climate.CurrentWeather.ToDescriptionAttribute()}");
                driveStatus.AppendLine($"Health: {GameSimulationApp.Instance.Vehicle.Health}");
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
        ///     Used when the player stops at a location on the trail, or the travel game Windows with no attached state. The
        ///     difference this state has from others is showing the name of the location, when between points we don't show this
        ///     since we already know the next point but don't want the player to know that.
        /// </summary>
        public static string TravelStatus
        {
            get
            {
                var showLocationName = GameSimulationApp.Instance.Trail.CurrentLocation.Status == LocationStatus.Arrived;
                var locationStatus = new StringBuilder();
                locationStatus.AppendLine("--------------------------------");

                // Only add the location name if we are on the next point, otherwise we should not show this.
                locationStatus.AppendLine(showLocationName
                    ? GameSimulationApp.Instance.Trail.CurrentLocation.Name
                    : $"{GameSimulationApp.Instance.Trail.DistanceToNextLocation.ToString("N0")} miles to {GameSimulationApp.Instance.Trail.NextLocation.Name}");

                locationStatus.AppendLine($"{GameSimulationApp.Instance.Time.Date}");
                locationStatus.AppendLine("--------------------------------");
                locationStatus.AppendLine(
                    $"Weather: {GameSimulationApp.Instance.Climate.CurrentWeather.ToDescriptionAttribute()}");
                locationStatus.AppendLine($"Health: {GameSimulationApp.Instance.Vehicle.Health}");
                locationStatus.AppendLine($"Pace: {GameSimulationApp.Instance.Vehicle.Pace}");
                locationStatus.AppendLine($"Rations: {GameSimulationApp.Instance.Vehicle.Ration}");
                locationStatus.AppendLine("--------------------------------");
                return locationStatus.ToString();
            }
        }

        /// <summary>
        ///     TombstoneItem for the player (or another dead player) that will be either viewed or shown to user so they can
        ///     confirm
        ///     their graves details.
        /// </summary>
        public TombstoneItem TombstoneItem { get; set; }
    }
}