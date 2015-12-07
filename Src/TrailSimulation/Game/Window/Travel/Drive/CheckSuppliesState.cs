using System;
using System.Text;
using TrailSimulation.Core;
using TrailSimulation.Entity;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Shows all the players supplies that they currently have in their vehicle inventory, along with the amount of money
    ///     they have. This screen is not for looking at group stats, only items which are normally not shown unlike the travel
    ///     menu that shows basic party stats at all times.
    /// </summary>
    [RequiredWindow(Windows.Travel)]
    public sealed class CheckSuppliesState : DialogState<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public CheckSuppliesState(IWindow gameMode) : base(gameMode)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            // Build up representation of supplies once in constructor and then reference when asked for render.
            var _supplies = new StringBuilder();
            _supplies.AppendLine($"{Environment.NewLine}Your Supplies{Environment.NewLine}");

            // Loop through every inventory item in the vehicle.
            foreach (var item in GameSimulationApp.Instance.Vehicle.Inventory)
            {
                // GetModule the next item in the vehicle inventory.
                var itemName = item.Value.Name.ToLowerInvariant();

                // Apply number formatting to quantities so they have thousand separators.
                var itemFormattedQuantity = item.Value.Quantity.ToString("N0");

                // Skip money, it is only shown in store.
                if (item.Key == Entities.Cash)
                    continue;

                // Place tab characters between the item name and the quantity.
                _supplies.AppendFormat("{0} {1}{2}",
                    itemName.PadRight(15),
                    itemFormattedQuantity.PadLeft(3),
                    Environment.NewLine);
            }

            // Add one more new line at the end of the table.
            _supplies.Append(Environment.NewLine);


            return _supplies.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            //parentGameMode.State = null;
            ClearForm();
        }
    }
}