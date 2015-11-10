using System.Collections.Generic;
using TrailEntities.Entity;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Before any items are removed, or added to the store all the interactions are stored in receipt info object. When
    ///     the game mode for the store is removed all the transactions will be completed and the players vehicle updated and
    ///     the store items removed, and balances of both updated respectfully.
    /// </summary>
    public sealed class StoreInfo
    {
        /// <summary>
        ///     Keeps track of all the pending transactions that need to be made.
        /// </summary>
        private Dictionary<Entity.Entity, Item> _totalTransactions;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Game.StoreInfo" /> class.
        /// </summary>
        public StoreInfo()
        {
            _totalTransactions = new Dictionary<Entity.Entity, Item>(GameSimulationApp.DefaultInventory);
        }

        /// <summary>
        ///     Keeps track of all the pending transactions that need to be made.
        /// </summary>
        public IDictionary<Entity.Entity, Item> Transactions
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
        ///     Adds an Item to the list of pending transactions. If it already exists it will be replaced.
        /// </summary>
        public void AddItem(Item item, int amount)
        {
            _totalTransactions[item.Category] = new Item(item, amount);
        }

        /// <summary>
        ///     Removes an Item from the list of pending transactions. If it does not exist then nothing will happen.
        /// </summary>
        public void RemoveItem(Item item)
        {
            // Loop through every single transaction.
            var copyList = new Dictionary<Entity.Entity, Item>(_totalTransactions);
            foreach (var transaction in copyList)
            {
                // Check if Item name matches incoming one.
                if (!transaction.Key.Equals(item.Category))
                    continue;

                // Reset the simulation item to default values, meaning the player has none of them.
                _totalTransactions[item.Category].Reset();
                break;
            }
        }
    }
}