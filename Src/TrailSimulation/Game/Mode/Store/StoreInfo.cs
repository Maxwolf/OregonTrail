using System.Collections.Generic;
using TrailSimulation.Core;
using TrailSimulation.Entity;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Before any items are removed, or added to the store all the interactions are stored in receipt info object. When
    ///     the game mode for the store is removed all the transactions will be completed and the players vehicle updated and
    ///     the store items removed, and balances of both updated respectfully.
    /// </summary>
    public sealed class StoreInfo : ModeInfo
    {
        /// <summary>
        ///     Keeps track of all the pending transactions that need to be made.
        /// </summary>
        private Dictionary<SimEntity, SimItem> _totalTransactions;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailSimulation.Game.StoreInfo" /> class.
        /// </summary>
        public StoreInfo()
        {
            _totalTransactions = new Dictionary<SimEntity, SimItem>(GameSimulationApp.DefaultInventory);
        }

        /// <summary>
        ///     Keeps track of all the pending transactions that need to be made.
        /// </summary>
        public IDictionary<SimEntity, SimItem> Transactions
        {
            get { return _totalTransactions; }
        }

        /// <summary>
        ///     Item which the player does not have enough of or is missing.
        /// </summary>
        internal SimItem SelectedItem { get; set; }

        public void ClearTransactions()
        {
            _totalTransactions.Clear();
        }

        /// <summary>
        ///     Returns the total cost of all the transactions this receipt information object represents.
        /// </summary>
        public float GetTransactionTotalCost()
        {
            // Loop through all transactions and multiply amount by cost.
            float totalCost = 0;
            foreach (var item in _totalTransactions)
            {
                totalCost += item.Value.Quantity*item.Value.Cost;
            }

            // Cast to unsigned integer and return.
            return totalCost;
        }

        /// <summary>
        ///     Adds an SimItem to the list of pending transactions. If it already exists it will be replaced.
        /// </summary>
        public void AddItem(SimItem item, int amount)
        {
            _totalTransactions[item.Category] = new SimItem(item, amount);
        }

        /// <summary>
        ///     Removes an SimItem from the list of pending transactions. If it does not exist then nothing will happen.
        /// </summary>
        public void RemoveItem(SimItem item)
        {
            // Loop through every single transaction.
            var copyList = new Dictionary<SimEntity, SimItem>(_totalTransactions);
            foreach (var transaction in copyList)
            {
                // Check if SimItem name matches incoming one.
                if (!transaction.Key.Equals(item.Category))
                    continue;

                // Reset the simulation SimItem to default values, meaning the player has none of them.
                _totalTransactions[item.Category].Reset();
                break;
            }
        }
    }
}