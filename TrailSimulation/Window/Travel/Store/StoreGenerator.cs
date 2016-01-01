// Created by Ron 'Maxwolf' McDowell (ron.mcdowell@gmail.com) 
// Timestamp 12/31/2015@4:38 AM

namespace TrailSimulation.Window.Travel.Store
{
    using System;
    using System.Collections.Generic;
    using Entity;
    using Entity.Item;
    using Entity.Location;
    using Entity.Vehicle;

    /// <summary>
    ///     Before any items are removed, or added to the store all the interactions are stored in receipt info object. When
    ///     the game mode for the store is removed all the transactions will be completed and the players vehicle updated and
    ///     the store items removed, and balances of both updated respectfully.
    /// </summary>
    public sealed class StoreGenerator
    {
        /// <summary>
        ///     Keeps track of all the pending transactions that need to be made.
        /// </summary>
        private Dictionary<Entities, SimItem> _totalTransactions;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StoreGenerator" /> class.
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public StoreGenerator()
        {
            Reset();
        }

        /// <summary>
        ///     Item which the player does not have enough of or is missing.
        /// </summary>
        public SimItem SelectedItem { get; set; }

        /// <summary>
        ///     Keeps track of all the pending transactions that need to be made.
        /// </summary>
        public IDictionary<Entities, SimItem> Transactions
        {
            get { return _totalTransactions; }
        }

        /// <summary>
        ///     Returns the total cost of all the transactions this receipt information object represents.
        /// </summary>
        public float TotalTransactionCost
        {
            get
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
        }

        /// <summary>
        ///     Checks if the player has enough animals to pull their vehicle.
        /// </summary>
        /// <returns>TRUE if player is missing enough items to correctly start the game, FALSE if everything is OK.</returns>
        internal bool MissingImportantItems
        {
            get
            {
                return GameSimulationApp.Instance.Trail.IsFirstLocation &&
                       GameSimulationApp.Instance.Trail.CurrentLocation?.Status == LocationStatus.Unreached &&
                       _totalTransactions[Entities.Animal].Quantity <= 0;
            }
        }

        /// <summary>
        ///     Processes all of the pending transactions in the store receipt info object.
        /// </summary>
        internal void PurchaseItems()
        {
            // Grab the total transaction cost once since it requires work to get the property value.
            var totalBill = TotalTransactionCost;

            // Throws exception if player cannot afford items. Developer calling this at wrong time!
            if (GameSimulationApp.Instance.Vehicle.Balance < totalBill)
                throw new InvalidOperationException(
                    "Attempted to purchase items the player does not have enough monies for!");

            // Loop through all the pending transaction and buy them out.
            foreach (var transaction in _totalTransactions)
            {
                GameSimulationApp.Instance.Vehicle.Purchase(transaction.Value);
            }

            // Remove all the transactions now that we have processed them.
            Reset();
        }

        /// <summary>
        ///     Cleans out all the transactions, if they have not been processed yet then they will be lost forever.
        /// </summary>
        private void Reset()
        {
            _totalTransactions = new Dictionary<Entities, SimItem>(Vehicle.DefaultInventory);
        }

        /// <summary>Adds an SimItem to the list of pending transactions. If it already exists it will be replaced.</summary>
        /// <param name="item">The item.</param>
        /// <param name="amount">The amount.</param>
        public void AddItem(SimItem item, int amount)
        {
            _totalTransactions[item.Category] = new SimItem(item, amount);
        }

        /// <summary>Removes an SimItem from the list of pending transactions. If it does not exist then nothing will happen.</summary>
        /// <param name="item">The item.</param>
        public void RemoveItem(SimItem item)
        {
            // Loop through every single transaction.
            var copyList = new Dictionary<Entities, SimItem>(_totalTransactions);
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