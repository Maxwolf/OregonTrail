using System.Collections.Generic;
using TrailEntities.Entity;
using TrailEntities.Simulation;
using TrailEntities.Simulation.Mode;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Before any items are removed, or added to the store all the interactions are stored in receipt info object. When
    ///     the game mode for the store is removed all the transactions will be completed and the players vehicle updated and
    ///     the store items removed, and balances of both updated respectfully.
    /// </summary>
    public sealed class StoreInfo : IModeInfo
    {
        /// <summary>
        ///     Keeps track of all the pending transactions that need to be made.
        /// </summary>
        private Dictionary<SimulationEntity, SimulationItem> _totalTransactions;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Game.StoreInfo" /> class.
        /// </summary>
        public StoreInfo()
        {
            _totalTransactions = new Dictionary<SimulationEntity, SimulationItem>(GameSimulationApp.DefaultInventory);
        }

        /// <summary>
        ///     Keeps track of all the pending transactions that need to be made.
        /// </summary>
        public IDictionary<SimulationEntity, SimulationItem> Transactions
        {
            get { return _totalTransactions; }
        }

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
        ///     Adds an SimulationItem to the list of pending transactions. If it already exists it will be replaced.
        /// </summary>
        public void AddItem(SimulationItem item, int amount)
        {
            _totalTransactions[item.Category] = new SimulationItem(item, amount);
        }

        /// <summary>
        ///     Removes an SimulationItem from the list of pending transactions. If it does not exist then nothing will happen.
        /// </summary>
        public void RemoveItem(SimulationItem item)
        {
            // Loop through every single transaction.
            var copyList = new Dictionary<SimulationEntity, SimulationItem>(_totalTransactions);
            foreach (var transaction in copyList)
            {
                // Check if SimulationItem name matches incoming one.
                if (!transaction.Key.Equals(item.Category))
                    continue;

                // Reset the simulation SimulationItem to default values, meaning the player has none of them.
                _totalTransactions[item.Category].Reset();
                break;
            }
        }
    }
}