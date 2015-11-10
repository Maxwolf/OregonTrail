using System;
using System.Text;
using TrailEntities.Entity;
using TrailEntities.Mode;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Shows all the players supplies that they currently have in their vehicle inventory, along with the amount of money
    ///     they have. This screen is not for looking at group stats, only items which are normally not shown unlike the travel
    ///     menu that shows basic party stats at all times.
    /// </summary>
    public sealed class CheckSuppliesState : ModeState<TravelInfo>
    {
        /// <summary>
        ///     Determines if the player is finished looking at their current inventory supply listing.
        /// </summary>
        private bool _hasCheckedSupplies;

        /// <summary>
        ///     Holds the computed list of current inventory items in the vehicle that are used by all party members.
        /// </summary>
        private StringBuilder _supplies;

        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public CheckSuppliesState(IMode gameMode, TravelInfo userData) : base(gameMode, userData)
        {
            // Build up representation of supplies once in constructor and then reference when asked for render.
            _supplies = new StringBuilder();
            _supplies.AppendLine($"{Environment.NewLine}Your Supplies{Environment.NewLine}");

            // Loop through every inventory item in the vehicle.
            foreach (var item in GameSimulationApp.Instance.Vehicle.Inventory)
            {
                // Get the next item in the vehicle inventory.
                var itemName = item.Value.Name.ToLowerInvariant();

                // Determine if this item is money and needs special formatting.
                var itemFormattedQuantity = item.Value.Quantity.ToString("N0");
                if (item.Key == SimulationEntity.Cash)
                    itemFormattedQuantity = item.Value.Quantity.ToString("C2");

                // Place tab characters between the item name and the quantity.
                _supplies.AppendFormat("{0} {1}{2}",
                    itemName.PadRight(15),
                    itemFormattedQuantity.PadLeft(3),
                    Environment.NewLine);
            }

            // Wait for user input...
            _supplies.Append($"{Environment.NewLine}{GameSimulationApp.PRESS_ENTER}");
        }

        /// <summary>
        ///     Determines if user input is currently allowed to be typed and filled into the input buffer.
        /// </summary>
        /// <remarks>Default is FALSE. Setting to TRUE allows characters and input buffer to be read when submitted.</remarks>
        public override bool AcceptsInput
        {
            get { return false; }
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string OnRenderState()
        {
            return _supplies.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            if (_hasCheckedSupplies)
                return;

            _hasCheckedSupplies = true;
            ParentMode.CurrentState = null;
        }
    }
}