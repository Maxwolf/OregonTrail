// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TravelInfo.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Holds all the information about traveling that we want to know, such as how long we need to go until next point,
//   what our current Windows is like moving, paused, etc.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



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
        /// Initializes a new instance of the <see cref="TravelInfo"/> class. 
        ///     Creates default store implementation.
        /// </summary>
        public TravelInfo()
        {
            // Store so player can buy food, clothes, ammo, etc.
            Store = new StoreGenerator();
        }

        /// <summary>
        ///     Reference for any river information that we might need to be holding when we encounter one it will be generated and
        ///     this object filled with needed data that can be accessed by the other states as we attach them.
        /// </summary>
        public RiverGenerator River { get; private set; }

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
                // Grab instance of game simulation.
                var game = GameSimulationApp.Instance;

                // GetModule the current food item from vehicle inventory.
                var foodItem = game.Vehicle.Inventory[Entities.Food];

                // Set default food status text, update to actual food item total weight if it exists.
                var foodStatus = "0 pounds";
                if (foodItem != null)
                    foodStatus = $"{foodItem.TotalWeight} pounds";

                // Build up the status for the vehicle as it moves through the simulation.
                var driveStatus = new StringBuilder();
                driveStatus.AppendLine("--------------------------------");
                driveStatus.AppendLine($"Date: {game.Time.Date}");
                driveStatus.AppendLine(
                    $"Weather: {game.Trail.CurrentLocation.Weather.ToDescriptionAttribute()}");
                driveStatus.AppendLine($"Health: {game.Vehicle.PassengerHealth.ToDescriptionAttribute()}");
                driveStatus.AppendLine($"Food: {foodStatus}");
                driveStatus.AppendLine($"Next landmark: {game.Trail.DistanceToNextLocation} miles");
                driveStatus.AppendLine($"Miles traveled: {game.Vehicle.Odometer} miles");
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
                // Grab instance of game simulation.
                var game = GameSimulationApp.Instance;

                var showLocationName = game.Trail.CurrentLocation.Status == LocationStatus.Arrived;
                var locationStatus = new StringBuilder();
                locationStatus.AppendLine("--------------------------------");

                // Only add the location name if we are on the next point, otherwise we should not show this.
                locationStatus.AppendLine(showLocationName
                    ? game.Trail.CurrentLocation.Name
                    : $"{game.Trail.DistanceToNextLocation.ToString("N0")} miles to {game.Trail.NextLocation.Name}");

                locationStatus.AppendLine($"{game.Time.Date}");
                locationStatus.AppendLine("--------------------------------");
                locationStatus.AppendLine(
                    $"Weather: {game.Trail.CurrentLocation.Weather.ToDescriptionAttribute()}");
                locationStatus.AppendLine($"Health: {game.Vehicle.PassengerHealth.ToDescriptionAttribute()}");
                locationStatus.AppendLine($"Pace: {game.Vehicle.Pace.ToDescriptionAttribute()}");
                locationStatus.AppendLine($"Rations: {game.Vehicle.Ration.ToDescriptionAttribute()}");
                locationStatus.AppendLine("--------------------------------");
                return locationStatus.ToString();
            }
        }


        /// <summary>
        ///     Creates a new river that can be accessed as a property from the travel game window.
        /// </summary>
        public void GenerateRiver()
        {
            // Skip if river has already been created.
            if (River != null)
                return;

            // Creates a new river.
            River = new RiverGenerator();
        }

        /// <summary>
        ///     Destroys all of the data associated with the previous river the player encountered.
        /// </summary>
        public void DestroyRiver()
        {
            // Skip if the river is already null.
            if (River == null)
                return;

            // Destroy the river data.
            River = null;
        }
    }
}